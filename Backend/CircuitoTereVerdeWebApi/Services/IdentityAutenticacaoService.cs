using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerde.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace CircuitoTereVerdeWebApi.Services;

public sealed class IdentityAutenticacaoService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityAutenticacaoService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #region Login, sessão e perfil

    // Valida a senha pelo Identity e retorna o perfil que controla as rotas administrativas.
    public async Task<UsuarioAutenticadoDto?> EntrarAsync(LoginDto dados)
    {
        var usuario = await _userManager.FindByEmailAsync(dados.Email);
        if (usuario is null)
        {
            return null;
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario, dados.Senha, isPersistent: false, lockoutOnFailure: true);
        if (!resultado.Succeeded)
        {
            return null;
        }

        return await MapearAsync(usuario);
    }

    public async Task<UsuarioAutenticadoDto?> ObterAtualAsync(string? usuarioId)
    {
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return null;
        }

        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
        {
            return null;
        }

        return await MapearAsync(usuario);
    }

    public async Task SairAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private async Task<UsuarioAutenticadoDto> MapearAsync(ApplicationUser usuario)
    {
        var perfis = await _userManager.GetRolesAsync(usuario);
        var nome = string.IsNullOrWhiteSpace(usuario.Nome) ? usuario.Email ?? string.Empty : usuario.Nome;
        return new UsuarioAutenticadoDto(usuario.Id, nome, usuario.Email ?? string.Empty, perfis.ToList());
    }

    #endregion
}

