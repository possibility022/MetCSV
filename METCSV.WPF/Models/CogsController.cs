using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using Prism.Mvvm;

namespace METCSV.WPF.Models
{
    class CogsController : BindableBase
    {

        private IProductProvider _provider;

        private int _downloadInProgress;
        public int DownloadInProgress
        {
            get { return _downloadInProgress; }
            set { SetProperty(ref _downloadInProgress, value); }
        }

        private int _readingInProgress;
        public int ReadingInProgress
        {
            get { return _readingInProgress; }
            set { SetProperty(ref _readingInProgress, value); }
        }

        public void SetProvider(IProductProvider provider)
        {
            _provider = provider;
            _provider.PropertyChanged += Provider_PropertyChanged;
        }

        private void Provider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DownloadInProgress = GetStatus(_provider.DownloaderStatus);
            ReadingInProgress = GetStatus(_provider.ReaderStatus);
        }

        private int GetStatus(OperationStatus status)
        {
            switch (status)
            {
                case OperationStatus.InProgress:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}
