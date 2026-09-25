using System.Security.Claims;
using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerdeWebApi.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CircuitoTereVerdeWebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AutenticacaoController : ControllerBase
{
    private readonly IdentityAutenticacaoService _autenticacaoService;
    private readonly IAntiforgery _antiforgery;

    public AutenticacaoController(
        IdentityAutenticacaoService autenticacaoService,
        IAntiforgery antiforgery)
    {
        _autenticacaoService = autenticacaoService;
        _antiforgery = antiforgery;
    }

    #region Sessão e proteção CSRF

    [HttpGet("csrf")]
    [AllowAnonymous]
    public IActionResult ObterTokenCsrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrar([FromBody] LoginDto dados)
    {
        var usuario = await _autenticacaoService.EntrarAsync(dados);
        if (usuario is null)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        return Ok(usuario);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> ObterAtual()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usuario = await _autenticacaoService.ObterAtualAsync(usuarioId);
        if (usuario is null)
        {
            return Unauthorized();
        }

        return Ok(usuario);
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await _autenticacaoService.SairAsync();
        return NoContent();
    }

    #endregion
}

