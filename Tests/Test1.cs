using System.Text.Json;
using WDR.Mappers.Mappers;
using WDR.Mappers.Mappers.Sylk;
using WDR.Mappers.Models;
namespace WDR.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestStrings()
    {
        var dataDynamic = TxtMapper.MapFromFile("Data/Text/nightelfabilityfunc.txt");
    }

    [TestMethod]
    public void TestAbilities()
    {
        using Stream abilityData = File.OpenRead("Data/AbilityData.slk");

        var dataDynamic = SylkMapper.Map(Sylk.FromStream(abilityData));

        // Convert dataDynamic to prettified json
        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText("Data/AbilityData.json", JsonSerializer.Serialize(dataDynamic, options));
    }

    [TestMethod]
    public void TestAbilityMetaData()
    {
        var metaFiles = Directory.GetFiles("Data/Meta/", "*.slk");
        Dictionary<string, List<MetaData>> allMetaData = [];

        foreach (var file in metaFiles)
        {
            using Stream abilityMetaData = File.OpenRead(file);
            var dataDynamic = SylkMapper.Map(Sylk.FromStream(abilityMetaData));

            var metaData = MetaDataMapper.Map(dataDynamic);

            foreach (var kvp in metaData)
            {
                if (!allMetaData.TryGetValue(kvp.Key, out var metaDataList))
                {
                    metaDataList = [];
                    allMetaData[kvp.Key] = metaDataList;
                }
                metaDataList.AddRange(kvp.Value);
            }
        }

        // Check for duplicates in allMetaData
        foreach (var kvp in allMetaData)
        {
            var distinctList = kvp.Value.Distinct().ToList();
            if (distinctList.Count != kvp.Value.Count)
            {
                Console.WriteLine($"Duplicates found in key: {kvp.Key}");
            }

            // Check for duplicate Field values
            HashSet<string> seenFields = [];
            foreach (var metaData in kvp.Value)
            {
                if (!seenFields.Add(metaData.Field))
                {
                    Console.WriteLine($"Duplicate Field found in key: {kvp.Key}, Field: {metaData.Field}");
                }
            }
        }

        // Convert dataDynamic to prettified json
        JsonSerializerOptions options = new() { WriteIndented = true };
        File.WriteAllText("MetaData.json", JsonSerializer.Serialize(allMetaData, options));
    }
}
