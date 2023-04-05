using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HexMaster.UrlShortner.SqlData.Entities;

[Table("Owners")]
public class OwnerEntity
{
    [Key] public Guid Id { get; set; }
    [Required][MaxLength(12)] public string SubjectId { get; set; }
    [Required][MaxLength(500)] public string DisplayName { get; set; }

    public List<ShortLinkEntity> ShortLinks { get; set; }
}
