using WDR.Mappers.Models;

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
        foreach (var sourceFields in sourceObject.Fields)
        {
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
