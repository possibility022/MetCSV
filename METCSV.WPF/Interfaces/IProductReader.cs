using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.WPF.Enums;
using METCSV.WPF.ProductProvider;

namespace METCSV.WPF.Interfaces
{
    interface IProductReader
    {
        IEnumerable<Product> GetProducts(string filename, string filename2);

        OperationStatus Status { get; }

        EventHandler<OperationStatus> OnStatusChanged { get; set; }

        string ProviderName { get; }

        void SetCancellationToken(CancellationToken token);
    }
}
