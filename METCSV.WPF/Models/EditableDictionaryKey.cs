namespace METCSV.WPF.ProductProvider
{
    public class EditableDictionaryKey<T1, T2> : BindableBase
    {
        private T1 key;
        private T2 value;

        public EditableDictionaryKey(T1 key, T2 value)
        {
            this.key = key;
            this.value = value;
        }

        public T1 Key { get => key; set => SetProperty(ref key, value); }

        public T2 Value { get => value; set => SetProperty(ref this.value, value); }
    }
}
