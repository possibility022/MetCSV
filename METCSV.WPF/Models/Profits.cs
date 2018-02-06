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
        private double _defaultProfit = 0.1;
        private HashSet<string> _allProviders;
        private Dictionary<string, double> _values;

        public string Provider { get; }

        public IReadOnlyDictionary<string, double> Values { get => _values; }

        public double DefaultProfit { get => _defaultProfit; set => SetProperty(ref _defaultProfit, value); }

        public Profits(string provider)
        {
            Provider = provider;
            RaisePropertyChanged(nameof(Provider));
        }

        /// <summary>
        /// Sets the new profits.
        /// </summary>
        /// <param name="newValues">The new values.</param>
        public void SetNewProfits(IEnumerable<EditableDictionaryKey<string, double>> newValues)
        {
            if (_values == null)
                _values = new Dictionary<string, double>();

            foreach(var newValue in newValues)
            {
                if (_values.ContainsKey(newValue.Key) == false)
                {
                    _values.Add(newValue.Key, newValue.Value);
                }
                else
                {
                    _values[newValue.Key] = newValue.Value;
                }
            }
        }

        /// <summary>
        /// Adds the manufacturer with default values if does not exists in collection.
        /// </summary>
        /// <param name="manufacturers">The manufacturers.</param>
        public void AddManufacturers(IEnumerable<string> manufacturers)
        {
            if (_values == null)
                _values = new Dictionary<string, double>();

            foreach(var manu in manufacturers)
            {
                if (_values.ContainsKey(manu) == false)
                {
                    _values.Add(manu, DefaultProfit);
                }
            }
        }
    }
}
