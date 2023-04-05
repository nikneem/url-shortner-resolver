using HexMaster.DomainDrivenDesign.ChangeTracking;
using HexMaster.UrlShortner.Core.ExtensionMethods;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using HexMaster.UrlShortner.SqlData;
using HexMaster.UrlShortner.SqlData.Entities;
using Microsoft.EntityFrameworkCore;

namespace HexMaster.UrlShortner.ShortLinks.Repositories;

public  class ShortLinksRepository : IShortLinksRepository
{

    private UrlShortnerDbContext _dbContext;

    public async Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize, int page, CancellationToken cancellationToken)
    {
        var dbQuery = _dbContext.ShortLinks.Where(x => x.OwnerId == ownerId);
        if (!string.IsNullOrWhiteSpace(query))
        {
            dbQuery = dbQuery.Where(ent => ent.ShortCode.Contains(query) || ent.TargetUrl.Contains(query));
        }

        var skip = pageSize * page;
        var totalCount = await dbQuery.CountAsync(cancellationToken);
        var totalPages = totalCount.CalculatePages(pageSize);

        var pageEntities = await dbQuery
            .OrderByDescending(ent => ent.CreatedOn)
            .Skip(skip)
            .Take(pageSize)
            .Select(ent => EntityToListItemProjection(ent))
            .ToListAsync(cancellationToken);

        return new ShortLinksListDto
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            ShortLinks = pageEntities
        };

    }
    public Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.ShortLinks
            .Where(x => x.OwnerId == ownerId && x.Id == id)
            .Select(ent => EntityToDetailsProjection(ent))
            .FirstAsync(cancellationToken);
    }
    public async Task<IShortLink> GetDomainModelAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        var domainModel = await  _dbContext.ShortLinks
            .Where(x => x.OwnerId == ownerId && x.Id == id)
            .Select(ent => new ShortLink(ent.Id, ent.ShortCode, ent.TargetUrl, ent.CreatedOn, ent.ExpiresOn))
            .FirstAsync(cancellationToken);
        return domainModel;
    }

    public async Task<bool> UpdateAsync(Guid ownerId, IShortLink domainModel, CancellationToken cancellationToken)
    {
        if (domainModel is ShortLink dm)
        {
            if (dm.TrackingState == TrackingState.New)
            {
                // Create new entity and add it to the database
                var entity = ToEntity(ownerId, dm);
                await _dbContext.AddAsync(entity, cancellationToken);
                var affected = await _dbContext.SaveChangesAsync(cancellationToken);
                return affected > 0;
            }
            if (dm.TrackingState == TrackingState.Modified)
            {
                // Fetch existing entity and update it in the database
                var databaseEntity = await _dbContext.ShortLinks.FirstOrDefaultAsync(ent => ent.OwnerId == ownerId && ent.Id == dm.Id);
                var newEntity = ToEntity(ownerId, dm, databaseEntity);
                _dbContext.Entry(newEntity).State = EntityState.Modified;
                var affected = await _dbContext.SaveChangesAsync(cancellationToken);
                return affected > 0;
            }
        }
        return false;
    }

    private static ShortLinkEntity ToEntity(Guid ownerId, ShortLink domainModel, ShortLinkEntity? existing = null)
    {
        var entity = existing ?? new ShortLinkEntity
        {
            Id = domainModel.Id,
            OwnerId = ownerId,
            CreatedOn = domainModel.CreatedOn
        };
        entity.ShortCode = domainModel.ShortCode;
        entity.TargetUrl = domainModel.TargetUrl;
        entity.ExpiresOn = domainModel.ExpiresOn;
        return entity;
    }

    private static ShortLinksListItemDto EntityToListItemProjection(ShortLinkEntity entity)
    {
        return new ShortLinksListItemDto
        {
            Id = entity.Id,
            ShortCode = entity.ShortCode,
            TargetUrl = entity.TargetUrl,
            CreatedOn = entity.CreatedOn,
            ExpiresOn = entity.ExpiresOn
        };
    }
    private static ShortLinkDetailsDto EntityToDetailsProjection(ShortLinkEntity entity)
    {
        return new ShortLinkDetailsDto
        {
            Id = entity.Id,
            ShortCode = entity.ShortCode,
            TargetUrl = entity.TargetUrl,
            CreatedOn = entity.CreatedOn,
            ExpiresOn = entity.ExpiresOn
        };
    }

}
