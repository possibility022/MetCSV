using METCSV.WPF.Helpers;
using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.ViewModels
{
    internal class ProfitsViewModel : BindableBase
    {
        private ObservableCollection<EditableDictionaryKey<string, double>> _profits;
        private string _infoText = "InfoText";
        private string _selectedProfits;
        private string _profitsCollections;

        public ObservableCollection<EditableDictionaryKey<string, double>> Profits
        {
            get => _profits;
            set => SetProperty(ref _profits, value);
        }

        Dictionary<string, ObservableCollection<EditableDictionaryKey<string, double>>> _allProfitsCollections;

        public string InfoText
        {
            get => $"Edytujesz marże dla: {SelectedProfits}.";
        }

        public string SelectedProfits
        {
            get => _selectedProfits;
            set
            {
                SetProperty(ref _selectedProfits, value);
                RaisePropertyChanged(nameof(InfoText));
            }
        }

        public IEnumerable<string> ProfitsCollections
        {
            get => _allProfitsCollections.Keys;
        }

        public ProfitsViewModel()
        {
            _allProfitsCollections = new Dictionary<string, ObservableCollection<EditableDictionaryKey<string, double>>>();
        }

        public void AddProfitsCollection(string provider, IDictionary<string, double> profits)
        {
            _allProfitsCollections.Add(provider, Converters.ToObservableCollection(profits));
            RaisePropertyChanged(nameof(ProfitsCollections));
        }

    }
}
