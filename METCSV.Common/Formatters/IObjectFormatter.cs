using System.Collections.Generic;
using System.Text;

namespace METCSV.Common.Formatters
{
    public interface IObjectFormatter<T>
    {
        void Get(StringBuilder sb, IEnumerable<T> items);

        void Get(StringBuilder sb, T item);

        string Get(T item);
        string Get(IEnumerable<T> items);
    }
}
