using WDR.Mappers.Models;
using WDR.Mergers.Mergers;

namespace WDR.Mergers;

public partial class DefaultWc3DataMerger(Wc3Data targetData) : BaseWc3DataMerger
{
    public Wc3Data Wc3Data { get; } = targetData;

    public void Expand(Wc3Data sourceData)
    {
        ExpandList(Wc3Data.Original, sourceData.Original, x => x.OriginalCode);
        ExpandList(Wc3Data.Custom, sourceData.Original, x => x.OriginalCode);
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^(.*?)(\d+)$")]
    private static partial System.Text.RegularExpressions.Regex FieldNameRegex();

    protected override void Merge(Wc3Object targetObject, Wc3Object sourceObject)
    {
        var targetMaxLevel = int.TryParse(targetObject.Fields.SingleOrDefault(x => x.Id == "alev")?.Value, out var tempTargetMaxLevel) ? tempTargetMaxLevel : 0;
        var sourceMaxLevel = int.TryParse(sourceObject.Fields.SingleOrDefault(x => x.Id == "alev")?.Value, out var tempSourceMaxLevel) ? tempSourceMaxLevel : 0;
        foreach (var sourceFields in sourceObject.Fields)
        {
            if (sourceFields.Level > 0 && sourceFields.Level <= sourceMaxLevel)
            {
                if (targetObject.Fields.SingleOrDefault(x => x.Id == sourceFields.Id && x.Level == sourceFields.Level) is null)
                {
                    targetObject.Fields.Add(sourceFields);
                }
                else
                {
                    var test = 1;
                    test++;
                }
            }
            else if (sourceFields.Level == sourceMaxLevel + 1 && sourceFields.Level <= targetMaxLevel)
            {
                for (var level = (int)sourceFields.Level; level <= targetMaxLevel; level++)
                {
                    if (targetObject.Fields.SingleOrDefault(x => x.Id == sourceFields.Id && x.Level == level) is null)
                    {
                        var newField = new Warcraft3Field
                        {
                            Id = sourceFields.Id,
                            Type = sourceFields.Type,
                            Level = level,
                            Column = sourceFields.Column,
                            Value = sourceFields.Value
                        };
                        targetObject.Fields.Add(newField);
                    }
                }
            }

            foreach (var targetField in targetObject.Fields.Where(x => x.Id == sourceFields.Id))
            {
                if (sourceFields.FieldName != null && targetField.Level != 0)
                {
                    var match = FieldNameRegex().Match(sourceFields.FieldName);
                    if (match.Success)
                    {
                        var namePart = match.Groups[1].Value;
                        targetField.FieldName = $"{namePart}{targetField.Level}";
                    }
                }
                targetField.FieldName ??= sourceFields.FieldName;
            }
        }
    }
}
