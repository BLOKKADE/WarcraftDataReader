using System.Text;

namespace WDR.Mappers.Mappers.Sylk;

public class SylkReader(Stream stream, Encoding encoding, bool leaveOpen = false) : IDisposable
{
    private readonly StreamReader _reader = new(stream, encoding ?? Encoding.Default, leaveOpen);
    private bool _disposedValue;

    public Queue<string>? ReadRecords()
    {
        var line = _reader.ReadLine();
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
        if (!_disposedValue)
        {
            if (disposing)
            {
                _reader?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
