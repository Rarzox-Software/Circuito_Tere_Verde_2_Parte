using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

public class GenericRepository<T> where T : class
{
    private readonly DbSet<T> _conjunto;

    public GenericRepository(DbContext context)
    {
        _conjunto = context.Set<T>();
    }

    #region Consultas e paginação

    // A consulta não rastreia alterações, a menos que a edição solicite o contrário.
    public IQueryable<T> Consultar(bool rastrearAlteracoes = false)
    {
        var consulta = _conjunto.AsQueryable();
        if (!rastrearAlteracoes)
        {
            consulta = consulta.AsNoTracking();
        }

        return consulta;
    }

    public async Task<List<T>> ListarAsync(IQueryable<T> consulta, CancellationToken cancellationToken = default)
    {
        return await consulta.ToListAsync(cancellationToken);
    }

    public async Task<T?> PrimeiroOuPadraoAsync(IQueryable<T> consulta, CancellationToken cancellationToken = default)
    {
        return await consulta.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> ContarAsync(IQueryable<T> consulta, CancellationToken cancellationToken = default)
    {
        return await consulta.CountAsync(cancellationToken);
    }

    public async Task<(List<T> Itens, int Total)> PaginarAsync(
        IQueryable<T> consulta,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        pagina = Math.Max(1, pagina);
        tamanhoPagina = Math.Clamp(tamanhoPagina, 1, 50);
        var total = await ContarAsync(consulta, cancellationToken);
        var itens = await ListarAsync(consulta.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina), cancellationToken);
        return (itens, total);
    }

    #endregion

    #region Operações básicas

    public async Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        await _conjunto.AddAsync(entidade, cancellationToken);
    }

    public void Atualizar(T entidade)
    {
        _conjunto.Update(entidade);
    }

    public void Excluir(T entidade)
    {
        _conjunto.Remove(entidade);
    }

    #endregion
}
