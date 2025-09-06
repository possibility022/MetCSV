using Newtonsoft.Json;
using System.Text;

namespace METCSV.Common.Formatters
{
    public class BasicJsonFormatter<T> : IObjectFormatter<T>, IObjectFormatterConstructor<T>
    {

        StringBuilder sb = new StringBuilder();
        private Action<string> action;

        public BasicJsonFormatter()
        {
            action = Log.LogProductInfo;
        }

        public void Flush()
        {
            action.Invoke(sb.ToString());
            sb.Clear();
        }

        public IObjectFormatter<T> GetNewInstance()
        {
            var obj = new BasicJsonFormatter<T>();
            obj.SetFlushAction(action);

            return obj;
        }

        private void SetFlushAction(Action<string> action)
        {
            this.action = action;
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
