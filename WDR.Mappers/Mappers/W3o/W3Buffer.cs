using System.Text;

namespace WDR.Mappers.Mappers.W3o;

internal class W3Buffer(byte[] buffer)
{
    private int _offset = 0;

    public int ReadInt()
    {
        var result = BitConverter.ToInt32(buffer, _offset);
        _offset += 4;
        return result;
    }

    public short ReadShort()
    {
        var result = BitConverter.ToInt16(buffer, _offset);
        _offset += 2;
        return result;
    }

    public float ReadFloat()
    {
        var result = BitConverter.ToSingle(buffer, _offset);
        _offset += 4;
        return (float)Math.Round(result, 3);
    }

    public string ReadString()
    {
        var stringBytes = new List<byte>();
        while (buffer[_offset] != 0x00)
        {
            stringBytes.Add(buffer[_offset]);
            _offset++;
        }
        _offset++; // consume the \0 end-of-string delimiter
        return Encoding.UTF8.GetString([.. stringBytes]);
    }

    public string ReadChars(int len = 1)
    {
        var stringBytes = new List<byte>();
        for (var i = 0; i < len; i++)
        {
            stringBytes.Add(buffer[_offset]);
            _offset++;
        }
        return Encoding.UTF8.GetString([.. stringBytes]);
    }

    public byte ReadByte()
    {
        var result = buffer[_offset];
        _offset++;
        return result;
    }

    public bool IsExhausted()
    {
        return _offset == buffer.Length;
    }
}
