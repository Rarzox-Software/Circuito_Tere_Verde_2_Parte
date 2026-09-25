using CircuitoTereVerde.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CircuitoTereVerdeWebApi.Controllers;

[ApiController]
[Route("api/postagens")]
public sealed class PostagensController : ControllerBase
{
    private readonly PostagemService _postagemService;

    public PostagensController(PostagemService postagemService)
    {
        _postagemService = postagemService;
    }

    #region Consultas públicas

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? parque,
        [FromQuery] string? tipo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 12,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _postagemService.ListarPublicasAsync(parque, tipo, pagina, tamanhoPagina, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("destaques")]
    public async Task<IActionResult> ListarDestaques(CancellationToken cancellationToken)
    {
        var postagens = await _postagemService.ListarDestaquesAsync(cancellationToken);
        return Ok(postagens);
    }

    [HttpGet("{id:int}/foto")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterFoto(int id, CancellationToken cancellationToken)
    {
        var administrador = User.IsInRole("ADMINISTRADOR");
        var foto = await _postagemService.ObterFotoAsync(id, administrador, cancellationToken);
        return File(foto.Conteudo, foto.TipoConteudo);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken cancellationToken)
    {
        var postagem = await _postagemService.ObterPublicaAsync(id, cancellationToken);
        return Ok(postagem);
    }

    #endregion
}
