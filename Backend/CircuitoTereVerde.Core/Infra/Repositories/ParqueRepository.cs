using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

// Repositório do catálogo de parques.
public sealed class ParqueRepository : GenericRepository<Parque>
{
    public ParqueRepository(DbContext context)
        : base(context)
    {
    }
}
