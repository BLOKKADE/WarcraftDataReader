﻿using WDR.Mappers.Models;

namespace WDR.Mappers.Mappers;
public static class TxtMapper
{
    public static List<MappedObject> MapFromFile(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        return Map(fileContent);
    }

    public static List<MappedObject> Map(string content)
    {
        var result = new List<MappedObject>();
        MappedObject? currentObject = null;

        using (var reader = new StringReader(content))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                {
                    continue;
                }

                if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
                {
                    currentObject = new MappedObject { Code = trimmedLine.Trim('[', ']') };
                    result.Add(currentObject);
                }
                else if (currentObject != null)
                {
                    var parts = trimmedLine.Split('=');
                    if (parts.Length == 2)
                    {
                        currentObject.Fields[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        return result;
    }
}
