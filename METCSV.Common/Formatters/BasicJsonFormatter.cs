using Newtonsoft.Json;
using System;
using System.Text;

namespace METCSV.Common.Formatters
{
    public class BasicJsonFormatter<T> : IObjectFormatter<T>, IObjectFormatterConstructor<T>
    {

        StringBuilder sb = new StringBuilder();
        private Action<string> _action;

        public BasicJsonFormatter()
        {
            _action = Log.LogProductInfo;
        }

        public void Flush()
        {
            _action.Invoke(sb.ToString());
            sb.Clear();
        }

        public IObjectFormatter<T> GetNewInstance()
        {
            var obj = new BasicJsonFormatter<T>();
            obj.SetFlushAction(_action);

            return obj;
        }

        public void SetFlushAction(Action<string> action)
        {
            _action = action;
        }

        public void WriteLine(string message)
        {
            sb.AppendLine(message);
        }

        public void WriteObject(T item)
        {
            sb.AppendLine(JsonConvert.SerializeObject(item, Formatting.Indented));
        }
    }
}
