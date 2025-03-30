using System.Dynamic;

namespace SylkReader;

public static class DynamicSylkMapper
{
    public static List<dynamic> MapToDynamicObjects(Sylk sylk)
    {
        List<dynamic> dynamicObjects = [];
        Dictionary<int, string> fieldNames = [];

        foreach (var keyValuePair in sylk.values)
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

                // Create or get the dynamic object for the current row (Y2+)
                dynamic obj;
                if (dynamicObjects.Count < point.Y - 1)
                {
                    obj = new ExpandoObject();
                    dynamicObjects.Add(obj);
                }
                else
                {
                    obj = dynamicObjects[point.Y - 2];
                }

                // Map the X values to the corresponding fields
                var dynamicObj = (IDictionary<string, object>)obj;
                dynamicObj[fieldNames[point.X]] = value;
            }
        }

        return dynamicObjects;
    }
}
