using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using METCSV.WPF.Helpers;
using METCSV.WPF.ProductProvider;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels.ProfitsInnerModels
{
    public class ProfitsTabInnerViewModel : BindableBase
    {
        private readonly Dictionary<Profits, ObservableCollection<EditableDictionaryKey<string, double>>> valuesCache = new();


        private ObservableCollection<Profits> profitsCollection = new ObservableCollection<Profits>();
        public ObservableCollection<Profits> ProfitsCollections
        {
            get => profitsCollection;
            set => SetProperty(ref profitsCollection, value);
        }


        private Profits selectedProfits;
        public Profits SelectedProfits
        {
            get => selectedProfits;
            set
            {
                SaveCurrentProfits();
                SetProperty(ref selectedProfits, value);
                if (value != null)
                {
                    CacheValues();
                    Values = valuesCache[value];
                }
                RaisePropertyChanged(nameof(InfoText));
            }
        }

        public string InfoText
        {
            get => $"Edytujesz marże dla: {SelectedProfits?.Provider}.";
        }

        private ObservableCollection<EditableDictionaryKey<string, double>> values = new();
        public ObservableCollection<EditableDictionaryKey<string, double>> Values
        {
            get => values;
            set => SetProperty(ref values, value);
        }

        public void SaveCurrentProfits()
        {
            if (Values != null && SelectedProfits != null)
            {
                SelectedProfits.SetNewProfits(Values);
            }
        }

        private void CacheValues()
        {
            if (SelectedProfits != null && valuesCache.ContainsKey(SelectedProfits) == false)
                valuesCache.Add(SelectedProfits, CustomConvert.ToObservableCollection(SelectedProfits.Values));
        }
    }
}
