using MET.Proxy.Configuration;

namespace METCSV.WPF.Configuration
{
    public class Settings : ISettings
    {
        private AbDownloaderSettings aBDownloader = new();
        private LamaDownloaderSettings lamaDownloader = new();
        private MetDownloaderSettings metDownlaoder = new();
        private TechDataDownloaderSettings tDDownloader = new();
        private EngineSettings engine = new();

        public EngineSettings Engine
        {
            get => engine;
            set => engine = value ?? new EngineSettings();
        }


        public AbDownloaderSettings AbDownloader
        {
            get => aBDownloader;
            set => aBDownloader = value ?? new AbDownloaderSettings();
        }


        public LamaDownloaderSettings LamaDownloader
        {
            get => lamaDownloader;
            set => lamaDownloader = value ?? new LamaDownloaderSettings();
        }


        public MetDownloaderSettings MetDownlaoder
        {
            get => metDownlaoder;
            set => metDownlaoder = value ?? new MetDownloaderSettings();
        }


        public TechDataDownloaderSettings TdDownloader
        {
            get => tDDownloader;
            set => tDDownloader = value ?? new TechDataDownloaderSettings();
        }

        public ITechDataSettings TechDataSettings => TdDownloader;
        public ILamaSettings LamaSettings => LamaDownloader;
        public IAbSettings AbSettings => AbDownloader;
        public IMetSettings MetSettings => MetDownlaoder;
    }
}
