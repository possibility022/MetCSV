namespace MET.Proxy.Configuration
{
    public interface IMetDownloaderSettings
    {
        public string CsvFile { get; }

        public string Url { get; }

        public string MetPriceCsvFile { get; set; }

        public string MetPriceUrl { get; set; }
    }
}