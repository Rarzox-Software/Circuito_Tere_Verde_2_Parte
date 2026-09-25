using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

// Repositório do catálogo de tipos de postagem.
public sealed class TipoPostagemRepository : GenericRepository<TipoPostagem>
{
    public TipoPostagemRepository(DbContext context)
        : base(context)
    {
    }
}
