using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Models;

[Table("TiposPostagem")]
[Index(nameof(Codigo), IsUnique = true)]
public sealed class TipoPostagem : IEntity
{
    [Key]
    public override int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string Nome { get; set; } = string.Empty;
}
