using HexMaster.UrlShortner.CosmosDb;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using HexMaster.UrlShortner.ShortLinks.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HexMaster.UrlShortner.ShortLinks.Configuration;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection  AddShortLinks(this IServiceCollection services)
    {
        services.AddScoped<IShortLinksService, ShortLinksService>();
        services.AddScoped<IShortLinksRepository, ShortLinksTableRepository>();
        return services;
    }
}