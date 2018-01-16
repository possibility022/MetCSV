using System;
using System.Collections.Generic;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;

namespace METCSV.WPF.ProductReaders
{
    abstract class ProductReaderBase : IProductReader
    {
        private OperationStatus _status;

        public abstract IEnumerable<Product> GetProducts(string filename, string filename2);

        public OperationStatus Status
        {
            get { return _status; }
            protected set
            {
                if (value != _status)
                {
                    _status = value;
                    OnStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public EventHandler OnStatusChanged { get; set; }
        public string ProviderName { get; protected set; }
    }
}
