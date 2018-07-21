using MET.Proxy.Configuration;
using METCSV.Common;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace METCSV.WPF.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private Task _hiddingTask;

        private Visibility _savedInfo = Visibility.Hidden;
        public Visibility SavedInfo
        {
            get { return _savedInfo; }
            set { SetProperty(ref _savedInfo, value); }
        }

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
            _hiddingTask = new Task(HideInfoAfter);
        }

        private void CopyFromSettings()
        {
            MetSettings = new MetDownloaderSettings();
            LamaSettings = new LamaDownloaderSettings();
            TdSettings = new TechDataDownloaderSettings();
            AbSettings = new AbDownloaderSettings();

            PropertyCopy.CopyValues(App.Settings.MetDownlaoder, MetSettings);
            PropertyCopy.CopyValues(App.Settings.ABDownloader, AbSettings);
            PropertyCopy.CopyValues(App.Settings.TDDownloader, TdSettings);
            PropertyCopy.CopyValues(App.Settings.LamaDownloader, LamaSettings);
        }

        private void HideInfoAfter()
        {
            Thread.Sleep(1000);
            SavedInfo = Visibility.Hidden;
        }

        public void Save()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(MetSettings, App.Settings.MetDownlaoder);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(AbSettings, App.Settings.ABDownloader);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(TdSettings, App.Settings.TDDownloader);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(LamaSettings, App.Settings.LamaDownloader);

            SavedInfo = Visibility.Visible;

            _hiddingTask = new Task(HideInfoAfter);
            _hiddingTask.Start();

        }

        public void RestoreChanges()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(App.Settings.MetDownlaoder, MetSettings);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(App.Settings.ABDownloader, AbSettings);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(App.Settings.TDDownloader, TdSettings);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(App.Settings.LamaDownloader, LamaSettings);
        }

        public bool AllSaved()
        {

            if (PropertyCopy.AnyChanges(MetSettings, App.Settings.MetDownlaoder))
                return false;

            if (PropertyCopy.AnyChanges(AbSettings, App.Settings.ABDownloader))
                return false;

            if (PropertyCopy.AnyChanges(TdSettings, App.Settings.TDDownloader))
                return false;

            if (PropertyCopy.AnyChanges(LamaSettings, App.Settings.LamaDownloader))
                return false;

            return true;
        }

    }

}
