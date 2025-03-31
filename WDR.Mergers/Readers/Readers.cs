using WDR.Mappers.Mappers;
using WDR.Mappers.Mappers.Sylk;
using WDR.Mappers.Models;
using WDR.Mergers.Helpers;

namespace WDR.Mergers.Readers;
public class TxtDataReader : IReader<Dictionary<string, List<MappedObject>>>
{
    private readonly Dictionary<string, List<MappedObject>> _mappedTxt = [];

    public Dictionary<string, List<MappedObject>> GetData()
    {
        return _mappedTxt;
    }

    public void ReadData(string path)
    {
        var type = MetaDataHelper.GetMetaDataType(Path.GetFileNameWithoutExtension(path));

        if (!_mappedTxt.TryGetValue(type, out var value))
        {
            value = [];
            _mappedTxt[type] = value;
        }

        var result = TxtMapper.MapFromFile(path);
        var distinct = result.DistinctBy(x => x.Code).ToList();

        distinct.ForEach(mappedObject => value.MergeMappedObjects(mappedObject));
    }
}

public class MetaDataReader(SlkFiles slkList) : IReader<Dictionary<string, List<MetaData>>>
{
    private readonly Dictionary<string, List<MetaData>> _metaData = [];

    public Dictionary<string, List<MetaData>> GetData()
    {
        return _metaData;
    }

    public void ReadData(string path)
    {
        using Stream metaDataStream = File.OpenRead(path);
        var mappedObjects = SylkMapper.Map(Sylk.FromStream(metaDataStream));
        var metaData = MetaDataMapper.Map(mappedObjects);

        if (metaData.TryGetValue("Profile", out var profileMetaData) && profileMetaData.Count > 0)
        {
            var type = MetaDataHelper.GetMetaDataType(Path.GetFileNameWithoutExtension(path));
            if (_metaData.TryGetValue(type, out var metaDataList))
            {
                metaDataList.AddRange(profileMetaData);
            }
            else
            {
                _metaData[type] = profileMetaData;
            }
        }

        slkList.UniqueSlkFields.UnionWith(metaData.Keys);

        foreach (var kvp in metaData.Where(x => x.Key != "Profile"))
        {
            if (!_metaData.TryGetValue(kvp.Key, out var metaDataList))
            {
                metaDataList = [];
                _metaData[kvp.Key] = metaDataList;
            }
            metaDataList.AddRange(kvp.Value);
        }
    }
}

public class ObjectDataReader(SlkFiles slkList) : IReader<Dictionary<string, List<MappedObject>>>
{
    private readonly Dictionary<string, List<MappedObject>> _mappedObjects = [];

    public Dictionary<string, List<MappedObject>> GetData()
    {
        return _mappedObjects;
    }

    public void ReadData(string path)
    {
        using Stream dataStream = File.OpenRead(path);
        var objectData = SylkMapper.Map(Sylk.FromStream(dataStream));
        var distinct = objectData.GroupBy(x => x.Code).Select(g => g.Last()).ToList();

        var type = slkList.UniqueSlkFields.First(x => string.Equals(x, Path.GetFileNameWithoutExtension(path), StringComparison.CurrentCultureIgnoreCase));

        if (!_mappedObjects.TryGetValue(type, out var value))
        {
            value = [];
            _mappedObjects[type] = value;
        }

        distinct.ForEach(mappedObject => value.MergeMappedObjects(mappedObject));
    }
}
