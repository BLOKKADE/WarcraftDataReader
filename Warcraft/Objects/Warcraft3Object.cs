namespace Warcraft.Objects;

public class Warcraft3Object
{
    public required Dictionary<string, List<Warcraft3Field>> Original { get; set; }
}

public class Warcraft3Field
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required object Value { get; set; }
    public required string FieldName { get; set; }
    public int? Column { get; set; }
    public int? Level { get; set; }
}
