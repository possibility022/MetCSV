using MET.Proxy.Configuration;

namespace METCSV.WPF.Configuration
{
    public class Settings : ISettings
    {
        private AbDownloaderDownloaderSettings aBDownloaderDownloader = new();
        private LamaDownloaderSettings lamaDownloader = new();
        private MetSettings metSettings = new();
        private TechDataDownloaderSettings tDDownloader = new();
        private EngineSettings engine = new();

        public EngineSettings Engine
        {
            get => engine;
            set => engine = value ?? new EngineSettings();
        }


        public AbDownloaderDownloaderSettings AbDownloaderDownloader
        {
            get => aBDownloaderDownloader;
            set => aBDownloaderDownloader = value ?? new AbDownloaderDownloaderSettings();
        }


        public LamaDownloaderSettings LamaDownloader
        {
            get => lamaDownloader;
            set => lamaDownloader = value ?? new LamaDownloaderSettings();
        }


        public MetSettings MetSettings
        {
            get => metSettings;
            set => metSettings = value ?? new MetSettings();
        }


        public TechDataDownloaderSettings TdDownloader
        {
            get => tDDownloader;
            set => tDDownloader = value ?? new TechDataDownloaderSettings();
        }

        public ITechDataSettings TechDataSettings => TdDownloader;
        public ILamaSettings LamaSettings => LamaDownloader;
        public IAbDownloaderSettings AbDownloaderSettings => AbDownloaderDownloader;

        IMetSettings ISettings.MetSettings => this.MetSettings;
    }
}
