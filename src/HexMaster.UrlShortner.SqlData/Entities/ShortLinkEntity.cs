using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HexMaster.UrlShortner.SqlData.Entities;


[Table("ShortLinks")]
public class ShortLinkEntity
{
    [Key] public Guid Id { get; set; }

    [Required] public Guid OwnerId { get; set; } 
    [Required][MaxLength(12)] public string ShortCode { get; set; }
    [Required][MaxLength(500)] public string TargetUrl { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? ExpiresOn { get; set; }

    [ForeignKey(nameof(OwnerId))] public OwnerEntity Owner { get; set; }
}