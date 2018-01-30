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
        public static HashSet<string> GetProviders(IEnumerable<Product> products)
        {
            HashSet<string> providers = new HashSet<string>();
            foreach(var product in products)
            {
                providers.Add(product.NazwaProducenta);
            }

            return providers;
        }
    }
}
