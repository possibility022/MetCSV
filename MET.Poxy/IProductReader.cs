using System;
using System.Collections.Generic;
using System.Threading;
using MET.Domain;
using METCSV.Common;

namespace MET.Proxy.Interfaces
{
    public interface IProductReader
    {
        IList<Product> GetProducts(string filename, string filename2);

        OperationStatus Status { get; }

        EventHandler<OperationStatus> OnStatusChanged { get; set; }

        string ProviderName { get; }

        string SapPrefix { get; }

        Providers Provider { get; }

        void SetCancellationToken(CancellationToken token);
    }
}
