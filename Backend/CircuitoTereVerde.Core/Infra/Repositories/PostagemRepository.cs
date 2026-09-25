using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

// Repositório das postagens.
public sealed class PostagemRepository : GenericRepository<Postagem>
{
    public PostagemRepository(DbContext context)
        : base(context)
    {
    }
}
