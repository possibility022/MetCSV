using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace METCSV.Common.Formatters
{
    public class BasicJsonFormatter<T> : IObjectFormatter<T>
    {
        public void Get(StringBuilder sb, IEnumerable<T> items)
        {
            sb.AppendLine(Get(items));
        }

        public void Get(StringBuilder sb, T item)
        {
            sb.AppendLine(Get(item));
        }

        public string Get(T item) 
            => JsonConvert.SerializeObject(item, Formatting.Indented);


        public string Get(IEnumerable<T> items) 
            => JsonConvert.SerializeObject(items, Formatting.Indented);
    }
}
