using Prism.Mvvm;

namespace METCSV.WPF.ProductProvider
{
    public class EditableDictionaryKey<T1, T2> : BindableBase
    {
        private T1 _key;
        private T2 _value;

        public EditableDictionaryKey(T1 key, T2 value)
        {
            _key = key;
            _value = value;
        }

        public T1 Key { get => _key; set => SetProperty(ref _key, value); }

        public T2 Value { get => _value; set => SetProperty(ref _value, value); }
    }
}
