using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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
        using var semSessao = await cliente.GetAsync("/api/admin/postagens");
        Assert.Equal(HttpStatusCode.Unauthorized, semSessao.StatusCode);

        using var semCadastro = await cliente.PostAsJsonAsync("/api/auth/register", new
        {
            email = _fixture.Factory.EmailAdministrador,
            senha = _fixture.Factory.SenhaAdministrador
        });
        Assert.Equal(HttpStatusCode.NotFound, semCadastro.StatusCode);

        var token = await ObterTokenCsrfAsync(cliente);
        using var requisicao = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = JsonContent.Create(new
            {
                email = _fixture.Factory.EmailAdministrador,
                senha = _fixture.Factory.SenhaAdministrador
            })
        };
        requisicao.Headers.Add("X-XSRF-TOKEN", token);
        using var login = await cliente.SendAsync(requisicao);
        Assert.True(login.IsSuccessStatusCode, await login.Content.ReadAsStringAsync());
        Assert.Contains(login.Headers.GetValues("Set-Cookie"), valor =>
            valor.Contains("CircuitoTereVerde.Auth=", StringComparison.Ordinal)
                && valor.Contains("httponly", StringComparison.OrdinalIgnoreCase));

        using var atual = await cliente.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, atual.StatusCode);
        using var perfil = JsonDocument.Parse(await atual.Content.ReadAsStringAsync());
        Assert.Contains("ADMINISTRADOR", perfil.RootElement.GetProperty("perfis")
            .EnumerateArray().Select(item => item.GetString()));

        using var acessoAdmin = await cliente.GetAsync("/api/admin/postagens");
        Assert.Equal(HttpStatusCode.OK, acessoAdmin.StatusCode);

        using var logout = await ApiTesteHelpers.EnviarComCsrfAsync(
            cliente, HttpMethod.Post, "/api/auth/logout");
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        using var semSessaoAposLogout = await cliente.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, semSessaoAposLogout.StatusCode);
    }

    private static async Task<string> ObterTokenCsrfAsync(HttpClient cliente)
    {
        using var resposta = await cliente.GetAsync("/api/auth/csrf");
        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"A API de teste respondeu {(int)resposta.StatusCode}: {detalhe}");
        }
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        return documento.RootElement.GetProperty("token").GetString()!;
    }

    #endregion
}
