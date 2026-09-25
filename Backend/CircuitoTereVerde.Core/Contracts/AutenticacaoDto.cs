using System.ComponentModel.DataAnnotations;

namespace CircuitoTereVerde.Core.Contracts;

public sealed class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}

public sealed record UsuarioAutenticadoDto(
    string Id,
    string Nome,
    string Email,
    IReadOnlyList<string> Perfis);

