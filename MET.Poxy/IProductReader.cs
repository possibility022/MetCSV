using System.Collections.Generic;
using MET.Data.Models;
using METCSV.Common;

namespace MET.Proxy.Interfaces
{
    public interface IProductReader
    {
        IList<Product> GetProducts(string filename, string filename2);

        OperationStatus Status { get; }

        string ProviderName { get; }

        Providers Provider { get; }
    }
}
