using CircuitoTereVerde.Core.Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
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
    private readonly string _ambiente;

    public ApiTesteFactory(string? ambiente = null)
    {
        _ambiente = ambiente ?? Environments.Development;
        // Usa HTTPS para que o cliente de teste envie o cookie Secure da API.
        ClientOptions.BaseAddress = new Uri("https://localhost");
    }

    public string EmailAdministrador
    {
        get
        {
            return Services.GetRequiredService<IConfiguration>()["BootstrapAdmin:Email"]
                ?? throw new InvalidOperationException("Configure o e-mail de teste no appsettings.Development.");
        }
    }

    public string SenhaAdministrador
    {
        get
        {
            return Services.GetRequiredService<IConfiguration>()["BootstrapAdmin:Password"]
                ?? throw new InvalidOperationException("Configure a senha de teste no appsettings.Development.");
        }
    }

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

        builder.UseEnvironment(_ambiente);
        builder.UseContentRoot(raizApi);
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureAppConfiguration((_, configuracao) =>
        {
            // O cenário de produção usa o mesmo SQLite de desenvolvimento, com as credenciais de teste.
            if (_ambiente == Environments.Production)
            {
                configuracao.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Data Source=../data/circuito-tereverde-dev.db",
                    ["BootstrapAdmin:Email"] = "admin@circuitotereverde.test",
                    ["BootstrapAdmin:Password"] = "AdminTeste123!"
                });
            }
        });
        builder.ConfigureServices(services =>
        {
            var pastaChaves = Path.Combine(diretorio.FullName, "build-codex", "data-protection-keys");
            Directory.CreateDirectory(pastaChaves);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(pastaChaves))
                .SetApplicationName("CircuitoTereVerde.Tests");
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
        // Iniciar a API executa o DatabaseInitializer, que cria a base se ela ainda não existir.
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
        // Mantém o banco e o administrador; limpa apenas as postagens e fotos criadas pelos testes.
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
                    contexto.FotosPostagem.Remove(postagem.FotoPostagem);
                }
            }

            await contexto.SaveChangesAsync();
        }

        Factory.Dispose();
    }
}

[CollectionDefinition(ApiTesteCollection.Nome, DisableParallelization = true)]
public sealed class ApiTesteCollection : ICollectionFixture<ApiTesteFixture>
{
    public const string Nome = "API com SQLite de desenvolvimento";
}
