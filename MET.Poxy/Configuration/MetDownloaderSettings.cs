namespace MET.Proxy.Configuration
{
    public class MetDownloaderSettings : IMetDownloaderSettings
    {
        public string CsvFile { get; set; } = "met.csv";

        public string Url { get; set; } = "http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv";
        public string MetPriceCsvFile { get; set; } = "metprice.csv";
        public string MetPriceUrl { get; set; } = "https://met.redcart.pl/export/9795fdf6ed48024e5e20eadb1d4d8a0f.csv";
    }
}
