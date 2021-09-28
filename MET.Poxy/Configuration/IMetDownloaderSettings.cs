namespace MET.Proxy.Configuration
{
    public interface IMetDownloaderSettings
    {
        public string CsvFile { get; }

        public string Url { get; }
    }
}