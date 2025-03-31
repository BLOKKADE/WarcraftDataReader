namespace WDR.Mergers.Helpers;
public static class MetaDataHelper
{
    private static readonly List<string> s_typeMappings =
    [
        "buff",
        "ability",
        "item",
        "unit",
        "destructable",
        "misc",
        "upgrade"
    ];

    public static string GetMetaDataType(string type)
    {
        return s_typeMappings.First(x => type.Contains(x, StringComparison.CurrentCultureIgnoreCase));
    }
}
