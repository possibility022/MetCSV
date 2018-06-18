namespace METCSV.WPF.Configuration
{
    public class Settings
    {
        public Settings()
        {
            Initialize();
        }

        public Settings(bool initialize)
        {
            if (initialize)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (ABDownloader == null)
                ABDownloader = new AbDownloaderSettings();
            if (LamaDownloader == null)
                LamaDownloader = new LamaDownloaderSettings();
            if (MetDownlaoder == null)
                MetDownlaoder = new MetDownloaderSettings();
            if (TDDownloader == null)
                TDDownloader = new TechDataDownloaderSettings();
        }

        public EngineSettings Engine { get; set; }


        public AbDownloaderSettings ABDownloader { get; set; }


        public LamaDownloaderSettings LamaDownloader { get; set; }


        public MetDownloaderSettings MetDownlaoder { get; set; }


        public TechDataDownloaderSettings TDDownloader { get; set; }

    }
}
