namespace Warcraft.Models;
public class MappedObject
{
    public required string Code { get; set; }
    public Dictionary<string, string> Fields { get; set; } = [];
}
