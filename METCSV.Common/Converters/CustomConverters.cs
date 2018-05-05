using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace METCSV.Common.Converters
{
    public static class CustomConverters
    {
        public static ConcurrentDictionary<T, IList<Product>> ConvertToDictionaryOfLists<T>(IEnumerable<Product> products, Func<Product, T> key)
        {
            Dictionary<T, IList<Product>> newDictionary = new Dictionary<T, IList<Product>>();
            
            foreach(var p in products)
            {
                var keyValue = key.Invoke(p);
                if (newDictionary.ContainsKey(keyValue))
                {
                    newDictionary[keyValue].Add(p);
                }
                else
                {
                    newDictionary.Add(keyValue, new List<Product> { p });
                }
            }

            return new ConcurrentDictionary<T, IList<Product>>(newDictionary);
        }
    }
}
