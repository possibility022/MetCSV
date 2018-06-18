using METCSV.Common;
using METCSV.WPF.Configuration;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    public class SettingsViewModel : BindableBase
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

        private AbDownloaderSettings _abSettings;
        public AbDownloaderSettings AbSettings
        {
            get { return _abSettings; }
            set { SetProperty(ref _abSettings, value); }
        }


        public SettingsViewModel()
        {
            CopyFromSettings();
        }

        private void CopyFromSettings()
        {
            MetSettings = new MetDownloaderSettings();
            LamaSettings = new LamaDownloaderSettings();
            TdSettings = new TechDataDownloaderSettings();
            AbSettings = new AbDownloaderSettings();

            PropertyCopy.CopyValues(App.SETTINGS.MetDownlaoder, MetSettings);
            PropertyCopy.CopyValues(App.SETTINGS.ABDownloader, AbSettings);
            PropertyCopy.CopyValues(App.SETTINGS.TDDownloader, TdSettings);
            PropertyCopy.CopyValues(App.SETTINGS.LamaDownloader, LamaSettings);
        }

        public void Save()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(MetSettings, App.SETTINGS.MetDownlaoder);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(AbSettings, App.SETTINGS.ABDownloader);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(TdSettings, App.SETTINGS.TDDownloader);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(LamaSettings, App.SETTINGS.LamaDownloader);
        }

        public void RestoreChanges()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(App.SETTINGS.MetDownlaoder, MetSettings);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(App.SETTINGS.ABDownloader, AbSettings);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(App.SETTINGS.TDDownloader, TdSettings);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(App.SETTINGS.LamaDownloader, LamaSettings);
        }

        public bool AllSaved()
        {

            if (PropertyCopy.AnyChanges(MetSettings, App.SETTINGS.MetDownlaoder))
                return false;

            if (PropertyCopy.AnyChanges(AbSettings, App.SETTINGS.ABDownloader))
                return false;

            if (PropertyCopy.AnyChanges(TdSettings, App.SETTINGS.TDDownloader))
                return false;

            if (PropertyCopy.AnyChanges(LamaSettings, App.SETTINGS.LamaDownloader))
                return false;

            return true;
        }

    }

}
