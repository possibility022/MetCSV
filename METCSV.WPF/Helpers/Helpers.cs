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
        public async static Task<HashSet<string>> GetProvidersAsync(IEnumerable<Product> products)
        {
            Task<HashSet<string>> task = new Task<HashSet<string>>(() => GetProviders(products));
            return await task;
        }

        public static HashSet<string> GetProviders(IEnumerable<Product> products)
        {
            HashSet<string> providers = new HashSet<string>();
            foreach (var product in products)
            {
                providers.Add(product.NazwaProducenta);
            }

            return providers;
        }
    }
}
