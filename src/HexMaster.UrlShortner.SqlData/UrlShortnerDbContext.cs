using HexMaster.UrlShortner.SqlData.Entities;
using Microsoft.EntityFrameworkCore;

namespace HexMaster.UrlShortner.SqlData;

public class UrlShortnerDbContext : DbContext
{
    public UrlShortnerDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    public UrlShortnerDbContext()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<OwnerEntity> Owners { get; set; }
    public DbSet<ShortLinkEntity> ShortLinks { get; set; }

}
