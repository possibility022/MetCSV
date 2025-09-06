using System;
using METCSV.Common.Formatters;

namespace METCSV.Tests.EngineTest;

public class ZeroOutputFormatter : IObjectFormatter<object>, IObjectFormatterConstructor<object>
{
    public static ZeroOutputFormatter Instance = new ZeroOutputFormatter();

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

    public void WriteObject(object item)
    {
    }
}