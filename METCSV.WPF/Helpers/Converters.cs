using METCSV.WPF.ProductProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Helpers
{
    static class Converters
    {
        static public Dictionary<T1, T2> ToDictionary<T1,T2>(IEnumerable<EditableDictionaryKey<T1,T2>> collection)
        {
            Dictionary<T1, T2> dictionary = new Dictionary<T1, T2>();
            foreach (var keyValuePair in collection)
            {
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dictionary;
        }
    }
}
