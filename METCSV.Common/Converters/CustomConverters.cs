using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace METCSV.Common.Converters
{
    public static class CustomConverters
    {
        public static ConcurrentDictionary<T, IList<T2>> ConvertToDictionaryOfLists<T, T2>(IEnumerable<T2> products, Func<T2, T> key)
        {
            Dictionary<T, IList<T2>> newDictionary = new Dictionary<T, IList<T2>>();
            
            foreach(var p in products)
            {
                var keyValue = key.Invoke(p);
                if (newDictionary.ContainsKey(keyValue))
                {
                    newDictionary[keyValue].Add(p);
                }
                else
                {
                    newDictionary.Add(keyValue, new List<T2> { p });
                }
            }

            return new ConcurrentDictionary<T, IList<T2>>(newDictionary);
        }
    }
}
