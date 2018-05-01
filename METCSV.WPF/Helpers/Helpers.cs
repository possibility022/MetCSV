using METCSV.Common;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Helpers
{
    sealed class HelpMe
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
            var products = productProvider.GetProducts();
            foreach (var product in products)
            {
                providers.Add(product.NazwaProducenta);
            }

            ManufacturersCollection all = new ManufacturersCollection(productProvider.Provider, providers);

            return all;
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
