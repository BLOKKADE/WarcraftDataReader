using System.Text.Json;
using WDR.Mappers.Mappers;
using WDR.Mappers.Mappers.W3o;
using WDR.Mappers.Models;
using WDR.Mergers;

namespace WDR.Tests;

[TestClass]
public sealed class CustomDataService
{
    [TestMethod]
    public void TestCustomData()
    {
        var customAbilityData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3map.w3a"), ObjectType.Abilities);
        var customAbilitySkinData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3mapSkin.w3a"), ObjectType.Abilities);

        ReplaceStrings.ReplaceString(customAbilitySkinData, WtsMapper.MapFromString(File.ReadAllText("Data/war3map.wts")));

        var customDataMerger = new CustomWc3DataMerger(customAbilityData);

        customDataMerger.Expand(customAbilitySkinData);

        File.WriteAllText("Data/ExpandedCustomData.json", JsonSerializer.Serialize(GroupByLevel(customDataMerger.Wc3Data)));
    }

    [TestMethod]
    public void TestDefaultData()
    {
        var slkTypeList = new SlkFiles();
        var txtReader = new TxtDataReader();
        var metaReader = new MetaDataReader(slkTypeList);
        var dataReader = new ObjectDataReader(slkTypeList);
        var mergerService = new DataMergeService(txtReader, metaReader, dataReader);

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
