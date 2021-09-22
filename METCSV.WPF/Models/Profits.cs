using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;

namespace METCSV.WPF.Models
{
    public class Profits : BindableBase
    {
        private double defaultProfit = 0.1;
        private Dictionary<string, double> values = new();

        public Providers Provider { get; }

        public IReadOnlyDictionary<string, double> Values { get => values; }

        public double DefaultProfit { get => defaultProfit; set => SetProperty(ref defaultProfit, value); }
        
        public Profits(Providers provider) : this(provider, 0.1)
        {
        }

        public Profits(Providers provider, double defaultProfit)
        {
            DefaultProfit = defaultProfit;
            Provider = provider;
            RaisePropertyChanged(nameof(Provider));
        }

        /// <summary>
        /// Sets the new profits.
        /// </summary>
        /// <param name="newValues">The new values.</param>
        public void SetNewProfits(IEnumerable<EditableDictionaryKey<string, double>> newValues)
        {
            SetProfit(newValues.Select(r => new KeyValuePair<string,double>(r.Key, r.Value)));
        }

        public void SetNewProfit(string key, double profitValue)
        {
            if (profitValue == defaultProfit)
            {
                if (values.ContainsKey(key))
                {
                    values.Remove(key);
                }
                return;
            }

            if (values.ContainsKey(key) == false)
            {
                values.Add(key, profitValue);
            }
            else
            {
                values[key] = profitValue;
            }
        }

        /// <summary>
        /// Sets the new profits.
        /// </summary>
        /// <param name="newValues">The new values.</param>
        public void SetNewProfits(Dictionary<string, double> newValues)
        {
            SetProfit(newValues.Select(r => new KeyValuePair<string, double>(r.Key, r.Value)));
        }

        private void SetProfit(IEnumerable<KeyValuePair<string, double>> newValues)
        {
            foreach (var newValue in newValues)
            {
                SetNewProfit(newValue.Key, newValue.Value);
            }
        }

        /// <summary>
        /// Adds the manufacturer with default values if does not exists in collection.
        /// </summary>
        /// <param name="manufacturers">The manufacturers.</param>
        public void AddManufacturers(IEnumerable<string> manufacturers)
        {
            if (values == null)
                values = new Dictionary<string, double>();

            foreach (var manu in manufacturers)
            {
                if (values.ContainsKey(manu) == false)
                {
                    values.Add(manu, DefaultProfit);
                }
            }
        }
    }
}
