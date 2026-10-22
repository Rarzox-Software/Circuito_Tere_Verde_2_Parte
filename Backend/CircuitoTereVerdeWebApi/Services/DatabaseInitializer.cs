using CircuitoTereVerde.Core.Infra.Context;
using CircuitoTereVerde.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerdeWebApi.Services;

public sealed class DatabaseInitializer
{
    private readonly CircuitoTereVerdeDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public DatabaseInitializer(
        CircuitoTereVerdeDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _environment = environment;
    }

    #region Preparação da base

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var migracoesPendentes = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        var deveCriarPostagensDesenvolvimento = _environment.IsDevelopment()
            && migracoesPendentes.Any(item => item.EndsWith("_DadosDesenvolvimento", StringComparison.Ordinal));

        // Cria a base com a Inicial ou aplica apenas as migrações que ainda não foram executadas.
        await _context.Database.MigrateAsync(cancellationToken);

        foreach (var nomePerfil in new[] { "USUARIO", "ADMINISTRADOR" })
        {
            if (!await _roleManager.RoleExistsAsync(nomePerfil))
            {
                var resultado = await _roleManager.CreateAsync(new IdentityRole(nomePerfil));
                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException("Não foi possível preparar os perfis de acesso.");
                }
            }
        }

        // O Identity cria o administrador com a senha do appsettings depois de preparar as tabelas.
        var administrador = await CriarAdministradorInicialAsync();

        // Os exemplos são incluídos uma única vez, logo após a migração de desenvolvimento.
        if (deveCriarPostagensDesenvolvimento && administrador is not null)
        {
            await CriarPostagensDesenvolvimentoAsync(administrador, cancellationToken);
        }
    }

    private async Task<ApplicationUser?> CriarAdministradorInicialAsync()
    {
        var email = _configuration["BootstrapAdmin:Email"]?.Trim();
        var senha = _configuration["BootstrapAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(senha))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException("Configure o e-mail e a senha do administrador inicial.");
        }

        var administrador = await _userManager.FindByEmailAsync(email);
        if (administrador is null)
        {
            administrador = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Nome = _configuration["BootstrapAdmin:Name"] ?? "Administrador",
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(administrador, senha);
            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException("Não foi possível criar o administrador inicial. Confira a senha configurada.");
            }
        }

        if (!await _userManager.IsInRoleAsync(administrador, "ADMINISTRADOR"))
        {
            var resultado = await _userManager.AddToRoleAsync(administrador, "ADMINISTRADOR");
            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException("Não foi possível atribuir o perfil de administrador.");
            }
        }

        return administrador;
    }

    private async Task CriarPostagensDesenvolvimentoAsync(
        ApplicationUser administrador,
        CancellationToken cancellationToken)
    {
        if (await _context.Postagens.AnyAsync(cancellationToken))
        {
            return;
        }

        var parques = await _context.Parques.ToDictionaryAsync(item => item.Codigo, cancellationToken);
        var tipos = await _context.TiposPostagem.ToDictionaryAsync(item => item.Codigo, cancellationToken);
        var pastaImagens = Path.Combine(_environment.ContentRootPath, "Assets", "PostagensDesenvolvimento");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var agora = DateTimeOffset.UtcNow;

        foreach (var dados in DadosPostagensDesenvolvimento.ObterPostagens())
        {
            var caminhoImagem = Path.Combine(pastaImagens, dados.ArquivoFoto);
            if (!File.Exists(caminhoImagem))
            {
                throw new FileNotFoundException("Não foi possível localizar uma imagem dos dados de desenvolvimento.", caminhoImagem);
            }

            var conteudoFoto = await File.ReadAllBytesAsync(caminhoImagem, cancellationToken);
            _context.Postagens.Add(new Postagem
            {
                ParqueId = parques[dados.CodigoParque].Id,
                TipoPostagemId = tipos[dados.CodigoTipo].Id,
                FotoPostagem = new FotoPostagem
                {
                    Conteudo = conteudoFoto,
                    TipoConteudo = dados.TipoConteudoFoto
                },
                Titulo = dados.Titulo,
                Descricao = dados.Descricao,
                DataPublicacao = agora.AddDays(dados.DiasDesdeInicio),
                DataInicio = hoje.AddDays(dados.DiasDesdeInicio),
                DataCriacao = agora,
                CriadoPorId = administrador.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
