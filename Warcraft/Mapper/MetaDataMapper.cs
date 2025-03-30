using System.Dynamic;
using Warcraft.Objects;

namespace Warcraft.Mapper;
public static class MetaDataMapper
{
    public static List<MetaData> Map(List<dynamic> data)
    {
        return [.. data.Cast<ExpandoObject>().SelectMany(MapObj)];
    }

    private static List<MetaData> MapObj(ExpandoObject obj)
    {
        IDictionary<string, object> dict = obj!;
        // Retrieve data from the dictionary and cast to string
        if (dict.TryGetValue("field", out var fieldObj) && fieldObj is string dataId &&
            dict.TryGetValue("ID", out var idObj) && idObj is string code &&
            dict.TryGetValue("type", out var typeObj) && typeObj is string type &&
            dict.TryGetValue("slk", out var slkObj) && slkObj is string slk)
        {
            var column = 0;
            // Check if the field is "Data" and retrieve the "data" value from the dictionary
            if (dataId == "Data" && dict.TryGetValue("data", out var dataIdObj) && dataIdObj is string dataIdStr &&
                // Try to parse the "data" value to an integer and ensure it is greater than or equal to 1
                int.TryParse(dataIdStr, out var dataIdInt) && dataIdInt >= 1)
            {
                // Append the corresponding letter (A, B, C, etc.) to the "Data" field based on the integer value
                // Example: If dataIdInt is 1, it becomes DataA
                dataId = $"{dataId}{(char)('A' + dataIdInt - 1)}";
                column = dataIdInt;
            }

            // abilitymetadata has useSpecific field that contains all abilities that have this field
            List<string> abilities = dict.TryGetValue("useSpecific", out var usedAbilities)
                ? [.. ((string)usedAbilities).Split(',')]
                : [];

            return [
                new () {
                    Code = code,
                    Field = dataId,
                    Type = GetType(type),
                    Column = column,
                    AbilityId = abilities,
                    Slk = slk
                }
            ];
        }

        return [];
    }

    private static string GetType(string type)
    {
        return type switch
        {
            "bool" => "int",
            "int" => "int",
            "real" => "real",
            "unreal" => "unreal",
            _ => "string",
        };
    }
}
