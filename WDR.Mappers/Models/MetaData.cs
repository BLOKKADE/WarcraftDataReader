namespace WDR.Mappers.Models;
public class MetaData
{
    // Code used in the world editor for example "Hbz2"
    public required string Code { get; set; }
    // For example: "DataA, DataB, HeroDur" or "Ubertip, Tip"
    public required string Field { get; set; }
    // Type of the field for example "string", "int", "real", "unreal"
    public required string Type { get; set; }
    public required int Column { get; set; }
    // Ids of abilities that use the Code, only used by abilities
    public required List<string> AbilityId { get; set; }
    public required string Slk { get; set; }
}
