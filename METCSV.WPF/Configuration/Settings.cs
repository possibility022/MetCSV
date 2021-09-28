using MET.Proxy.Configuration;

namespace METCSV.WPF.Configuration
{
    public class Settings : ISettings
    {
        private AbSettings abSettings = new();
        private LamaSettings lamaSettings = new();
        private MetDownloaderSettings metDownloaderSettings = new();
        private TechDataSettings techDataSettings = new();
        private EngineSettings engine = new();

        public EngineSettings Engine
        {
            get => engine;
            set => engine = value ?? new EngineSettings();
        }
        
        public AbSettings AbSettings
        {
            get => abSettings;
            set => abSettings = value ?? new AbSettings();
        }
        
        public LamaSettings LamaSettings
        {
            get => lamaSettings;
            set => lamaSettings = value ?? new LamaSettings();
        }
        
        public MetDownloaderSettings MetDownloaderSettings
        {
            get => metDownloaderSettings;
            set => metDownloaderSettings = value ?? new MetDownloaderSettings();
        }

        public TechDataSettings Td
        {
            get => techDataSettings;
            set => techDataSettings = value ?? new TechDataSettings();
        }

        ITechDataReaderSettings ISettings.TechDataReaderSettings => techDataSettings;

        ILamaReaderSettings ISettings.LamaReaderSettings => lamaSettings;

        IAbReaderSettings ISettings.AbReaderSettings => abSettings;
        
        ITechDataDownloaderSettings ISettings.TechDataDownloaderSettings => Td;
        ILamaDownloaderSettings ISettings.LamaDownloaderSettings => LamaSettings;
        IAbDownloaderSettings ISettings.AbDownloaderSettings => AbSettings;

        IMetDownloaderSettings ISettings.MetDownloaderSettings => MetDownloaderSettings;
    }
}
