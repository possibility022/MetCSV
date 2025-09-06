using System.Collections.ObjectModel;
using MET.Proxy.Configuration;
using METCSV.Common;
using METCSV.WPF.Configuration;
using System.Windows;
using System.Windows.Input;
using MET.Data.Storage;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.Workflows;
using Microsoft.Toolkit.Mvvm.Input;

namespace METCSV.WPF.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private Task hiddingTask;

        private Visibility savedInfo = Visibility.Hidden;
        public Visibility SavedInfo
        {
            get => savedInfo;
            set => SetProperty(ref savedInfo, value);
        }

        private bool renameSettingsIsActive;
        public bool RenameSettingsIsActive
        {
            get => renameSettingsIsActive;
            set => SetProperty(ref renameSettingsIsActive, value);
        }

        private bool generalTabIsActive;
        public bool GeneralTabIsActive
        {
            get => generalTabIsActive;
            set => SetProperty(ref generalTabIsActive, value);
        }

        private bool metTabIsActive = true;
        public bool MetTabIsActive
        {
            get => metTabIsActive;
            set => SetProperty(ref metTabIsActive, value);
        }

        private bool abTabIsActive;
        public bool AbTabIsActive
        {
            get => abTabIsActive;
            set => SetProperty(ref abTabIsActive, value);
        }

        private bool tdTabIsActive;
        public bool TdTabIsActive
        {
            get => tdTabIsActive;
            set => SetProperty(ref tdTabIsActive, value);
        }

        private bool lamaTabIsActive;
        public bool LamaTabIsActive
        {
            get => lamaTabIsActive;
            set => SetProperty(ref lamaTabIsActive, value);
        }

        private EngineSettings engineSettings;
        public EngineSettings EngineSettings
        {
            get => engineSettings;
            private set => SetProperty(ref engineSettings, value);
        }

        private MetDownloaderSettings metDownloaderSettings;
        public MetDownloaderSettings MetDownloaderSettings
        {
            get => metDownloaderSettings;
            private set => SetProperty(ref metDownloaderSettings, value);
        }

        private LamaSettings lamaSettings;
        public LamaSettings LamaSettings
        {
            get => lamaSettings;
            private set => SetProperty(ref lamaSettings, value);
        }

        private TechDataSettings tdDownloaderSettings;
        public TechDataSettings TdDownloaderSettings
        {
            get => tdDownloaderSettings;
            private set => SetProperty(ref tdDownloaderSettings, value);
        }

        private AbSettings abSettings;
        private object renameRowSelectedItem;

        public AbSettings AbSettings
        {
            get => abSettings;
            set => SetProperty(ref abSettings, value);
        }


        public SettingsViewModel()
        {
            RemoveSelectedRenameRow =
                new RelayCommand(() => RemoveSelectedRenameRowAction());
            AddNewRenameRow
                = new RelayCommand(() => AddNewRenameRowAction());

            LoadRenameTable();
            CopyFromSettings();
            hiddingTask = new Task(HideInfoAfter);
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

        public ObservableCollection<EditableDictionaryKey<string, string>> RenameMappings { get; } = new();

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
            using var storage = new StorageService(context);
            storage.MakeSureDbCreated();

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
                    System.Windows.MessageBox.Show(
                        "Nie można zapisać. Któraś wartość w pierwszej tabeli wystepuje więcej niż jeden raz.");
                }
            }

            SavedInfo = Visibility.Visible;

            SettingsIo.SaveSettings();

            hiddingTask = new Task(HideInfoAfter);
            hiddingTask.Start();

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
