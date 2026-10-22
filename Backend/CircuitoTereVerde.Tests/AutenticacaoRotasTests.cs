using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CircuitoTereVerde.Core.Infra.Context;
using CircuitoTereVerde.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CircuitoTereVerde.Tests;

[Collection(ApiTesteCollection.Nome)]
public sealed class AutenticacaoRotasTests
{
    private readonly ApiTesteFixture _fixture;

    public AutenticacaoRotasTests(ApiTesteFixture fixture)
    {
        _fixture = fixture;
    }

    #region Login, sessão e acesso administrativo

    [Fact]
    public async Task LoginAdministraSessaoHttpOnlyEProtegeAsRotas()
    {
        using var cliente = _fixture.Factory.CreateClient();
        // Uma chamada administrativa sem sessão precisa responder 401, sem redirecionar para HTML.
        using var semSessao = await cliente.GetAsync("/api/admin/postagens");
        Assert.Equal(HttpStatusCode.Unauthorized, semSessao.StatusCode);

        // O MVP não oferece cadastro público de contas.
        using var semCadastro = await cliente.PostAsJsonAsync("/api/auth/register", new
        {
            email = _fixture.Factory.EmailAdministrador,
            senha = _fixture.Factory.SenhaAdministrador
        });
        Assert.Equal(HttpStatusCode.NotFound, semCadastro.StatusCode);

        // O login usa a conta do appsettings.Development e cria o cookie protegido.
        using var login = await cliente.PostAsJsonAsync("/api/auth/login", new
        {
            email = _fixture.Factory.EmailAdministrador,
            senha = _fixture.Factory.SenhaAdministrador
        });
        Assert.True(login.IsSuccessStatusCode, await login.Content.ReadAsStringAsync());
        Assert.Contains(login.Headers.GetValues("Set-Cookie"), valor =>
            valor.Contains("CircuitoTereVerde.Auth=", StringComparison.Ordinal)
                && valor.Contains("httponly", StringComparison.OrdinalIgnoreCase)
                && valor.Contains("secure", StringComparison.OrdinalIgnoreCase)
                && valor.Contains("samesite=strict", StringComparison.OrdinalIgnoreCase));

        // O cliente reaproveita o cookie para consultar a sessão e acessar as postagens administrativas.
        using var atual = await cliente.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, atual.StatusCode);
        using var perfil = JsonDocument.Parse(await atual.Content.ReadAsStringAsync());
        Assert.Contains("ADMINISTRADOR", perfil.RootElement.GetProperty("perfis")
            .EnumerateArray().Select(item => item.GetString()));

        using var acessoAdmin = await cliente.GetAsync("/api/admin/postagens");
        Assert.Equal(HttpStatusCode.OK, acessoAdmin.StatusCode);

        // O logout encerra a sessão; a próxima consulta deve voltar a responder 401.
        using var logout = await cliente.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        using var semSessaoAposLogout = await cliente.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, semSessaoAposLogout.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/admin/postagens")]
    [InlineData("GET", "/api/admin/postagens/selecionada")]
    [InlineData("POST", "/api/admin/postagens")]
    [InlineData("PUT", "/api/admin/postagens")]
    [InlineData("DELETE", "/api/admin/postagens")]
    public async Task TodasAsOperacoesAdministrativasExigemLogin(string metodo, string rota)
    {
        // Nenhuma operação administrativa pode ser executada por um visitante sem sessão.
        using var cliente = _fixture.Factory.CreateClient();
        using var requisicao = ApiTesteHelpers.CriarRequisicaoComId(new HttpMethod(metodo), rota, 1);
        using var resposta = await cliente.SendAsync(requisicao);
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task UsuarioSemPerfilAdministrativoRecebe403()
    {
        await using var escopo = _fixture.Factory.Services.CreateAsyncScope();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        // A conta existe apenas para conferir a autorização e é removida no final do teste.
        var usuario = await CriarContaDeTesteAsync(usuarios, "USUARIO");

        try
        {
            using var cliente = _fixture.Factory.CreateClient();
            using var login = await cliente.PostAsJsonAsync("/api/auth/login", new
            {
                email = usuario.Email,
                senha = _fixture.Factory.SenhaAdministrador
            });
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);

            // A sessão é válida, mas o perfil USUARIO não pode listar nem alterar postagens administrativas.
            foreach (var metodo in new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete })
            {
                using var requisicao = ApiTesteHelpers.CriarRequisicaoComId(metodo, "/api/admin/postagens", 1);
                using var resposta = await cliente.SendAsync(requisicao);
                Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            }
        }
        finally
        {
            Assert.True((await usuarios.DeleteAsync(usuario)).Succeeded);
        }
    }

    [Fact]
    public async Task LoginBloqueiaCincoErrosEPermiteEntrarAposOBloqueio()
    {
        await using var escopo = _fixture.Factory.Services.CreateAsyncScope();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var contexto = escopo.ServiceProvider.GetRequiredService<CircuitoTereVerdeDbContext>();
        // Usa outra conta administrativa para não bloquear o administrador de desenvolvimento.
        var administrador = await CriarContaDeTesteAsync(usuarios, "ADMINISTRADOR");

        try
        {
            using var cliente = _fixture.Factory.CreateClient();
            var inicioTentativas = DateTimeOffset.UtcNow;

            for (var tentativa = 1; tentativa <= 5; tentativa++)
            {
                using var resposta = await cliente.PostAsJsonAsync("/api/auth/login", new
                {
                    email = administrador.Email,
                    senha = "SenhaIncorreta123!"
                });
                Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);

                // Confere no SQLite que cada tentativa foi registrada pelo Identity.
                var persistido = await contexto.Users.AsNoTracking().SingleAsync(item => item.Id == administrador.Id);
                if (tentativa < 5)
                {
                    Assert.Equal(tentativa, persistido.AccessFailedCount);
                }
                else
                {
                    Assert.NotNull(persistido.LockoutEnd);
                    Assert.InRange(persistido.LockoutEnd.Value,
                        inicioTentativas.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(10));
                }
            }

            // Mesmo a senha correta deve ser recusada durante o bloqueio.
            using var bloqueado = await cliente.PostAsJsonAsync("/api/auth/login", new
            {
                email = administrador.Email,
                senha = _fixture.Factory.SenhaAdministrador
            });
            Assert.Equal(HttpStatusCode.Unauthorized, bloqueado.StatusCode);

            // Antecipa no banco o fim do bloqueio da conta de teste, sem esperar dez minutos.
            contexto.ChangeTracker.Clear();
            administrador = (await usuarios.FindByIdAsync(administrador.Id))!;
            Assert.True((await usuarios.SetLockoutEndDateAsync(administrador, DateTimeOffset.UtcNow.AddMinutes(-1))).Succeeded);

            using var liberado = await cliente.PostAsJsonAsync("/api/auth/login", new
            {
                email = administrador.Email,
                senha = _fixture.Factory.SenhaAdministrador
            });
            Assert.Equal(HttpStatusCode.OK, liberado.StatusCode);
            var aposLogin = await contexto.Users.AsNoTracking().SingleAsync(item => item.Id == administrador.Id);
            Assert.Equal(0, aposLogin.AccessFailedCount);
        }
        finally
        {
            Assert.True((await usuarios.DeleteAsync(administrador)).Succeeded);
        }
    }

    [Fact]
    public async Task LoginDeContaInexistenteRecebe401()
    {
        // Um e-mail desconhecido recebe a mesma resposta de credenciais inválidas.
        using var cliente = _fixture.Factory.CreateClient();
        using var resposta = await cliente.PostAsJsonAsync("/api/auth/login", new
        {
            email = $"inexistente-{Guid.NewGuid():N}@circuitotereverde.test",
            senha = _fixture.Factory.SenhaAdministrador
        });
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task LoginHttpLocalCriaCookieSemSecure()
    {
        // A apresentação local usa HTTP; neste caso o cookie precisa ser aceito pelo navegador.
        using var cliente = _fixture.Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });

        using var login = await cliente.PostAsJsonAsync("/api/auth/login", new
        {
            email = _fixture.Factory.EmailAdministrador,
            senha = _fixture.Factory.SenhaAdministrador
        });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.DoesNotContain(login.Headers.GetValues("Set-Cookie"), valor =>
            valor.Contains("secure", StringComparison.OrdinalIgnoreCase));

        // A mesma sessão HTTP local deve acessar a rota protegida normalmente.
        using var sessao = await cliente.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, sessao.StatusCode);
    }

    [Fact]
    public void ProducaoConfiguraCookieSempreSeguro()
    {
        // A configuração de produção exige Secure mesmo antes de uma requisição chegar à API.
        using var fabrica = new ApiTesteFactory(Environments.Production);
        var opcoes = fabrica.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        var cookie = opcoes.Get(IdentityConstants.ApplicationScheme).Cookie;

        Assert.True(cookie.HttpOnly);
        Assert.Equal(CookieSecurePolicy.Always, cookie.SecurePolicy);
        Assert.Equal(SameSiteMode.Strict, cookie.SameSite);
    }

    #endregion

    #region Contas de teste no banco real

    private async Task<ApplicationUser> CriarContaDeTesteAsync(UserManager<ApplicationUser> usuarios, string perfil)
    {
        var email = $"teste-{Guid.NewGuid():N}@circuitotereverde.test";
        var usuario = new ApplicationUser { UserName = email, Email = email, Nome = "Conta de teste" };
        Assert.True((await usuarios.CreateAsync(usuario, _fixture.Factory.SenhaAdministrador)).Succeeded);
        Assert.True((await usuarios.AddToRoleAsync(usuario, perfil)).Succeeded);
        return usuario;
    }

    #endregion
}
