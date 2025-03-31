using System.Text.Json;
using WDR.Mergers.Mergers;
using WDR.Mergers.Readers;

namespace WDR.Tests;
[TestClass]
public sealed class MergerServiceTests
{
    private readonly ObjectWc3DataMerger mergerService;
    private readonly TxtDataReader txtReader;
    private readonly MetaDataReader metaReader;
    private readonly ObjectDataReader dataReader;

    public MergerServiceTests()
    {
        var slkTypeList = new SlkFiles();
        txtReader = new TxtDataReader();
        metaReader = new MetaDataReader(slkTypeList);
        dataReader = new ObjectDataReader(slkTypeList);
        mergerService = new ObjectWc3DataMerger(txtReader, metaReader, dataReader);
    }

    [TestMethod]
    public void TestMetaData()
    {
        var metaFiles = Directory.GetFiles("Data/Meta/", "*.slk");

        foreach (var file in metaFiles)
        {
            metaReader.ReadData(file);
        }
    }

    [TestMethod]
    public void TestTxtObjects()
    {
        foreach (var file in Directory.GetFiles("Data/Text/", "*.txt"))
        {
            txtReader.ReadData(file);
        }
    }

    [TestMethod]
    public void TestDataObject()
    {
        TestMetaData();
        foreach (var file in Directory.GetFiles("Data/", "*.slk"))
        {
            dataReader.ReadData(file);
        }
    }

    [TestMethod]
    public void TestMerge()
    {
        TestTxtObjects();
        TestDataObject();

        var mergedObject = mergerService.MergeData();

        // Convert to json
        File.WriteAllText("Data/Everything.json", JsonSerializer.Serialize(mergedObject));
    }
}
