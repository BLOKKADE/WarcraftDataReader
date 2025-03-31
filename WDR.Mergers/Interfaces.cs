namespace WDR.Mergers;
public interface IReader<out T>
{
    T GetData();
    void ReadData(string path);
}
