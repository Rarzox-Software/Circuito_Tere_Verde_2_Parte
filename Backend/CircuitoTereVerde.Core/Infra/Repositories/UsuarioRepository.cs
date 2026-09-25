using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Repositories;

// Repositório dos usuários gerenciados pelo Identity.
public sealed class UsuarioRepository : GenericRepository<ApplicationUser>
{
    public UsuarioRepository(DbContext context)
        : base(context)
    {
    }
}
