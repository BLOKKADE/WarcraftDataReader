using Warcraft.Models;

namespace Warcraft.Mappers;

public static class WarcraftObjectMapper
{
    public static Wc3Data MapObjectsToWarcraftObject(string source, List<MappedObject> objectData, List<MetaData> metaData)
    {
        // Create a dictionary to group metadata by field
        // there can be duplicates in abilitymetadata
        var metaFieldDict = metaData
            .Where(m => string.Equals(m.Slk, source, StringComparison.OrdinalIgnoreCase))
            .GroupBy(m => m.Field)
            .ToDictionary(g => g.Key, g => g.ToList());

        Wc3Data warcraft3Object = new();

        foreach (var tempObject in objectData)
        {
            var wc3Obj = new Wc3Object() { Code = tempObject.Code, OriginalCode = tempObject.Code };

            foreach (var kvp in tempObject.Fields)
            {
                var fieldName = kvp.Key;
                var fieldValue = kvp.Value;

                // Check if the field name exists in the metadata dictionary
                // If not, try removing trailing numbers from the field name
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
                        metaField = metaFields.Find(m => m.AbilityId.Contains(wc3Obj.Code));

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
                    wc3Obj.Fields.Add(warcraft3Field);
                }
            }

            warcraft3Object.Original.Add(wc3Obj);
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
}
