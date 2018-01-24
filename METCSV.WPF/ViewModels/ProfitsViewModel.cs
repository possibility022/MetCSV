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

        public ObservableCollection<EditableDictionaryKey<string, double>> Profits { get => _profits; set => SetProperty(ref _profits, value); }

        public string InfoText { get => _infoText; set => SetProperty(ref _infoText, $"Edytujesz marże dla: {value}."); }

        public ProfitsViewModel()
        {
            Profits = new ObservableCollection<EditableDictionaryKey<string, double>>
            {
                new EditableDictionaryKey<string, double>("Kategoria A", 2.0),
                new EditableDictionaryKey<string, double>("Kategoria B", 3.0)
            };
        }

    }
}
