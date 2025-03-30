using System.Text.Json;
using Warcraft.Objects;

namespace Warcraft.Mapper;

public static class WarcraftObjectMapper
{
    public static Warcraft3Object MapFromJson(string dataJsonPath, string metaJsonPath)
    {
        var tempJson = File.ReadAllText(dataJsonPath);
        var metaJson = File.ReadAllText(metaJsonPath);

        var tempObjects = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(tempJson)!;
        var metaData = JsonSerializer.Deserialize<List<MetaData>>(metaJson)!;

        var dataName = Path.GetFileNameWithoutExtension(dataJsonPath);

        return MapJsonToWarcraft3Object(dataName, tempObjects, metaData);
    }

    public static Warcraft3Object MapJsonToWarcraft3Object(string source, List<Dictionary<string, string>> objectData, List<MetaData> metaData)
    {
        // Create a dictionary to group metadata by field
        // there can be duplicates in abilitymetadata
        var metaFieldDict = metaData
            .Where(m => string.Equals(m.Slk, source, StringComparison.OrdinalIgnoreCase))
            .GroupBy(m => m.Field)
            .ToDictionary(g => g.Key, g => g.ToList());

        Warcraft3Object warcraft3Object = new()
        {
            Original = []
        };

        // Get the source identifier based on the source type
        var sourceIdentifier = GetSourceIdentifier(source);

        // Iterate through each object in the JSON data
        foreach (var tempObject in objectData)
        {
            // Skip to the next object if the source identifier is not present
            // should not happen!!!
            if (!tempObject.ContainsKey(sourceIdentifier))
            {
                continue;
            }

            var code = tempObject[sourceIdentifier];
            List<Warcraft3Field> fields = [];

            foreach (var kvp in tempObject)
            {
                var fieldName = kvp.Key;
                var fieldValue = kvp.Value;

                // Check if the field name exists in the metadata dictionary
                // If not, try removing trailing numbers from the field name
                // e.g., "Field1" -> "Field", "Field12" -> "Field"
                if (metaFieldDict.TryGetValue(fieldName, out var metaFields) ||
                    metaFieldDict.TryGetValue(RemoveTrailingNumbers(fieldName), out metaFields))
                {
                    // Determine the level of the field
                    var level = fieldName.Length > RemoveTrailingNumbers(fieldName).Length ? int.Parse(fieldName[RemoveTrailingNumbers(fieldName).Length..]) : 0;

                    MetaData? metaField = null;

                    // Special handling for ability data
                    if (source.Equals("abilitydata", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // if metaFields explicitly contains the ability ID, use that
                        metaFields = [.. metaFields.OrderByDescending(m => m.AbilityId.Count)];
                        metaField = metaFields.Find(m => m.AbilityId.Contains(code));

                        // if no ability ID was found, use the last field in the list
                        // last field usually contains a general code used by most abilities
                        if (metaField == null && metaFields[^1].AbilityId.Count == 0)
                        {
                            metaField = metaFields[^1];
                        }

                        if (metaField == null)
                        {
                            continue;
                        }
                    }

                    // Use the first metadata field if none was found
                    metaField ??= metaFields[0];

                    Warcraft3Field warcraft3Field = new()
                    {
                        Id = metaField.Code,
                        Type = metaField.Type,
                        Value = fieldValue,
                        Column = metaField.Column,
                        Level = level,
                        FieldName = fieldName
                    };
                    fields.Add(warcraft3Field);
                }
            }

            warcraft3Object.Original[code] = fields;
        }

        return warcraft3Object;
    }

    // Removes trailing numbers from a field name
    // e.g., "Field1" -> "Field", "Field12" -> "Field"
    private static string RemoveTrailingNumbers(string fieldName)
    {
        var i = fieldName.Length - 1;
        while (i >= 0 && char.IsDigit(fieldName[i]))
        {
            i--;
        }
        return fieldName[..(i + 1)];
    }

    private static string GetSourceIdentifier(string source)
    {
        return source.ToLower() switch
        {
            "abilitydata" => "alias",
            "unitui" => "unitUIID",
            "unitweapons" => "unitWeaponID",
            "itemdata" => "itemID",
            "unitabilities" => "unitAbilID",
            "unitbalance" => "unitBalanceID",
            "unitdata" => "unitID",
            _ => throw new ArgumentException("Invalid source identifier.")
        };
    }
}
