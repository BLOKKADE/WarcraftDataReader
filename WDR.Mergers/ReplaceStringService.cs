using System.Text.RegularExpressions;
using WDR.Mappers.Models;

namespace WDR.Mergers;
public static partial class ReplaceStrings
{
    public static void ReplaceString(Wc3Data wc3Data, Dictionary<int, string> mappedStrings)
    {
        ReplaceStringInFields(wc3Data.Original, mappedStrings);
        ReplaceStringInFields(wc3Data.Custom, mappedStrings);
    }

    private static void ReplaceStringInFields(List<Wc3Object> objects, Dictionary<int, string> mappedStrings)
    {
        foreach (var obj in objects)
        {
            foreach (var field in obj.Fields)
            {
                if (field.Type is "string" && !string.IsNullOrEmpty(field.Value))
                {
                    field.Value = ReplaceString(field.Value, mappedStrings);
                }
            }
        }
    }

    private static string ReplaceString(string str, Dictionary<int, string> mappedStrings)
    {
        return TrigstrRegex().Replace(str, match =>
        {
            if (int.TryParse(match.Groups[1].Value, out var id) &&
                mappedStrings.TryGetValue(id, out var value))
            {
                return value;
            }
            return match.Value;
        });
    }

    [GeneratedRegex(@"TRIGSTR_(\d+)")]
    private static partial Regex TrigstrRegex();
}
