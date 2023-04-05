namespace HexMaster.UrlShortner.Core.ExtensionMethods;

public static  class IntegerExtensions
{

    public static int CalculatePages(this int totalRecords, int pageSize)
    {
        return (totalRecords + pageSize - 1) / pageSize;
    }

}
