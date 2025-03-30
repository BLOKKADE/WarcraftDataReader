using Warcraft.Models;

namespace CustomWc3Data;

public static class CustomWc3DataMerger
{
    public static Wc3Data ExpandCustomData(Wc3Data defaultData, Wc3Data customData)
    {
        foreach (var origObj in defaultData.Original)
        {
            var customOriginalObj = customData.Original.SingleOrDefault(x => x.Code == origObj.Code);
            var customCustomObjs = customData.Custom.Where(x => x.OriginalCode == origObj.Code);

            // if  the original object is not edited in the custom data then add it
            if (customOriginalObj == null)
            {
                customData.Original.Add(origObj);
            }
            else
            {
                // if found then add all fields not edited in the custom data
                Merge(origObj, customOriginalObj);
            }

            // add all unedited fields from the original obj to all custom objects based on it
            foreach (var obj in customCustomObjs)
            {
                Merge(origObj, obj);
            }
        }

        return customData;
    }

    // Goes through each field in the default data and adds it to the customdata if missing
    // Also adds the field name if it is missing, for easier lookup
    private static void Merge(Wc3Object defaultData, Wc3Object customData)
    {
        foreach (var defaultField in defaultData.Fields)
        {
            var customField = customData.Fields.FirstOrDefault(x => x.Id == defaultField.Id);
            if (customField == null)
            {
                customData.Fields.Add(defaultField);
            }
            else
            {
                customField.FieldName ??= defaultField.FieldName;
            }
        }
    }
}
