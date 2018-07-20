using MET.Domain; using MET.Workflows;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace METCSV.WPF.Helpers
{
    static class HelpMe
    {
        public static Task<ManufacturersCollection> GetProvidersAsync(IProductProvider productProvider)
        {

            Task<ManufacturersCollection> task = new Task<ManufacturersCollection>(() => GetProviders(productProvider));
            task.Start();
            return task;
        }

        public static ManufacturersCollection GetProviders(IProductProvider productProvider)
        {
            HashSet<string> providers = new HashSet<string>();
            foreach (var product in productProvider.GetProducts())
            {
                providers.Add(product.NazwaProducenta);
            }

            return new ManufacturersCollection(productProvider.Provider, providers);
        }

        public static void CalculatePrices(IEnumerable<Product> products, Profits profits)
        {
            foreach (var p in products)
            {
                double profit = profits.Values.ContainsKey(p.NazwaProducenta) ?
                    profits.Values[p.NazwaProducenta]
                    : profits.DefaultProfit;

                p.SetCennaNetto(p.CenaZakupuNetto + (p.CenaZakupuNetto * profit));
            }
        }

        public static Task CalculatePricesInBackground(IEnumerable<Product> products, Profits profits)
        {
            Task t = new Task(() => CalculatePrices(products, profits));
            t.Start();
            return t;
        }
    }
}
