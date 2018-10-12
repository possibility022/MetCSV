using System;

namespace METCSV.Common.Formatters
{
    public interface IObjectFormatter<T>
    {
        void WriteLine(string message);

        void WriteLine(T item);

        void Flush();

        void SetFlushAction(Action<string> action);
    }
}
