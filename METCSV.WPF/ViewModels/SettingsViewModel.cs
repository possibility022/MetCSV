using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MET.Proxy.Configuration;
using METCSV.Common;
using METCSV.WPF.Configuration;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MET.Data.Storage;
using METCSV.WPF.ProductProvider;
using Microsoft.Toolkit.Mvvm.Input;
using Org.BouncyCastle.Utilities.Collections;

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

        private bool renameSettingsIsActive;
        public bool RenameSettingsIsActive
        {
            get { return renameSettingsIsActive; }
            set { SetProperty(ref renameSettingsIsActive, value); }
        }

        private bool _generalTabIsActive;
        public bool GeneralTabIsActive
        {
            get { return _generalTabIsActive; }
            set { SetProperty(ref _generalTabIsActive, value); }
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

        private EngineSettings _engineSettings;
        public EngineSettings EngineSettings
        {
            get { return _engineSettings; }
            set { SetProperty(ref _engineSettings, value); }
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
        private object renameRowSelectedItem;

        public AbDownloaderSettings AbSettings
        {
            get { return _abSettings; }
            set { SetProperty(ref _abSettings, value); }
        }


        public SettingsViewModel()
        {
            RemoveSelectedRenameRow =
                new RelayCommand(() => RemoveSelectedRenameRowAction());
            AddNewRenameRow
                = new RelayCommand(() => AddNewRenameRowAction());

            LoadRenameTable();
            CopyFromSettings();
            _hiddingTask = new Task(HideInfoAfter);
        }


        public object RenameRowSelectedItem
        {
            get => renameRowSelectedItem;
            set
            {
                SetProperty(ref renameRowSelectedItem, value);
                RaisePropertyChanged(nameof(RemoveSelectedRenameRow));

            }
        }

        public ICommand RemoveSelectedRenameRow { get; }
        public ICommand AddNewRenameRow { get; }

        public ObservableCollection<EditableDictionaryKey<string, string>> RenameMappings { get; set; } =
            new ObservableCollection<EditableDictionaryKey<string, string>>();

        private void RemoveSelectedRenameRowAction()
        {
            if (RenameRowSelectedItem != null)
                RenameMappings.Remove((EditableDictionaryKey<string, string>)RenameRowSelectedItem);
        }

        private void AddNewRenameRowAction()
        {
            RenameMappings.Add(new EditableDictionaryKey<string, string>("Uzupełnij", "Uzupełnij"));
        }

        private void LoadRenameTable()
        {
            using var context = new StorageContext();
            var storageService = new StorageService(context);

            var renameMappings = storageService.GetRenameManufacturerDictionary();

            foreach (var renameMapping in renameMappings)
            {
                RenameMappings.Add(new EditableDictionaryKey<string, string>(renameMapping.Key, renameMapping.Value));
            }
        }

        private void SaveRenameTable()
        {
            using var context = new StorageContext();
            var storageService = new StorageService(context);

            var newMappings = RenameMappings.ToDictionary(r => r.Key, rr => rr.Value);

            storageService.OverrideRenameManufacturerDictionary(newMappings);
        }

        private bool CanSaveRenameTable()
        {
            var s = new HashSet<string>();

            foreach (var editableDictionaryKey in RenameMappings)
            {
                if (!s.Add(editableDictionaryKey.Key))
                {
                    return false;
                }
            }

            return true;
        }


        private void CopyFromSettings()
        {
            MetSettings = new MetDownloaderSettings();
            LamaSettings = new LamaDownloaderSettings();
            TdSettings = new TechDataDownloaderSettings();
            AbSettings = new AbDownloaderSettings();
            EngineSettings = new EngineSettings();


            PropertyCopy.CopyValues(App.Settings.MetDownlaoder, MetSettings);
            PropertyCopy.CopyValues(App.Settings.AbDownloader, AbSettings);
            PropertyCopy.CopyValues(App.Settings.TdDownloader, TdSettings);
            PropertyCopy.CopyValues(App.Settings.LamaDownloader, LamaSettings);
            PropertyCopy.CopyValues(App.Settings.Engine, EngineSettings);
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
                PropertyCopy.CopyValues(AbSettings, App.Settings.AbDownloader);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(TdSettings, App.Settings.TdDownloader);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(LamaSettings, App.Settings.LamaDownloader);

            else if (GeneralTabIsActive)
                PropertyCopy.CopyValues(EngineSettings, App.Settings.Engine);

            else if (RenameSettingsIsActive)
            {
                if (CanSaveRenameTable())
                {
                    SaveRenameTable();
                }
                else
                {
                    MessageBox.Show(
                        "Nie można zapisać. Któraś wartość w pierwszej tabeli wystepuje więcej niż jeden raz.");
                }
            }

            SavedInfo = Visibility.Visible;

            _hiddingTask = new Task(HideInfoAfter);
            _hiddingTask.Start();

        }

        public void RestoreChanges()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(App.Settings.MetDownlaoder, MetSettings);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(App.Settings.AbDownloader, AbSettings);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(App.Settings.TdDownloader, TdSettings);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(App.Settings.LamaDownloader, LamaSettings);

            else if (GeneralTabIsActive)
                PropertyCopy.CopyValues(App.Settings.Engine, EngineSettings);
        }

        public bool AllSaved()
        {

            if (PropertyCopy.AnyChanges(MetSettings, App.Settings.MetDownlaoder))
                return false;

            if (PropertyCopy.AnyChanges(AbSettings, App.Settings.AbDownloader))
                return false;

            if (PropertyCopy.AnyChanges(TdSettings, App.Settings.TdDownloader))
                return false;

            if (PropertyCopy.AnyChanges(LamaSettings, App.Settings.LamaDownloader))
                return false;

            if (PropertyCopy.AnyChanges(EngineSettings, App.Settings.Engine))
                return false;

            return true;
        }

    }

}
