using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CircuitoTereVerde.Core.Models;

[Table("FotosPostagem")]
public sealed class FotoPostagem : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override int Id { get; set; }

    [Required]
    public byte[] Conteudo { get; set; } = [];

    [Required]
    [MaxLength(80)]
    public string TipoConteudo { get; set; } = string.Empty;
}
