using Microsoft.AspNetCore.Identity;

namespace CircuitoTereVerde.Core.Models;

public sealed class ApplicationUser : IdentityUser
{
    public string Nome { get; set; } = string.Empty;

    public ICollection<Postagem> PostagensCriadas { get; set; } = [];
}
