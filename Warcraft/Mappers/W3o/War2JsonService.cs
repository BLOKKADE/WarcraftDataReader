using Warcraft.Models;

namespace Warcraft.Mappers.W3o;
public static class War2JsonService
{
    public static Wc3Data ProcessFile(string inputPath, ObjectType objectType)
    {
        var buffer = File.ReadAllBytes(inputPath);
        return ObjectTranslator.WarToJson(buffer, objectType);
    }
}
