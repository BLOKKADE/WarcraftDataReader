namespace WDR.Mergers.Readers;
public interface IReader<out T>
{
    T GetData();
    void ReadData(string path);
}
