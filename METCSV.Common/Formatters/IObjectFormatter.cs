using System.Collections.Generic;
using System.Text;

namespace METCSV.Common.Formatters
{
    public interface IObjectFormatter<T>
    {
        void Get(StringBuilder sb, IEnumerable<T> item);

        void Get(StringBuilder sb, T item);
    }
}
