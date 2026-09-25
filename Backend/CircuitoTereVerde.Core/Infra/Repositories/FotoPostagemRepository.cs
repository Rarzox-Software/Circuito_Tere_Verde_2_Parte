using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

// Repositório das imagens salvas junto às postagens.
public sealed class FotoPostagemRepository : GenericRepository<FotoPostagem>
{
    public FotoPostagemRepository(DbContext context)
        : base(context)
    {
    }
}
