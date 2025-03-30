using System.Text.Json;
using Warcraft.Mapper;
using Warcraft.Objects;
using Warcraft.Sylk;
namespace Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestStrings()
    {
        var dataDynamic = TxtMapper.Map("Data/Text/nightelfabilityfunc.txt");
    }

    [TestMethod]
    public void TestAbilities()
    {
        using Stream abilityData = File.OpenRead("Data/AbilityData.slk");

        var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityData));

        // Convert dataDynamic to prettified json
        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText("Data/AbilityData.json", JsonSerializer.Serialize(dataDynamic, options));
    }

    [TestMethod]
    public void TestAbilityMetaData()
    {
        var metaFiles = Directory.GetFiles("Data/Meta/", "*.slk");
        List<MetaData> allMetaData = [];

        foreach (var file in metaFiles)
        {
            using Stream abilityMetaData = File.OpenRead(file);
            var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityMetaData));

            var metaData = MetaDataMapper.Map(dataDynamic);
            allMetaData.AddRange(metaData);
        }

        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText("MetaData.json", JsonSerializer.Serialize(allMetaData, options));
    }

    [TestMethod]
    public void TestWarcraftObjectConverter_WithAbilities()
    {
        using Stream abilityData = File.OpenRead("Data/AbilityData.slk");

        var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityData));

        // Map dataDynamic to List<Dictionary<string, string>>
        var dataDictionaryList = dataDynamic
            .ConvertAll(d => ((IDictionary<string, object>)d).ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty));

        using Stream abilityMetaData = File.OpenRead("Data/Meta/AbilityMetaData.slk");
        var metaDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityMetaData));

        var metaData = MetaDataMapper.Map(dataDynamic);

        var objects = WarcraftObjectMapper.MapJsonToWarcraft3Object("AbilityData", dataDictionaryList, metaData);
    }

    [TestMethod]
    public void TestWarcraftObjectConverter_WithItems()
    {
        using Stream itemData = File.OpenRead("Data/ItemData.slk");

        var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(itemData));

        // Map dataDynamic to List<Dictionary<string, string>>
        var dataDictionaryList = dataDynamic
            .ConvertAll(d => ((IDictionary<string, object>)d).ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty));

        using Stream itemMetaData = File.OpenRead("Data/Meta/UnitMetaData.slk");
        var metaDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(itemMetaData));

        var metaData = MetaDataMapper.Map(dataDynamic);

        var objects = WarcraftObjectMapper.MapJsonToWarcraft3Object("ItemData", dataDictionaryList, metaData);
    }

    [TestMethod]
    public void TestWarcraftObjectConverter_WithUnits()
    {
        using Stream abilityMetaData = File.OpenRead("Data/Meta/UnitMetaData.slk");
        var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityMetaData));

        var metaData = MetaDataMapper.Map(dataDynamic);

        Warcraft3Object objects = new() { Original = [] };

        string[] unitFiles = ["unitbalance", "unitdata", "unitui", "unitweapons", "unitabilities"];

        foreach (var str in unitFiles)
        {
            using Stream abilityData = File.OpenRead($"Data/{str}.slk");
            var dynamicObjects = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityData));
            // Map dataDynamic to List<Dictionary<string, string>>
            var dataDictionaryList = dynamicObjects
                .ConvertAll(d => ((IDictionary<string, object>)d).ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty));
            var objects2 = WarcraftObjectMapper.MapJsonToWarcraft3Object(str, dataDictionaryList, metaData);

            // Add everything in objects2 to objects
            foreach (var kvp in objects2.Original)
            {
                if (!objects.Original.TryGetValue(kvp.Key, out var value))
                {
                    value = [];
                    objects.Original[kvp.Key] = value;
                }

                value.AddRange(kvp.Value);
            }
        }
    }
}
