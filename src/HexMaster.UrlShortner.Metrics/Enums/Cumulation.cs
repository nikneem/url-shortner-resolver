namespace HexMaster.UrlShortner.Metrics.Enums;

public abstract class Cumulation
{

    public static readonly Cumulation Hourly = new CumulationHourly();
    
    public abstract TimeSpan MaxTimespan { get; }
    public abstract string Key { get; }

}


public class CumulationHourly : Cumulation
{
    public override TimeSpan MaxTimespan => TimeSpan.FromHours(24);
    public override string Key => CumulationKey.Hourly;
}
public class CumulationDaily : Cumulation
{
    public override TimeSpan MaxTimespan => TimeSpan.FromDays(180);
    public override string Key => CumulationKey.Daily;
}

