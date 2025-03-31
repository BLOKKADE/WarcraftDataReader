using System.Text.Json;
using WDR.Mappers.Mappers;
using WDR.Mappers.Mappers.W3o;
using WDR.Mappers.Models;
using WDR.Mergers;
using WDR.Mergers.Mergers;
using WDR.Mergers.Readers;

namespace WDR.Tests;

[TestClass]
public sealed class CustomDataService
{
    [TestMethod]
    public void TestCustomData()
    {
        CreateFiles("w3a", ObjectType.Abilities);
        CreateFiles("w3u", ObjectType.Units);
        CreateFiles("w3t", ObjectType.Items);
    }

    private static void CreateFiles(string ending, ObjectType objectType)
    {
        var customAbilityData = ObjectFileReader.FromBuffer(File.ReadAllBytes($"Data/w3o/war3map.{ending}"), objectType);
        var customAbilitySkinData = ObjectFileReader.FromBuffer(File.ReadAllBytes($"Data/w3o/war3mapSkin.{ending}"), objectType);

        ReplaceStrings.ReplaceString(customAbilitySkinData, WtsMapper.MapFromString(File.ReadAllText("Data/w3o/war3map.wts")));

        var customDataMerger = new CustomWc3DataMerger(customAbilityData);

        customDataMerger.Expand(customAbilitySkinData);

        var filteredData = customDataMerger.Wc3Data.Custom.Select(obj => new
        {
            obj.Code,
            FieldsByLevel = obj.Fields
                .GroupBy(f => f.Level ?? 0) // Use 0 as default value for null levels
                .OrderBy(g => g.Key) // Group by level in ascending order
                .ToDictionary(g => g.Key, g => g.Select(f => new { f.Id, f.Value }).ToList()) // Select only id and value
        });

        File.WriteAllText($"Data/{ending}.json", JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public void TestExpandCustomWithDefaultData()
    {
        var slkTypeList = new SlkFiles();
        var txtReader = new TxtDataReader();
        var metaReader = new MetaDataReader(slkTypeList);
        var dataReader = new ObjectDataReader(slkTypeList);
        var mergerService = new ObjectWc3DataMerger(txtReader, metaReader, dataReader);

        foreach (var file in Directory.GetFiles("Data/Text/", "*ability*.txt"))
        {
            txtReader.ReadData(file);
        }

        foreach (var file in Directory.GetFiles("Data/Meta/", "AbilityMetaData.slk"))
        {
            metaReader.ReadData(file);
        }

        foreach (var file in Directory.GetFiles("Data/", "AbilityData.slk"))
        {
            dataReader.ReadData(file);
        }

        var defaultData = mergerService.MergeData();

        var customAbilityData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/w3o/war3map.w3a"), ObjectType.Abilities);
        var customAbilitySkinData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/w3o/war3mapSkin.w3a"), ObjectType.Abilities);

        ReplaceStrings.ReplaceString(customAbilitySkinData, WtsMapper.MapFromString(File.ReadAllText("Data/w3o/war3map.wts")));

        var customDataMerger = new CustomWc3DataMerger(customAbilityData);

        customDataMerger.Expand(customAbilitySkinData);

        var defaultWc3DataMerger = new DefaultWc3DataMerger(customDataMerger.Wc3Data);

        defaultWc3DataMerger.Expand(defaultData);

        //save to json file
        File.WriteAllText("Data/ExpandedCustomData.json", JsonSerializer.Serialize(GroupByLevel(defaultWc3DataMerger.Wc3Data)));
    }

    [TestMethod]
    public void TestDefaultData()
    {
        var slkTypeList = new SlkFiles();
        var txtReader = new TxtDataReader();
        var metaReader = new MetaDataReader(slkTypeList);
        var dataReader = new ObjectDataReader(slkTypeList);
        var mergerService = new ObjectWc3DataMerger(txtReader, metaReader, dataReader);

        foreach (var file in Directory.GetFiles("Data/Text/", "*ability*.txt"))
        {
            txtReader.ReadData(file);
        }

        foreach (var file in Directory.GetFiles("Data/Meta/", "AbilityMetaData.slk"))
        {
            metaReader.ReadData(file);
        }

        foreach (var file in Directory.GetFiles("Data/", "AbilityData.slk"))
        {
            dataReader.ReadData(file);
        }

        var metaData = metaReader.GetData();

        File.WriteAllText("Data/MetaData_2.0.1.json", JsonSerializer.Serialize(metaData));

        var defaultData = mergerService.MergeData();

        //save to json file
        File.WriteAllText("Data/DefaultData_2.0.1.json", JsonSerializer.Serialize(GroupByLevel(defaultData)));
    }

    private static object GroupByLevel(Wc3Data wc3Data)
    {
        var groupedData = new
        {
            Original = wc3Data.Original.ConvertAll(GroupByLevel),
            Custom = wc3Data.Custom.ConvertAll(GroupByLevel)
        };

        return groupedData;
    }

    private static object GroupByLevel(Wc3Object wc3Object)
    {
        var groupedData = new
        {
            wc3Object.Code,
            wc3Object.OriginalCode,
            FieldsByLevel = wc3Object.Fields
                .GroupBy(static f => f.Level ?? 0) // Use 0 as default value for null levels
                .OrderBy(static g => g.Key) // Group by level in ascending order
                .ToDictionary(static g => g.Key, static g => g.ToList())
        };
        return groupedData;
    }
}
