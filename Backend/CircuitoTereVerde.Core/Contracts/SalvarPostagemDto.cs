using System.ComponentModel.DataAnnotations;

namespace CircuitoTereVerde.Core.Contracts;

public sealed class SalvarPostagemDto
{
    [Required]
    public string Parque { get; set; } = string.Empty;

    [Required]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [StringLength(160, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(5000, MinimumLength = 1)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateOnly? DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }
    public string Foto { get; set; } = string.Empty;
}

