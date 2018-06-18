namespace METCSV.WPF.Configuration
{
    public class Settings
    {
        public Settings()
        {
            ABDownloader = new AbDownloaderSettings();
            LamaDownloader = new LamaDownloaderSettings();
            MetDownlaoder = new MetDownloaderSettings();
            TDDownloader = new TechDataDownloaderSettings();
        }

        public EngineSettings Engine { get; set; }


        public AbDownloaderSettings ABDownloader { get; set; }


        public LamaDownloaderSettings LamaDownloader { get; set; }


        public MetDownloaderSettings MetDownlaoder { get; set; }


        public TechDataDownloaderSettings TDDownloader { get; set; }

    }
}
