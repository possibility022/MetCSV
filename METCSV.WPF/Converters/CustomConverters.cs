using METCSV.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace METCSV.WPF.Converters
{
    public static class CustomConverters
    {
        public static ConcurrentDictionary<string, IList<Product>> ConvertToDictionaryOfLists(ConcurrentBag<Product> products)
        {
            Dictionary<string, IList<Product>> newDictionary = new Dictionary<string, IList<Product>>();

            Product p = null;

            while (products.TryTake(out p) || products.Count > 0)
            {
                if (newDictionary.ContainsKey(p.SymbolSAP))
                {
                    newDictionary[p.SymbolSAP].Add(p);
                }
                else
                {
                    newDictionary.Add(p.SymbolSAP, new List<Product> { p });
                }
            }

            return new ConcurrentDictionary<string, IList<Product>>(newDictionary);
        }
    }
}
