using System.Text.RegularExpressions;

namespace WDR.Mappers.Mappers;
public static partial class WtsMapper
{
    public static Dictionary<int, string> MapFromString(string input)
    {
        var result = new Dictionary<int, string>();
        var matches = StringContentRegex().Matches(input)
            .Select(match => new { Key = int.TryParse(match.Groups[1].Value, out var key) ? key : (int?)null, Value = match.Groups[2].Value.Trim() })
            .Where(x => x.Key.HasValue);

        foreach (var match in matches)
        {
            if (match.Key.HasValue)
            {
                result[match.Key.Value] = match.Value;
            }
        }

        return result;
    }

    [GeneratedRegex(@"STRING (\d+)\s*(?:\/\/[^\n]*\n)?\s*{([^}]*)}", RegexOptions.Singleline)]
    private static partial Regex StringContentRegex();
}
