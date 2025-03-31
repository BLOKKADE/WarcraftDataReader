using WDR.Mappers.Mappers;
using WDR.Mappers.Models;

namespace WDR.Mergers;

public class DataMergeService(
    IReader<Dictionary<string, List<MappedObject>>> textReader,
    IReader<Dictionary<string, List<MetaData>>> metaReader,
    IReader<Dictionary<string, List<MappedObject>>> dataReader) : BaseWc3DataMerger
{
    public Wc3Data MergeData()
    {
        var mappedTxt = textReader.GetData();
        var metaData = metaReader.GetData();
        var mappedObjects = dataReader.GetData();

        return MergeObjects(mappedTxt, mappedObjects, metaData);
    }

    private Wc3Data MergeObjects(
        Dictionary<string, List<MappedObject>> mappedTxtDict,
        Dictionary<string, List<MappedObject>> mappedObjectsDict,
        Dictionary<string, List<MetaData>> metaDataDict)
    {
        var wc3Data = new Wc3Data();

        // Create data object using txt objects with MetaData
        foreach (var (type, mappedObjects) in mappedTxtDict)
        {
            if (!metaDataDict.TryGetValue(type, out var metaData) && type == "item")
            {
                metaData = metaDataDict.GetValueOrDefault("unit");
            }

            if (metaData == null)
            {
                throw new InvalidOperationException($"Missing metadata for {type}");
            }

            wc3Data = WarcraftObjectMapper.MapObjectsToWarcraftObject("Profile", mappedObjects, metaData);
        }

        // Create data objects using objects with MetaData and txtobjects output
        foreach (var (type, mappedObjects) in mappedObjectsDict)
        {
            if (metaDataDict.TryGetValue(type, out var metaData))
            {
                var result = WarcraftObjectMapper.MapObjectsToWarcraftObject(type, mappedObjects, metaData);
                ExpandList(wc3Data.Original, result.Original, x => x.Code);
            }
            else
            {
                throw new InvalidOperationException($"Missing metadata for {type}");
            }
        }

        return wc3Data;
    }

    protected override void Merge(Wc3Object targetObject, Wc3Object sourceObject)
    {
        foreach (var sourceField in sourceObject.Fields)
        {
            var targetField = targetObject.Fields.FirstOrDefault(x => x.Id == sourceField.Id);
            if (targetField == null)
            {
                targetObject.Fields.Add(sourceField);
            }
        }
    }
}
