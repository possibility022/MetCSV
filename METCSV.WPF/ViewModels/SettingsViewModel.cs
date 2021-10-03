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
using METCSV.WPF.Workflows;
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

        private MetDownloaderSettings metDownloaderSettings;
        public MetDownloaderSettings MetDownloaderSettings
        {
            get { return metDownloaderSettings; }
            set { SetProperty(ref metDownloaderSettings, value); }
        }

        private LamaSettings lamaSettings;
        public LamaSettings LamaSettings
        {
            get { return lamaSettings; }
            set { SetProperty(ref lamaSettings, value); }
        }

        private TechDataSettings tdDownloaderSettings;
        public TechDataSettings TdDownloaderSettings
        {
            get { return tdDownloaderSettings; }
            set { SetProperty(ref tdDownloaderSettings, value); }
        }

        private AbSettings abSettings;
        private object renameRowSelectedItem;

        public AbSettings AbSettings
        {
            get { return abSettings; }
            set { SetProperty(ref abSettings, value); }
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
            MetDownloaderSettings = new MetDownloaderSettings();
            LamaSettings = new LamaSettings();
            TdDownloaderSettings = new TechDataSettings();
            AbSettings = new AbSettings();
            EngineSettings = new EngineSettings();


            PropertyCopy.CopyValues(App.Settings.MetDownloaderSettings, MetDownloaderSettings);
            PropertyCopy.CopyValues(App.Settings.AbSettings, AbSettings);
            PropertyCopy.CopyValues(App.Settings.Td, TdDownloaderSettings);
            PropertyCopy.CopyValues(App.Settings.LamaSettings, LamaSettings);
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
                PropertyCopy.CopyValues(MetDownloaderSettings, App.Settings.MetDownloaderSettings);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(AbSettings, App.Settings.AbSettings);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(TdDownloaderSettings, App.Settings.Td);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(LamaSettings, App.Settings.LamaSettings);

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

            SettingsIO.SaveSettings();

            _hiddingTask = new Task(HideInfoAfter);
            _hiddingTask.Start();

        }

        public void RestoreChanges()
        {
            if (MetTabIsActive)
                PropertyCopy.CopyValues(App.Settings.MetDownloaderSettings, MetDownloaderSettings);

            else if (AbTabIsActive)
                PropertyCopy.CopyValues(App.Settings.AbSettings, AbSettings);

            else if (TdTabIsActive)
                PropertyCopy.CopyValues(App.Settings.Td, TdDownloaderSettings);

            else if (LamaTabIsActive)
                PropertyCopy.CopyValues(App.Settings.LamaSettings, LamaSettings);

            else if (GeneralTabIsActive)
                PropertyCopy.CopyValues(App.Settings.Engine, EngineSettings);
        }

        public bool AllSaved()
        {

            if (PropertyCopy.AnyChanges(MetDownloaderSettings, App.Settings.MetDownloaderSettings))
                return false;

            if (PropertyCopy.AnyChanges(AbSettings, App.Settings.AbSettings))
                return false;

            if (PropertyCopy.AnyChanges(TdDownloaderSettings, App.Settings.Td))
                return false;

            if (PropertyCopy.AnyChanges(LamaSettings, App.Settings.LamaSettings))
                return false;

            if (PropertyCopy.AnyChanges(EngineSettings, App.Settings.Engine))
                return false;

            return true;
        }

    }

}
