using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.Common;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Interfaces
{
    interface IProductReader
    {
        IList<Product> GetProducts(string filename, string filename2);

        OperationStatus Status { get; }

        EventHandler<OperationStatus> OnStatusChanged { get; set; }

        string ProviderName { get; }

        Providers Provider { get; }

        void SetCancellationToken(CancellationToken token);
    }
}
