using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace METCSV.Common.Formatters
{
    public class BasicJsonFormatter<T> : IObjectFormatter<T>
    {
        public void Get(StringBuilder sb, IEnumerable<T> item)
        {
            sb.AppendLine(JsonConvert.SerializeObject(item, Formatting.Indented));
        }

        public void Get(StringBuilder sb, T item)
        {
            sb.AppendLine(JsonConvert.SerializeObject(item, Formatting.Indented));
        }
    }
}
