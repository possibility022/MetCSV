using System;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductReaders;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModels : BindableBase
    {

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private IDownloader _metDownloader;
        private IDownloader _lamaDownloader;
        private IDownloader _abDownloader;
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
            var downloadingResult = await Download();
            
            if (!downloadingResult)
                return;
            
        }

        private async Task<bool> Download()
        {
            _abDownloader = new AbDownloader(_cancellationTokenSource.Token);
            _lamaDownloader = new LamaDownloader();
            _metDownloader = new MetDownloader();
            _techDataDownloader = new TechDataDownloader();

            _abDownloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;
            _lamaDownloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;
            _metDownloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;
            _techDataDownloader.OnDownloadingStatusChanged += OnDownloadingStatusChanged;

            _metDownloader.StartDownloading();
            _lamaDownloader.StartDownloading();
            _abDownloader.StartDownloading();
            _techDataDownloader.StartDownloading();

            Task metDownloaderTask = new Task(() => _metDownloader.StartDownloading());
            Task lamaDownloaderTask = new Task(() => _lamaDownloader.StartDownloading());
            Task abDownloaderTask = new Task(() => _abDownloader.StartDownloading());
            Task techDataDownloaderTask = new Task(() => _techDataDownloader.StartDownloading());

            metDownloaderTask.Start();
            lamaDownloaderTask.Start();
            abDownloaderTask.Start();
            techDataDownloaderTask.Start();

            await metDownloaderTask;
            await lamaDownloaderTask;
            await abDownloaderTask;
            await techDataDownloaderTask;

            // ReSharper disable once DelegateSubtraction
            _abDownloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;
            // ReSharper disable once DelegateSubtraction
            _lamaDownloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;
            // ReSharper disable once DelegateSubtraction
            _metDownloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;
            // ReSharper disable once DelegateSubtraction
            _techDataDownloader.OnDownloadingStatusChanged -= OnDownloadingStatusChanged;


            return _metDownloader.Status != OperationStatus.Faild 
                && _abDownloader.Status != OperationStatus.Faild 
                && _lamaDownloader.Status != OperationStatus.Faild 
                && _techDataDownloader.Status != OperationStatus.Faild;
        }

        private async Task<bool> ReadAllFiles()
        {
            _metProductReader = new MetProductReader();
            _lamaProductReader = new LamaProductReader();
            _abProductReader = new AbProductReader();
            _techdataProductReader = new TechDataProductReader();

            _metProductReader.OnStatusChanged += OnStatusChanged;
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
