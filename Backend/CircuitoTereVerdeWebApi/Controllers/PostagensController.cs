using CircuitoTereVerde.Core.Services;
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
        [FromHeader] string? parque,
        [FromHeader] string? tipo,
        [FromHeader] int pagina = 1,
        [FromHeader] int tamanhoPagina = 12,
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

    [HttpGet("detalhe")]
    public async Task<IActionResult> Obter(
        [FromHeader] int id,
        CancellationToken cancellationToken)
    {
        var postagem = await _postagemService.ObterPublicaAsync(id, cancellationToken);
        return Ok(postagem);
    }

    #endregion
}
