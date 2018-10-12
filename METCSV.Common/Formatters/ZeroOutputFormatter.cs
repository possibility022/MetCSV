using System;

namespace METCSV.Common.Formatters
{
    public class ZeroOutputFormatter : IObjectFormatter<object>, IObjectFormatterConstructor<object>
    {
        public void Flush()
        {

        }

        public IObjectFormatter<object> GetNewInstance()
        {
            return this;
        }

        public void SetFlushAction(Action<string> action)
        {

        }

        public void WriteLine(string message)
        {

        }

        public void WriteLine(object item)
        {

        }
    }
}
