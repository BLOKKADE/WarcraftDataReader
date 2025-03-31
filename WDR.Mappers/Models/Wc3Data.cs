namespace WDR.Mappers.Models;

public class Wc3Data
{
    public List<Wc3Object> Original { get; set; } = [];
    public List<Wc3Object> Custom { get; set; } = [];
}

public class Wc3Object
{
    public required string Code { get; set; }
    public string? OriginalCode { get; set; }
    public List<Warcraft3Field> Fields { get; set; } = [];
}
public class Warcraft3Field
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public string? Value { get; set; }
    public string? FieldName { get; set; }
    public int? Column { get; set; }
    public int? Level { get; set; }
}
