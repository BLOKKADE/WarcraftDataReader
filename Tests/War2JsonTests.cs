using WDR.Mappers.Mappers.W3o;

namespace WDR.Tests;

[TestClass]
public sealed class War2JsonTests
{
    [TestMethod]
    public void Testw3a()
    {
        var result = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3map.w3a"), ObjectType.Abilities);
    }

    [TestMethod]
    public void Testw3aSkin()
    {
        var result = ObjectFileReader.FromBuffer(File.ReadAllBytes("Data/war3mapSkin.w3a"), ObjectType.Abilities);
    }
}
