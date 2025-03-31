using WDR.Mappers.Models;

namespace WDR.Mergers;

public class CustomWc3DataMerger(Wc3Data targetData) : BaseWc3DataMerger
{
    public Wc3Data Wc3Data { get; } = targetData;
    public void Expand(Wc3Data sourceData)
    {
        ExpandList(Wc3Data.Original, sourceData.Original, x => x.Code);
        ExpandList(Wc3Data.Custom, sourceData.Custom, x => x.Code);
    }

    // Goes through each field in the source object and adds it to the target object if missing
    protected override void Merge(Wc3Object targetObject, Wc3Object sourceObject)
    {
        var temp = targetObject.Fields.SingleOrDefault(x => x.Id == "alev")?.Value;
        var maxLevel = int.TryParse(temp, out var level) ? level : 0;

        var targetFieldsDict = targetObject.Fields
            .GroupBy(x => (x.Id, x.Level))
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var sourceField in sourceObject.Fields)
        {
            if (sourceField.Level > maxLevel)
            {
                continue;
            }

            var key = (sourceField.Id, sourceField.Level);
            if (!targetFieldsDict.ContainsKey(key))
            {
                targetObject.Fields.Add(sourceField);
                targetFieldsDict[key] = sourceField;
            }
        }
    }
}
