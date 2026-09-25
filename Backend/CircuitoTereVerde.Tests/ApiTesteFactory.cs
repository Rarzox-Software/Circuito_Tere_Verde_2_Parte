using CircuitoTereVerde.Core.Infra.Context;
using CircuitoTereVerde.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CircuitoTereVerde.Tests;

// A API de teste usa o appsettings.Development e o banco SQLite fixo do projeto.
public sealed class ApiTesteFactory : WebApplicationFactory<Program>
{
    public string EmailAdministrador { get; } = $"teste-{Guid.NewGuid():N}@circuito.local";
    public string SenhaAdministrador { get; } = "TesteApi2026!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var diretorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (diretorio is not null
            && !File.Exists(Path.Combine(diretorio.FullName, "CircuitoTereVerdeWebApi", "appsettings.json")))
        {
            diretorio = diretorio.Parent;
        }

        if (diretorio is null)
        {
            throw new DirectoryNotFoundException("Não foi possível localizar as configurações da API para o teste.");
        }

        var raizApi = Path.Combine(diretorio.FullName, "CircuitoTereVerdeWebApi");

        builder.UseEnvironment(Environments.Development);
        builder.UseContentRoot(raizApi);
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            var pastaChaves = Path.Combine(diretorio.FullName, "build-codex", "data-protection-keys");
            Directory.CreateDirectory(pastaChaves);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(pastaChaves))
                .SetApplicationName("CircuitoTereVerde.Tests");
        });
        builder.ConfigureAppConfiguration((_, configuracao) =>
        {
            configuracao.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BootstrapAdmin:Name"] = "Administrador de teste",
                ["BootstrapAdmin:Email"] = EmailAdministrador,
                ["BootstrapAdmin:Password"] = SenhaAdministrador
            });
        });
    }
}

// Mantém as rotas sequenciais, pois todos os testes usam o mesmo banco fixo.
public sealed class ApiTesteFixture : IAsyncLifetime
{
    private readonly List<int> _postagensCriadas = [];
    private bool _apiIniciada;

    public ApiTesteFactory Factory { get; } = new();

    public async Task InitializeAsync()
    {
        using var cliente = Factory.CreateClient();
        _apiIniciada = true;
        using var resposta = await cliente.GetAsync("/api/parques");
        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"A API de teste respondeu {(int)resposta.StatusCode}: {detalhe}");
        }
    }

    public void RegistrarPostagemCriada(int id)
    {
        _postagensCriadas.Add(id);
    }

    public async Task DisposeAsync()
    {
        if (_apiIniciada)
        {
            await using var escopo = Factory.Services.CreateAsyncScope();
            var contexto = escopo.ServiceProvider.GetRequiredService<CircuitoTereVerdeDbContext>();
            foreach (var id in _postagensCriadas)
            {
                var postagem = await contexto.Postagens.FirstOrDefaultAsync(item => item.Id == id);
                if (postagem is not null)
                {
                    contexto.Postagens.Remove(postagem);
                }
            }

            await contexto.SaveChangesAsync();

            var gerenciadorUsuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var administrador = await gerenciadorUsuarios.FindByEmailAsync(Factory.EmailAdministrador);
            if (administrador is not null)
            {
                var resultado = await gerenciadorUsuarios.DeleteAsync(administrador);
                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException("Não foi possível remover o administrador criado pelo teste.");
                }
            }
        }

        Factory.Dispose();
    }
}

[CollectionDefinition(ApiTesteCollection.Nome, DisableParallelization = true)]
public sealed class ApiTesteCollection : ICollectionFixture<ApiTesteFixture>
{
    public const string Nome = "API com SQLite de desenvolvimento";
}
