using System;
using System.Collections.Generic;
using System.Threading;
using MET.Domain;
using METCSV.Common;
using MET.Proxy.Interfaces;
using METCSV.Common.Exceptions;

namespace MET.Proxy.ProductReaders
{
    public abstract class ProductReaderBase : IProductReader
    {
        private OperationStatus _status;

        protected CancellationToken _token;

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

        public abstract Providers Provider { get; }

        public ProductReaderBase(CancellationToken token)
        {
            _token = token;
        }

        protected void LogError(Exception ex, string message)
        {
            Log.Error(ex, FormatMessage(message));
        }

        protected void LogError(string message)
        {
            Log.Error(FormatMessage(message));
        }

        protected void LogInfo(string message)
        {
            Log.Info(FormatMessage(message));
        }

        private string FormatMessage(string message)
        {
            return $"[{Provider}] - {message}";
        }

        protected void ThrowIfCanceled()
        {
            if (_token.IsCancellationRequested)
                throw new CancelledException();
        }
    }
}
