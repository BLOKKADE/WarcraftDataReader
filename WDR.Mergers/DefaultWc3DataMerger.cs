using WDR.Mappers.Models;

namespace WDR.Mergers;

public static partial class DefaultWc3DataMerger
{
    public static Wc3Data Expand(Wc3Data targetData, Wc3Data sourceData)
    {
        ExpandList(targetData.Original, sourceData.Original);
        ExpandList(targetData.Custom, sourceData.Original);

        return targetData;
    }

    private static void ExpandList(List<Wc3Object> targetList, List<Wc3Object> sourceList)
    {
        foreach (var sourceObject in sourceList)
        {
            var targetObjects = targetList.Where(x => x.OriginalCode == sourceObject.Code);
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
                    Merge(sourceObject, targetObject);
                }
            }
        }
    }

    // Goes through each field in the default data and adds it to the customdata if missing
    // Also adds the field name if it is missing, for easier lookup
    private static void Merge(Wc3Object sourceObject, Wc3Object targetObject)
    {
        foreach (var sourceFields in sourceObject.Fields)
        {
            var targetFields = targetObject.Fields.Where(x => x.Id == sourceFields.Id);
            if (targetFields?.Any() != true)
            {
                targetObject.Fields.Add(sourceFields);
            }
            else
            {
                foreach (var targetField in targetFields)
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

    [System.Text.RegularExpressions.GeneratedRegex(@"^(.*?)(\d+)$")]
    private static partial System.Text.RegularExpressions.Regex FieldNameRegex();
}
