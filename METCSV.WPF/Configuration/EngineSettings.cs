namespace METCSV.WPF.Configuration
{
    public class EngineSettings
    {
        public bool OfflineMode { get; set; } = true;

        public bool SetProfits { get; set; } = true;
        public string NewVersionURL { get; set; } = "http://tbsys.ddns.net/metcsv/version.xml";

        public int MaximumPriceErrorDifference { get; set; } = 20;
        public double DefaultProfit { get; set; } = 0.1;
    }
}
