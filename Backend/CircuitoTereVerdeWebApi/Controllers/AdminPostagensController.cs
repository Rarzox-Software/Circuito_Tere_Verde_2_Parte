using System.Security.Claims;
using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerdeWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CircuitoTereVerdeWebApi.Controllers;

[ApiController]
[Authorize(Roles = "ADMINISTRADOR")]
[Route("api/admin/postagens")]
public sealed class AdminPostagensController : ControllerBase
{
    private readonly AdminPostagensService _adminPostagensService;

    public AdminPostagensController(AdminPostagensService adminPostagensService)
    {
        _adminPostagensService = adminPostagensService;
    }

    #region Consultas administrativas

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromHeader] int pagina = 1,
        [FromHeader] int tamanhoPagina = 12,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _adminPostagensService.ListarAsync(pagina, tamanhoPagina, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("selecionada")]
    public async Task<IActionResult> Obter(
        [FromHeader] int id,
        CancellationToken cancellationToken)
    {
        var postagem = await _adminPostagensService.ObterAsync(id, cancellationToken);
        return Ok(postagem);
    }

    #endregion

    #region Cadastro e edição

    [HttpPost]
    // A foto chega em Base64, então a requisição comporta até 5 MB após a decodificação.
    [RequestSizeLimit(7_100_000)]
    public async Task<IActionResult> Criar([FromBody] SalvarPostagemDto dados, CancellationToken cancellationToken)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var postagem = await _adminPostagensService.CriarAsync(dados, usuarioId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, postagem);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5_300_000)]
    public async Task<IActionResult> Atualizar(
        [FromHeader] int id,
        [FromForm] SalvarPostagemDto dados,
        IFormFile? foto,
        CancellationToken cancellationToken)
    {
        var postagem = await _adminPostagensService.AtualizarAsync(id, dados, foto, cancellationToken);
        return Ok(postagem);
    }

    [HttpDelete]
    public async Task<IActionResult> Excluir(
        [FromHeader] int id,
        CancellationToken cancellationToken)
    {
        await _adminPostagensService.ExcluirAsync(id, cancellationToken);
        return NoContent();
    }

    #endregion

}
