namespace METCSV.WPF.Configuration
{
    public class Settings
    {
        private AbDownloaderSettings _aBDownloader;
        private LamaDownloaderSettings _lamaDownloader;
        private MetDownloaderSettings _metDownlaoder;
        private TechDataDownloaderSettings _tDDownloader;
        private EngineSettings _engine;

        public EngineSettings Engine
        {
            get { return _engine; }
            set { _engine = value ?? new EngineSettings(); }
        }


        public AbDownloaderSettings ABDownloader
        {
            get { return _aBDownloader; }
            set { _aBDownloader = value ?? new AbDownloaderSettings(); }
        }


        public LamaDownloaderSettings LamaDownloader
        {
            get { return _lamaDownloader; }
            set { _lamaDownloader = value ?? new LamaDownloaderSettings(); }
        }


        public MetDownloaderSettings MetDownlaoder
        {
            get { return _metDownlaoder; }
            set { _metDownlaoder = value ?? new MetDownloaderSettings(); }
        }


        public TechDataDownloaderSettings TDDownloader
        {
            get { return _tDDownloader; }
            set { _tDDownloader = value ?? new TechDataDownloaderSettings(); }
        }

    }
}
