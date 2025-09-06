using METCSV.WPF.ProductProvider;
using System.Collections.ObjectModel;

namespace METCSV.WPF.Helpers
{
    static class CustomConvert
    {
        public static ObservableCollection<EditableDictionaryKey<T1, T2>> ToObservableCollection<T1,T2>(IReadOnlyDictionary<T1,T2> dictionary)
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
