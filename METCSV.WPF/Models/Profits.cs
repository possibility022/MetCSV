using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Models
{
    public class Profits : BindableBase
    {
        private double _defaultProfit;
        private HashSet<string> _allProviders;
        private Dictionary<string, double> _values;

        public string Provider { get; }

        public IReadOnlyDictionary<string, double> Values { get => _values; }

        public HashSet<string> AllProviders { get => _allProviders; set => SetProperty(ref _allProviders, value); }

        public double DefaultProfit { get => _defaultProfit; set => SetProperty(ref _defaultProfit, value); }

        public Profits(string provider)
        {
            Provider = provider;
            RaisePropertyChanged(nameof(Provider));
        }

        public void SetNewProfits(IEnumerable<EditableDictionaryKey<string, double>> newValues)
        {
            Dictionary<string, double> values = new Dictionary<string, double>();
            foreach (var val in newValues)
            {
                values.Add(val.Key, val.Value);
            }

            _values = values;
        }
    }
}
