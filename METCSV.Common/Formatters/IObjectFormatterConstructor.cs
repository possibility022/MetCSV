namespace METCSV.Common.Formatters
{
    public interface IObjectFormatterConstructor<T>
    {
        IObjectFormatter<T> GetNewInstance();
    }
}
