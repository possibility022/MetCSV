using METCSV.WPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Proxy.ProductProvider;
using METCSV.WPF.ViewModels.ProfitsInnerModels;

namespace METCSV.WPF.Helpers
{
    static class HelpMe
    {
        public static Task<ManufacturersCollection> GetManufacturersAsync(IProductProvider productProvider)
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

        public static Task<CategoryCollection> GetCategoriesCollectionAsync(IProductProvider productProvider)
        {
            Task<CategoryCollection>
                task = new Task<CategoryCollection>(() => GetCategoriesCollection(productProvider));
            task.Start();

            return task;
        }

        public static CategoryCollection GetCategoriesCollection(IProductProvider productProvider)
        {
            var categories = new HashSet<string>();
            foreach (var product in productProvider.GetProducts())
            {
                if (!string.IsNullOrEmpty(product.Kategoria))
                    categories.Add(product.Kategoria);
            }

            return new CategoryCollection(productProvider.Provider, categories);
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
    }
}
