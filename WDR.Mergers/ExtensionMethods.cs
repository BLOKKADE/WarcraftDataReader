using WDR.Mappers.Models;

namespace WDR.Mergers;
public static class ExtensionMethods
{
    public static void MergeMappedObjects(this List<MappedObject> list, MappedObject newObject)
    {
        var existingObject = list.FirstOrDefault(o => o.Code == newObject.Code);

        if (existingObject == null)
        {
            list.Add(newObject);
        }
        else
        {
            foreach (var field in newObject.Fields)
            {
                if (existingObject.Fields.ContainsKey(field.Key))
                {
                    if (existingObject.Fields[field.Key] == field.Value)
                    {
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                existingObject.Fields[field.Key] = field.Value;
            }
        }
    }

    public static void MergeWarcraftObjects(this Wc3Data target, Wc3Data source)
    {
        foreach (var sourceObj in source.Original)
        {
            var targetObj = target.Original.FirstOrDefault(x => x.Code == sourceObj.Code);

            if (targetObj != null)
            {
                foreach (var sourceField in sourceObj.Fields)
                {
                    var targetField = targetObj.Fields.FirstOrDefault(x => x.Id == sourceField.Id);
                    if (targetField == null)
                    {
                        targetObj.Fields.Add(sourceField);
                    }
                }
            }
            else
            {
                target.Original.Add(sourceObj);
            }
        }
    }
}
