using Warcraft.Mappers;
using Warcraft.Mappers.Sylk;
using Warcraft.Models;

namespace DefaultWc3Data;

public class DefaultWc3DataService
{
    private Wc3Data Wc3Data { get; } = new();

    private readonly Dictionary<string, List<MappedObject>> _mappedObjects = [];
    private readonly Dictionary<string, List<MappedObject>> _mappedTxt = [];
    private readonly Dictionary<string, List<MetaData>> _metaData = [];
    private readonly HashSet<string> _uniqueSlkFields = [];

    public void ReadTxtObjects(string path)
    {
        var type = MetaDataHelper.GetMetaDataType(Path.GetFileNameWithoutExtension(path));

        if (!_mappedTxt.TryGetValue(type, out var value))
        {
            value = [];
            _mappedTxt[type] = value;
        }

        var result = TxtMapper.MapFromFile(path);

        // get distinct mapped objects, because for some reason sometimes it contains duplicates????
        var distinct = result.DistinctBy(x => x.Code).ToList();

        distinct.ForEach(mappedObject => value.MergeMappedObjects(mappedObject));
    }

    public void ReadMetaData(string path)
    {
        using Stream metaDataStream = File.OpenRead(path);
        var mappedObjects = SylkMapper.Map(Sylk.FromStream(metaDataStream));

        var metaData = MetaDataMapper.Map(mappedObjects);

        var profileMetaData = metaData.TryGetValue("Profile", out var profile) ? profile : [];

        if (profileMetaData.Count > 0)
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

        // Get all unique slk fields from metaData list
        var slks = new HashSet<string>(metaData.Keys);
        _uniqueSlkFields.UnionWith(slks);

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

    public void ReadData(string path)
    {
        using Stream dataStream = File.OpenRead(path);
        var mappedObjects = SylkMapper.Map(Sylk.FromStream(dataStream));

        // sometimes there are duplicates and the last one is the one used by the game
        var distinct = mappedObjects
            .GroupBy(x => x.Code)
            .Select(g => g.Last())
            .ToList();

        var type = _uniqueSlkFields.First(x => string.Equals(x, Path.GetFileNameWithoutExtension(path), StringComparison.CurrentCultureIgnoreCase));

        if (!_mappedObjects.TryGetValue(type, out var value))
        {
            value = [];
            _mappedObjects[type] = value;
        }

        distinct.ForEach(mappedObject => value.MergeMappedObjects(mappedObject));
    }

    public Wc3Data MergeObjects()
    {
        // Merge txt objects with WarcraftObjectMapper and MetaData
        foreach (var (type, mappedObjects) in _mappedTxt)
        {
            if (!_metaData.TryGetValue(type, out var metaData))
            {
                if (type == "item")
                {
                    metaData = _metaData["unit"];
                }
                else
                {
                    throw new InvalidOperationException($"MetaData for '{type}' does not exist.");
                }
            }

            var result = WarcraftObjectMapper.MapObjectsToWarcraftObject("Profile", mappedObjects, metaData);
            Wc3Data.MergeWarcraftObjects(result);
        }

        // Merge data objects with WarcraftObjectMapper and MetaData
        foreach (var (type, mappedObjects) in _mappedObjects)
        {
            if (!_metaData.TryGetValue(type, out var metaData))
            {
                throw new InvalidOperationException($"MetaData for '{type}' does not exist.");
            }

            var result = WarcraftObjectMapper.MapObjectsToWarcraftObject(type, mappedObjects, metaData);
            Wc3Data.MergeWarcraftObjects(result);
        }

        return Wc3Data;
    }
}
