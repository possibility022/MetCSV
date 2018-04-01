using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.Common;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;

namespace METCSV.WPF.ProductReaders
{
    abstract class ProductReaderBase : IProductReader
    {
        private OperationStatus _status;

        private CancellationToken _token;

        public abstract IList<Product> GetProducts(string filename, string filename2);

        public OperationStatus Status
        {
            get { return _status; }
            protected set
            {
                if (value != _status)
                {
                    _status = value;
                    OnStatusChanged?.Invoke(this, _status);
                }
            }
        }

        public EventHandler<OperationStatus> OnStatusChanged { get; set; }

        public string ProviderName { get; protected set; }

        public void SetCancellationToken(CancellationToken token)
        {
            _token = token;
        }
    }
}
