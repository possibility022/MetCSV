namespace MET.Proxy.Configuration
{
    public interface ILamaReaderSettings
    {
        string CsvFileEncoding { get; set; }
        string CsvDelimiter { get; set; }
    }
}