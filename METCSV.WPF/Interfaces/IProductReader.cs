using System;
using System.Collections.Generic;
using METCSV.WPF.Enums;
using METCSV.WPF.Models;

namespace METCSV.WPF.Interfaces
{
    interface IProductReader
    {
        IEnumerable<Product> GetProducts(string filename, string filename2);

        OperationStatus Status { get; }

        EventHandler OnStatusChanged { get; set; }

        string ProviderName { get; }
    }
}
