using METCSV.WPF.Helpers;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace METCSV.WPF.ViewModels
{
    internal class ProfitsViewModel : BindableBase
    {
        private ObservableCollection<Profits> _profitsCollection;
        private Profits _selectedProfits;
        private ObservableCollection<EditableDictionaryKey<string, double>> _values;

        public ObservableCollection<Profits> ProfitsCollections
        {
            get => _profitsCollection;
            set => SetProperty(ref _profitsCollection, value);
        }

        public string InfoText
        {
            get => $"Edytujesz marże dla: {SelectedProfits.Provider}.";
        }

        public Profits SelectedProfits
        {
            get => _selectedProfits;
            set
            {
                SaveCurrentProfits();
                SetProperty(ref _selectedProfits, value);
                // Cloning value from <Profits> in to ObservableCollection
                Values = new ObservableCollection<EditableDictionaryKey<string, double>>(Converters.ToObservableCollection(ProfitsCollections.First(t => t.Equals(value)).Values));
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
        }

        public void AddProfitsCollection(Profits profits)
        {
            ProfitsCollections.Add(profits);
            RaisePropertyChanged(nameof(ProfitsCollections));
        }

        private void SaveCurrentProfits()
        {
            if (Values != null && SelectedProfits != null)
            {
                SelectedProfits.SetNewProfits(Values);
            }
        }

    }
}
