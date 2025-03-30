using System.Drawing;

namespace WDR.Mappers.Mappers.Sylk;
public class Sylk
{
    public static Sylk FromStreams(params Stream[] streams)
    {
        streams = streams ?? throw new ArgumentNullException(nameof(streams));

        Sylk sylk = new();
        foreach (var stream in streams)
            sylk.Merge(FromStream(stream));
        return sylk;
    }

    public static Sylk FromStream(Stream stream)
    {
        using SylkReader reader = new(stream, System.Text.Encoding.Default);
        Sylk sylk = new();

        var header = reader.ReadRecords();
        if (header!.FirstOrDefault() != "ID")
            throw new FormatException("Invalid SLK data.");

        var isRunning = true;
        var x = 0;
        var y = 0;
        while (isRunning)
        {
            var records = reader.ReadRecords();
            if (records == null)
                break;
            if (records.Count == 0)
                continue;

            switch (records.Dequeue())
            {
                case "B":
                    break;

                case "C":
                    foreach (var record in records)
                    {
                        if (record.StartsWith('X'))
                            x = int.Parse(record[1..]);
                        else if (record.StartsWith('Y'))
                            y = int.Parse(record[1..]);
                        else if (record.StartsWith('K'))
                            sylk.Values[new Point(x, y)] = record[1..].Trim('"');
                        else
                            throw new InvalidOperationException("Unknown value:" + record);
                    }
                    break;

                case "E":
                    isRunning = false;
                    break;
            }
        }

        return sylk;
    }

    public readonly Dictionary<Point, string> Values = [];

    public void Merge(Sylk source)
    {
        foreach (var keyValuePair in source.Values)
        {
            Values[keyValuePair.Key] = keyValuePair.Value;
        }
    }
}
