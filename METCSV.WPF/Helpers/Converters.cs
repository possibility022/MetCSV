﻿using METCSV.WPF.ProductProvider;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace METCSV.WPF.Helpers
{
    static class CustomConvert
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

        static public ObservableCollection<EditableDictionaryKey<T1, T2>> ToObservableCollection<T1,T2>(IReadOnlyDictionary<T1,T2> dictionary)
        {
            ObservableCollection<EditableDictionaryKey<T1, T2>>  collection = new ObservableCollection<EditableDictionaryKey<T1, T2>>();
            foreach(var pair in dictionary)
            {
                collection.Add(new EditableDictionaryKey<T1, T2>(pair.Key, pair.Value));
            }

            return collection;
        }
    }
}
