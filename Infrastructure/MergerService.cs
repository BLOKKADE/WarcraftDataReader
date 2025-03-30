using Warcraft.Mapper;
using Warcraft.Objects;

namespace Infrastructure;

public class MergerService
{
    public required Warcraft3Object Warcraft3Object { get; set; }
    public List<Warcraft.Objects.Object> TxtObjects { get; set; } = [];
    public Dictionary<string, List<MetaData>> MetaData { get; set; } = [];

    public void ReadTxtObjects(string path)
    {
        foreach (var newObject in TxtMapper.MapFromFile(path))
        {
            var existingObject = TxtObjects.FirstOrDefault(o => o.Code == newObject.Code);
            if (existingObject == null)
            {
                TxtObjects.Add(newObject);
            }
            else
            {
                foreach (var field in newObject.Fields)
                {
                    if (existingObject.Fields.ContainsKey(field.Key))
                    {
                        throw new InvalidOperationException($"Field '{field.Key}' already exists in object with Code '{existingObject.Code}'.");
                    }
                    existingObject.Fields[field.Key] = field.Value;
                }
            }
        }
    }
    /*
    public void ReadMetaData(string path)
    {
        using Stream metaDataStream = File.OpenRead(path);
        var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(metaDataStream));

        var metaData = MetaDataMapper.Map(dataDynamic);

        var key = Path.GetFileNameWithoutExtension(path);

        if (MetaData.ContainsKey(key))
        {
            throw new InvalidOperationException($"MetaData for '{key}' already exists.");
        }

        MetaData[key] = metaData;
    }

    public void ReadData(string dataPath)
    {
        var warcraft3Object = WarcraftObjectMapper.MapFromJson(dataPath, metaPath);
        if (Warcraft3Object != null)
        {
            throw new InvalidOperationException("Warcraft3Object already exists.");
        }
        Warcraft3Object = warcraft3Object;
    }
    */
}
