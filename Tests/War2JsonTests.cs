using Warcraft.Mappers.W3o;

namespace Tests;

[TestClass]
public sealed class War2JsonTests
{
    [TestMethod]
    public void TestAbilities()
    {
        var result = War2JsonService.ProcessFile("Data/war3map.w3a", ObjectType.Abilities);
    }
}
