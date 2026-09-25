using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Models;

[Table("Parques")]
[Index(nameof(Codigo), IsUnique = true)]
public sealed class Parque : IEntity
{
    [Key]
    public override int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Nome { get; set; } = string.Empty;
}
