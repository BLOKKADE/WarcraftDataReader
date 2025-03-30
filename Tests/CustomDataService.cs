using System.Text.Json;
using WDR.Mappers.Mappers.W3o;
using WDR.Mergers;

namespace WDR.Tests;

[TestClass]
public sealed class CustomDataService
{
    [TestMethod]
    public void TestCustomData()
    {
        var mergerService = new DefaultWc3DataService();

        foreach (var file in Directory.GetFiles("Data/Text/", "*ability*.txt"))
        {
            mergerService.ReadTxtObjects(file);
        }

        foreach (var file in Directory.GetFiles("Data/Meta/", "AbilityMetaData.slk"))
        {
            mergerService.ReadMetaData(file);
        }

        foreach (var file in Directory.GetFiles("Data/", "AbilityData.slk"))
        {
            mergerService.ReadData(file);
        }

        var customAbilityData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3map.w3a"), ObjectType.Abilities);
        var customAbilitySkinData = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3mapSkin.w3a"), ObjectType.Abilities);

        var defaultData = mergerService.MergeObjects();

        //expand custom data
        var expCustomDataWSkin = CustomWc3DataMerger.Expand(customAbilityData, customAbilitySkinData);

        var expCustomData = DefaultWc3DataMerger.Expand(expCustomDataWSkin, defaultData);

        //save to json file
        File.WriteAllText("Data/ExpandedCustomData.json", JsonSerializer.Serialize(expCustomData));
    }
}
