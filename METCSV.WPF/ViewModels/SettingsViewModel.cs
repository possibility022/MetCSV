using METCSV.WPF.Configuration;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    class SettingsViewModel : BindableBase
    {

        private bool _metTabIsActive = true;
        public bool MetTabIsActive
        {
            get { return _metTabIsActive; }
            set { SetProperty(ref _metTabIsActive, value); }
        }

        private bool _abTabIsActive;
        public bool AbTabIsActive
        {
            get { return _abTabIsActive; }
            set { SetProperty(ref _abTabIsActive, value); }
        }

        private bool _tdTabIsActive;
        public bool TdTabIsActive
        {
            get { return _tdTabIsActive; }
            set { SetProperty(ref _tdTabIsActive, value); }
        }

        private bool _lamaTabIsActive;
        public bool LamaTabIsActive
        {
            get { return _lamaTabIsActive; }
            set { SetProperty(ref _lamaTabIsActive, value); }
        }

        private MetDownloaderSettings _metSettings;
        public MetDownloaderSettings MetSettings
        {
            get { return _metSettings; }
            set { SetProperty(ref _metSettings, value); }
        }

        private LamaDownloaderSettings _lamaSettings;
        public LamaDownloaderSettings LamaSettings
        {
            get { return _lamaSettings; }
            set { SetProperty(ref _lamaSettings, value); }
        }

        private TechDataDownloaderSettings _tdSettings;
        public TechDataDownloaderSettings TdSettings
        {
            get { return _tdSettings; }
            set { SetProperty(ref _tdSettings, value); }
        }


        public SettingsViewModel()
        {
            MetSettings = new MetDownloaderSettings(); // todo load it from settings.
            LamaSettings = new LamaDownloaderSettings();
            TdSettings = new TechDataDownloaderSettings();
        }



    }

}
