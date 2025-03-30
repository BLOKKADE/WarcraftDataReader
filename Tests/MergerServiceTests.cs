using System.Text.Json;
using DefaultWc3Data;
using Warcraft.Models;

namespace Tests;
[TestClass]
public sealed class MergerServiceTests
{
    private readonly DefaultWc3DataService mergerService;

    public MergerServiceTests()
    {
        mergerService = new DefaultWc3DataService();
    }

    [TestMethod]
    public void TestMetaData()
    {
        var metaFiles = Directory.GetFiles("Data/Meta/", "*.slk");

        Dictionary<string, List<MetaData>> allMetaData = [];

        foreach (var file in metaFiles)
        {
            mergerService.ReadMetaData(file);
        }
    }

    [TestMethod]
    public void TestTxtObjects()
    {
        var txtFiles = Directory.GetFiles("Data/Text/", "*.txt");
        foreach (var file in txtFiles)
        {
            mergerService.ReadTxtObjects(file);
        }
    }

    [TestMethod]
    public void TestDataObject()
    {
        TestMetaData();
        var txtFiles = Directory.GetFiles("Data/", "*.slk");
        foreach (var file in txtFiles)
        {
            mergerService.ReadData(file);
        }
    }

    [TestMethod]
    public void TestMerge()
    {
        TestTxtObjects();
        TestDataObject();

        mergerService.MergeObjects();

        var mergedObject = mergerService.Wc3Data;

        // Convert to json
        File.WriteAllText("Data/Everything.json", JsonSerializer.Serialize(mergerService.Wc3Data));
    }
}
