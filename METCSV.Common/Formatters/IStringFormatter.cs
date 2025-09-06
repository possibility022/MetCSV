namespace METCSV.Common.Formatters
{
    public interface IStringFormatter
    {
        void WriteLine(string message);
        void Flush();
    }
}
