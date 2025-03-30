using System.Text.Json;
using SylkReader;
using Warcraft.Mapper;
using Warcraft.Objects;
namespace Tests;

[TestClass]
public sealed class Test1
{
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

            MetaDataMapper metaDataMapper = new();
            var metaData = metaDataMapper.Map(dataDynamic);
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

        MetaDataMapper metaDataMapper = new();
        var metaData = metaDataMapper.Map(metaDynamic);

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

        MetaDataMapper metaDataMapper = new();
        var metaData = metaDataMapper.Map(metaDynamic);

        var objects = WarcraftObjectMapper.MapJsonToWarcraft3Object("ItemData", dataDictionaryList, metaData);
    }

    [TestMethod]
    public void TestWarcraftObjectConverter_WithUnits()
    {
        using Stream abilityMetaData = File.OpenRead("Data/Meta/UnitMetaData.slk");
        var metaDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityMetaData));

        MetaDataMapper metaDataMapper = new();
        var metaData = metaDataMapper.Map(metaDynamic);

        Warcraft3Object objects = new() { Original = [] };

        string[] unitFiles = { "unitbalance", "unitdata", "unitui", "unitweapons", "unitabilities" };

        foreach (var str in unitFiles)
        {
            using Stream abilityData = File.OpenRead($"Data/{str}.slk");
            var dataDynamic = DynamicSylkMapper.MapToDynamicObjects(Sylk.FromStream(abilityData));
            // Map dataDynamic to List<Dictionary<string, string>>
            var dataDictionaryList = dataDynamic
                .ConvertAll(d => ((IDictionary<string, object>)d).ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty));
            var objects2 = WarcraftObjectMapper.MapJsonToWarcraft3Object(str, dataDictionaryList, metaData);

            // Add everything in objects2 to objects
            foreach (var kvp in objects2.Original)
            {
                if (!objects.Original.TryGetValue(kvp.Key, out var value))
                {
                    value = new List<Warcraft3Field>();
                    objects.Original[kvp.Key] = value;
                }

                value.AddRange(kvp.Value);
            }
        }
    }
}
