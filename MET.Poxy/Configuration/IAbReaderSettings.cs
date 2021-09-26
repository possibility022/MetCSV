namespace MET.Proxy.Configuration
{
    public interface IAbReaderSettings
    {
        public string CsvDelimiter { get; set; }

        public string CsvFileEncoding { get; set; }
    }
}