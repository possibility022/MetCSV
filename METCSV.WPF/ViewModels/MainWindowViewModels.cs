using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModels : BindableBase
    {

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private IDownloader _abDownloader;
        private IDownloader _lamaDownloader;
        private IDownloader _metDownloader;
        private IDownloader _techDataDownloader;

        private IProductReader _metProductReader;
        private IProductReader _lamaProductReader;
        private IProductReader _abProductReader;
        private IProductReader _techdataProductReader;

        public MainWindowViewModels()
        {

        }


        public async Task DoIt()
        {
            Task<bool> downloadingTask = new Task<bool>(DownloadAllFiles);
            downloadingTask.Start();

            var downloadingResult = await downloadingTask;
            
            if (!downloadingResult)
                return;

            CreateReaders();

            var productsMet = await ReadFile(_metProductReader);
            var productsAb = await ReadFile(_abProductReader);
            var productsTechData = await ReadFile(_techdataProductReader);
            var productsLama = await ReadFile(_lamaProductReader);
        }

        private bool DownloadAllFiles()
        {
            _abDownloader = new AbDownloader(_cancellationTokenSource.Token);
            _lamaDownloader = new LamaDownloader();
            _metDownloader = new MetDownloader();
            _techDataDownloader = new TechDataDownloader();

            Task t1 = Task.Run(() => Download(_abDownloader));
            Task t2 = Task.Run(() => Download(_lamaDownloader));
            Task t3 = Task.Run(() => Download(_metDownloader));
            Task t4 = Task.Run(() => Download(_techDataDownloader));

            Task.WaitAll(t1, t2, t3, t4);

            return _metDownloader.Status != OperationStatus.Faild 
                && _abDownloader.Status != OperationStatus.Faild 
                && _lamaDownloader.Status != OperationStatus.Faild 
                && _techDataDownloader.Status != OperationStatus.Faild;
        }

        private OperationStatus Download(IDownloader downloader)
        {
            downloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;
            downloader.StartDownloading();
            // ReSharper disable once DelegateSubtraction
            downloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;
            return downloader.Status;
        }

        private void CreateReaders()
        {
            _metProductReader = new MetProductReader();
            _lamaProductReader = new LamaProductReader();
            _abProductReader = new AbProductReader();
            _techdataProductReader = new TechDataProductReader();
        }

        private async Task<IEnumerable<Product>> ReadFile(IProductReader productReader, string filename1, string filename2)
        {
            productReader.OnStatusChanged += OnStatusChanged;
            Task<IEnumerable<Product>> task = new Task<IEnumerable<Product>>(() => productReader.GetProducts(filename1, filename2));
            return await task;
        }

        private void OnStatusChanged(object sender, EventArgs eventArgs)
        {
            //todo implement
        }

        private void OnDownloadingStatusChanged(object sender, EventArgs eventArgs)
        {
            //todo implement?
        }
    }
}
