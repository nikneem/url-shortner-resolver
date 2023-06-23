using HexMaster.UrlShortner.Metrics.Enums;

namespace HexMaster.UrlShortner.Metrics.DataTransferObjects;

public record CumulatedMetricsResult(DateTimeOffset startOn, DateTimeOffset endOn, Cumulation Cumulation, int Sum)
{
    
}