using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Models;

[Table("Postagens")]
[Index(nameof(ParqueId), nameof(TipoPostagemId), nameof(DataInicio), nameof(DataFim))]
public sealed class Postagem : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override int Id { get; set; }

    [Required]
    public int ParqueId { get; set; }

    [ForeignKey(nameof(ParqueId))]
    public Parque Parque { get; set; } = null!;

    [Required]
    public int TipoPostagemId { get; set; }

    [ForeignKey(nameof(TipoPostagemId))]
    public TipoPostagem TipoPostagem { get; set; } = null!;

    [Required]
    public int FotoPostagemId { get; set; }

    [ForeignKey(nameof(FotoPostagemId))]
    public FotoPostagem FotoPostagem { get; set; } = null!;

    [Required]
    [MaxLength(160)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset DataPublicacao { get; set; }

    [Required]
    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    [Required]
    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataAtualizacao { get; set; }

    [Required]
    [MaxLength(450)]
    public string CriadoPorId { get; set; } = string.Empty;
}
