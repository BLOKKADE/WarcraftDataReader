using Warcraft.Models;

namespace Warcraft.Mappers.Sylk;

public static class SylkMapper
{
    public static List<MappedObject> Map(Sylk sylk)
    {
        List<MappedObject> mappedObjects = [];
        Dictionary<int, string> fieldNames = [];

        foreach (var keyValuePair in sylk.Values)
        {
            var point = keyValuePair.Key;
            var value = keyValuePair.Value;

            if (point.Y == 1)
            {
                // Store field names from the first row (Y1)
                fieldNames[point.X] = value;
            }
            else
            {
                // Skip if fieldNames doesn't contain the key
                if (!fieldNames.ContainsKey(point.X))
                {
                    continue;
                }

                // Create or get the MappedObj for the current row (Y2+)
                MappedObject obj;
                if (mappedObjects.Count < point.Y - 1)
                {
                    obj = new MappedObject() { Code = value };
                    mappedObjects.Add(obj);
                }
                else
                {
                    obj = mappedObjects[point.Y - 2];

                    // Map the X values to the corresponding fields
                    obj.Fields[fieldNames[point.X]] = value;
                }
            }
        }

        return mappedObjects;
    }
}
