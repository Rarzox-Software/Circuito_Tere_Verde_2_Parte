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

    public DatabaseInitializer(
        CircuitoTereVerdeDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    #region Preparação da base

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
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
        await CriarAdministradorInicialAsync();
    }

    private async Task CriarAdministradorInicialAsync()
    {
        var email = _configuration["BootstrapAdmin:Email"]?.Trim();
        var senha = _configuration["BootstrapAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(senha))
        {
            return;
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
    }

    #endregion
}
