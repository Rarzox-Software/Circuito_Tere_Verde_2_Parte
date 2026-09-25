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
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 12,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _adminPostagensService.ListarAsync(pagina, tamanhoPagina, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken cancellationToken)
    {
        var postagem = await _adminPostagensService.ObterAsync(id, cancellationToken);
        return Ok(postagem);
    }

    #endregion

    #region Cadastro e edição

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(5_300_000)]
    public async Task<IActionResult> Criar(
        [FromForm] SalvarPostagemDto dados,
        [FromForm(Name = "foto")] IFormFile? arquivoFoto,
        CancellationToken cancellationToken)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var postagem = await _adminPostagensService.CriarAsync(dados, arquivoFoto, usuarioId, cancellationToken);
        return CreatedAtAction(nameof(Obter), new { id = postagem.Id }, postagem);
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(5_300_000)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromForm] SalvarPostagemDto dados,
        [FromForm(Name = "foto")] IFormFile? arquivoFoto,
        CancellationToken cancellationToken)
    {
        var postagem = await _adminPostagensService.AtualizarAsync(id, dados, arquivoFoto, cancellationToken);
        return Ok(postagem);
    }

    [HttpDelete("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        await _adminPostagensService.ExcluirAsync(id, cancellationToken);
        return NoContent();
    }

    #endregion

}
