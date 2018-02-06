using METCSV.WPF.Helpers;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.Workflows;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace METCSV.WPF.ViewModels
{
    internal class ProfitsViewModel : BindableBase
    {
        private ObservableCollection<Profits> _profitsCollection;
        private Profits _selectedProfits;
        private ObservableCollection<EditableDictionaryKey<string, double>> _values;
        private string _errorText;
        private Visibility _errorTextVisibility;

        private Dictionary<Profits, ObservableCollection<EditableDictionaryKey<string, double>>> _valuesCache;

        public ObservableCollection<Profits> ProfitsCollections
        {
            get => _profitsCollection;
            set => SetProperty(ref _profitsCollection, value);
        }

        public string InfoText
        {
            get => $"Edytujesz marże dla: {SelectedProfits?.Provider}.";
        }

        public string ErrorText { get => _errorText; private set => SetProperty(ref _errorText, value); }

        public Visibility ErrorTextVisibility { get => _errorTextVisibility; set => SetProperty(ref _errorTextVisibility, value); }

        public Profits SelectedProfits
        {
            get => _selectedProfits;
            set
            {
                SaveCurrentProfits();
                SetProperty(ref _selectedProfits, value);
                CacheValues();
                Values = _valuesCache[value];
                RaisePropertyChanged(nameof(InfoText));
            }
        }

        public ObservableCollection<EditableDictionaryKey<string, double>> Values
        {
            get => _values;
            set => SetProperty(ref _values, value);
        }

        public ProfitsViewModel()
        {
            _profitsCollection = new ObservableCollection<Profits>();
            _errorTextVisibility = Visibility.Hidden;
            _valuesCache = new Dictionary<Profits, ObservableCollection<EditableDictionaryKey<string, double>>>();
        }

        public void AddProfitsCollection(Profits profits)
        {
            ProfitsCollections.Add(profits);
            RaisePropertyChanged(nameof(ProfitsCollections));
        }

        public void CacheValues()
        {
            if (_valuesCache.ContainsKey(SelectedProfits) == false)
                _valuesCache.Add(SelectedProfits, Converters.ToObservableCollection(SelectedProfits.Values));
        }

        private void SaveCurrentProfits()
        {
            if (Values != null && SelectedProfits != null)
            {
                SelectedProfits.SetNewProfits(Values);
            }
        }

        public void SaveAllProfits()
        {
            foreach(var val in _valuesCache)
            {
                val.Key.SetNewProfits(val.Value);
            }
        }

        public void OnWindowLoaded()
        {
            LoadFromFiles();
        }

        public void LoadFromFiles()
        {
            string message = string.Empty;
            var loadingDoneWithoutErrors = LoadFromFiles(out message);

            if (loadingDoneWithoutErrors == false)
            {
                ErrorText = message;
                ErrorTextVisibility = Visibility.Visible;
            }
        }

        private bool LoadFromFiles(out string message)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var provider in App.Providers)
            {
                var file = $"{provider}{App.ProfitsFileExtension}";

                if (File.Exists(file))
                {
                    ProfitsCollections.Add(ProfitsIO.LoadFromFile(file));
                }
                else
                {
                    sb.AppendLine($@"Nie znalazłem pliku z zapisanymi marżami dla dostawcy: {provider}");
                    sb.AppendLine($@"Oczekuję pliku tutaj: {Path.GetFullPath(file)}");
                }
            }

            if (sb.Length > 0)
            {
                sb.AppendLine(@"Możesz wprowadzić nowe wartości lub spróbować wkleić plik do odpowiedniego folderu. Jeśli program będzie kontunuował to użyje on domyślnych wartości marży.");
            }

            message = sb.ToString();

            return sb.Length == 0;
        }

    }
}
