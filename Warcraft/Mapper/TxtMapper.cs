namespace Warcraft.Mapper;
public static class TxtMapper
{
    public static Dictionary<string, Dictionary<string, string>> ReadTextExample(string filePath)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        var currentCode = string.Empty;

        foreach (var line in File.ReadLines(filePath))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
            {
                continue;
            }

            if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
            {
                currentCode = trimmedLine.Trim('[', ']');
                if (!result.ContainsKey(currentCode))
                {
                    result[currentCode] = [];
                }
            }
            else if (!string.IsNullOrEmpty(currentCode))
            {
                var parts = trimmedLine.Split('=');
                if (parts.Length == 2)
                {
                    result[currentCode][parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        return result;
    }
}
