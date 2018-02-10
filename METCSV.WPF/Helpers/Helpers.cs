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
    }
}
