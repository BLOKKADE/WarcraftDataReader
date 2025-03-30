namespace Infrastructure;
internal class MetaDataPerData
{
    public static string GetMetaData(string key) => key.ToLower() switch
    {
        "abilitydata" => "abilitymetadata",
        "buffdata" => "buffmetadata",
        "destructabledata" => "destructablemetadata",
        "itemdata" => "unitmetadata",

        // Add more mappings as needed
        _ => throw new ArgumentException($"No metadata found for key: {key}")
    };
}
