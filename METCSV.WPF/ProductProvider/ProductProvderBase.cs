using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using Prism.Mvvm;

namespace METCSV.WPF.ProductProvider
{
    class ProductProviderBase : BindableBase, IProductProvider
    {

        private IDownloader _downloader;
        private IProductReader _productReader;
        private OperationStatus _downloaderStatus;
        private OperationStatus _readerStatus;

        protected CancellationToken _token;

        public IEnumerable<Product> GetProducts()
        {
            DownloadData();
            return ReadFile(_productReader, _downloader);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            _token = token;
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
        
        private IEnumerable<Product> ReadFile(IProductReader productReader, IDownloader downloader)
        {
            productReader.OnStatusChanged += OnProductReaderStatusChanged;

            if (downloader.Status != OperationStatus.Complete)
            {
                throw new InvalidOperationException("Nie mozna rozpoczac czytania pliku jesli pobieranie nie powiodlo sie.");
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

    }
}



