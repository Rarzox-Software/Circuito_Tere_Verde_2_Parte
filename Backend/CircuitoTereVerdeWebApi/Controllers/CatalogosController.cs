using CircuitoTereVerde.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CircuitoTereVerdeWebApi.Controllers;

[ApiController]
[Route("api")]
public sealed class CatalogosController : ControllerBase
{
    private readonly CatalogoService _catalogoService;

    public CatalogosController(CatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
    }

    #region Catálogos públicos

    [HttpGet("parques")]
    public async Task<IActionResult> ListarParques(CancellationToken cancellationToken)
    {
        var parques = await _catalogoService.ListarParquesAsync(cancellationToken);
        return Ok(parques);
    }

    [HttpGet("tipos-postagem")]
    public async Task<IActionResult> ListarTipos(CancellationToken cancellationToken)
    {
        var tipos = await _catalogoService.ListarTiposAsync(cancellationToken);
        return Ok(tipos);
    }

    #endregion
}
