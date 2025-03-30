using System.Text;

namespace SylkReader;

public class SylkReader(Stream stream, Encoding encoding, bool leaveOpen = false) : IDisposable
{
    private readonly StreamReader reader = new(stream, encoding ?? Encoding.Default, leaveOpen);
    private bool disposedValue;

    public Queue<string>? ReadRecords()
    {
        var line = reader.ReadLine();
        if (line == null)
            return new Queue<string>();

        StringBuilder record = new();
        Queue<string> records = new();

        foreach (var c in line)
        {
            if (c == ';')
            {
                records.Enqueue(record.ToString());
                record = new StringBuilder();
                continue;
            }
            record.Append(c);
        }

        if (record.Length > 0)
            records.Enqueue(record.ToString().Trim());

        return records;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                reader?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
