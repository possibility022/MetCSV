namespace METCSV.Common.Formatters
{
    public interface IObjectFormatter<T> : IStringFormatter
    {

        void WriteObject(T item);
    }
}
