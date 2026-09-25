using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerde.Core.Models;
using CircuitoTereVerde.Core.Infra.UnitOfWork;

namespace CircuitoTereVerde.Core.Services;

public sealed class CatalogoService
{
    private readonly UnitOfWork _uow;

    public CatalogoService(UnitOfWork uow)
    {
        _uow = uow;
    }

    #region Consultas dos catálogos

    public async Task<IReadOnlyList<CatalogoDto>> ListarParquesAsync(CancellationToken cancellationToken = default)
    {
        var repositorio = _uow.ParqueRepository;
        var parques = await repositorio.ListarAsync(repositorio.Consultar().OrderBy(parque => parque.Nome), cancellationToken);

        return parques.Select(parque => new CatalogoDto(parque.Id, parque.Codigo, parque.Nome)).ToList();
    }

    public async Task<IReadOnlyList<CatalogoDto>> ListarTiposAsync(CancellationToken cancellationToken = default)
    {
        var repositorio = _uow.TipoPostagemRepository;
        var tipos = await repositorio.ListarAsync(repositorio.Consultar().OrderBy(tipo => tipo.Id), cancellationToken);

        return tipos.Select(tipo => new CatalogoDto(tipo.Id, tipo.Codigo, tipo.Nome)).ToList();
    }
    #endregion
}



