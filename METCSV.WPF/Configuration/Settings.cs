namespace METCSV.WPF.Configuration
{
    public static class Settings
    {

        static Settings()
        {
            ABDownloader = new AbDownloaderSettings();
            LamaDownloader = new LamaDownloaderSettings();
            MetDownlaoder = new MetDownloaderSettings();
            TDDownloader = new TechDataDownloaderSettings();
        }

        public static bool OfflineMode { get; set; } = true;

        public static AbDownloaderSettings ABDownloader { get; set; }

        public static LamaDownloaderSettings LamaDownloader { get; set; }

        public static MetDownloaderSettings MetDownlaoder { get; set; }

        public static TechDataDownloaderSettings TDDownloader { get; set; }

    }
}
