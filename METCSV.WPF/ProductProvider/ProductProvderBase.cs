using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MET.Domain;
using MET.Proxy;
using MET.Proxy.Interfaces;
using METCSV.Common;
using METCSV.WPF.Interfaces;
using Prism.Mvvm;

namespace METCSV.WPF.ProductProvider
{
    abstract class ProductProviderBase : BindableBase, IProductProvider
    {

        protected const string ArchiveFolder = "Archive";

        protected abstract string ArchiveFileNamePrefix { get; }

        private IDownloader _downloader;
        private IProductReader _productReader;
        private OperationStatus _downloaderStatus;
        private OperationStatus _readerStatus;

        public Providers Provider { get; protected set; }

        protected CancellationToken _token;

        IList<Product> _products;

        public ProductProviderBase(CancellationToken cancellationToken)
        {
            _token = cancellationToken;
        }

        public IList<Product> GetProducts()
        {
            return _products;
        }

        public void SetProductDownloader(IDownloader downloader)
        {
            if (_downloader?.Status == OperationStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot set downloader when the old one is in progress state.");
            }

            _downloader = downloader;
        }

        public void SetProductReader(IProductReader reader)
        {
            if (_productReader?.Status == OperationStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot set reader when the old one is in progress state.");
            }

            _productReader = reader;
        }

        public OperationStatus DownloaderStatus
        {
            get => _downloaderStatus;
            private set => SetProperty(ref _downloaderStatus, value);
        }

        public OperationStatus ReaderStatus
        {
            get => _readerStatus;
            private set => SetProperty(ref _readerStatus, value);
        }

        private void DownloadData()
        {
            _downloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;
            _downloader.StartDownloading();

            // ReSharper disable once DelegateSubtraction
            _downloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;
        }
        
        private IList<Product> ReadFile(IProductReader productReader, IDownloader downloader)
        {
            productReader.OnStatusChanged += OnProductReaderStatusChanged;

            if (downloader.Status != OperationStatus.Complete)
            {
                ReaderStatus = OperationStatus.Faild;
                Log.Info("Wczytywanie przerwane ze względu na nieukończone pobieranie.");
                return new List<Product>();
            }

            string filename2 =
                downloader.DownloadedFiles.Length >= 2 ?
                    downloader.DownloadedFiles[1]
                    : string.Empty;
            
            var producets = productReader.GetProducts(downloader.DownloadedFiles[0], filename2);
            
            // ReSharper disable once DelegateSubtraction
            productReader.OnStatusChanged -= OnProductReaderStatusChanged;

            return producets;
        }

        private void OnProductReaderStatusChanged(object sender, OperationStatus e)
        {
            ReaderStatus = e;
        }

        private void OnDownloadingStatusChanged(object sender, OperationStatus e)
        {
            DownloaderStatus = e;
        }

        static public Task<bool> DownloadAndLoadAsync(IProductProvider productProvider)
        {
            Task<bool> task = new Task<bool>(productProvider.DownloadAndLoad);
            task.Start();
            return task;
        }

        public bool DownloadAndLoad()
        {
            DownloadData();
            _products = ReadFile(_productReader, _downloader);
            return _productReader.Status == OperationStatus.Complete && _downloader.Status == OperationStatus.Complete;
        }

        public ICollection<Product> LoadOldProducts()
        {
            var file = Directory.GetFiles(ArchiveFolder)
                .Where(r => r.StartsWith(ArchiveFileNamePrefix))
                .Select(f => new FileInfo(f))
                .OrderByDescending(fi => fi.CreationTime)
                .FirstOrDefault();

            
        }
    }
}



