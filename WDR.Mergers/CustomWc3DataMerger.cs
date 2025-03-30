using WDR.Mappers.Models;

namespace WDR.Mergers;

public static class CustomWc3DataMerger
{
    public static Wc3Data Expand(Wc3Data targetData, Wc3Data sourceData)
    {
        ExpandList(targetData.Original, sourceData.Original);
        ExpandList(targetData.Custom, sourceData.Custom);
        return targetData;
    }

    private static void ExpandList(List<Wc3Object> targetList, List<Wc3Object> sourceList)
    {
        foreach (var sourceObject in sourceList)
        {
            var targetObjects = targetList.Where(x => x.Code == sourceObject.Code);
            // if  the original object is not edited in the custom data then add it
            if (targetObjects == null)
            {
                targetList.Add(sourceObject);
            }
            else
            {
                // add all unedited fields from the original obj to all custom objects based on it
                foreach (var targetObject in targetObjects)
                {
                    Merge(targetObject, sourceObject);
                }
            }
        }
    }

    // Goes through each field in the default data and adds it to the customdata if missing
    private static void Merge(Wc3Object targetObject, Wc3Object sourceObject)
    {
        var maxLevel = int.TryParse(targetObject.Fields.SingleOrDefault(x => x.Id == "alev")?.Value?.ToString(), out var level) ? level : 0;

        foreach (var sourceField in sourceObject.Fields)
        {
            if (sourceField.Level > maxLevel)
            {
                continue;
            }

            var targetField = targetObject.Fields.Where(x => x.Id == sourceField.Id && x.Level == sourceField.Level);
            if (targetField?.Any() != true)
            {
                targetObject.Fields.Add(sourceField);
            }
            else
            {
                continue;
            }
        }
    }
}
