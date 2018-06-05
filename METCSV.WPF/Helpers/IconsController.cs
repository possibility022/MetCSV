using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Windows;

namespace METCSV.WPF.Helpers
{
    class IconsController : BindableBase
    {
        private IProductProvider _provider;


        #region download

        private Visibility _downloadFaildVisibility = Visibility.Hidden;
        public Visibility DownloadFaildVisibility
        {
            get { return _downloadFaildVisibility; }
            set { SetProperty(ref _downloadFaildVisibility, value); }
        }

        private Visibility _downloadInProgressVisibility = Visibility.Hidden;
        public Visibility DownloadInProgressVisibility
        {
            get { return _downloadInProgressVisibility; }
            set { SetProperty(ref _downloadInProgressVisibility, value); }
        }

        private Visibility _downloadWaitingVisibility = Visibility.Visible;

        public Visibility DownloadWaitingVisibility
        {
            get { return _downloadWaitingVisibility; }
            set { SetProperty(ref _downloadWaitingVisibility, value); }
        }

        private Visibility _downloadDoneVisibility = Visibility.Hidden;

        public Visibility DownloadDoneVisibility
        {
            get { return _downloadDoneVisibility; }
            set { SetProperty(ref _downloadDoneVisibility, value); }
        }

        #endregion

        #region read

        private Visibility _readFaildVisibility = Visibility.Hidden;
        public Visibility ReadFaildVisibility
        {
            get { return _readFaildVisibility; }
            set { SetProperty(ref _readFaildVisibility, value); }
        }

        private Visibility _readInProgressVisibility = Visibility.Hidden;
        public Visibility ReadInProgressVisibility
        {
            get { return _readInProgressVisibility; }
            set { SetProperty(ref _readInProgressVisibility, value); }
        }

        private Visibility _readWaitingVisibility = Visibility.Visible;

        public Visibility ReadWaitingVisibility
        {
            get { return _readWaitingVisibility; }
            set { SetProperty(ref _readWaitingVisibility, value); }
        }

        private Visibility _readDoneVisibility = Visibility.Hidden;

        public Visibility ReadDoneVisibility
        {
            get { return _readDoneVisibility; }
            set { SetProperty(ref _readDoneVisibility, value); }
        }

        #endregion


        public void SetProvider(IProductProvider provider)
        {
            if (_provider != null)
            {
                _provider.PropertyChanged -= Provider_PropertyChanged;
            }

            _provider = provider;
            _provider.PropertyChanged += Provider_PropertyChanged;
        }

        private void Provider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GetStatus(_provider.DownloaderStatus, out var waiting, out var progress, out var done, out var faild);

            DownloadInProgressVisibility = progress;
            DownloadWaitingVisibility = waiting;
            DownloadDoneVisibility = done;
            DownloadFaildVisibility = faild;

            GetStatus(_provider.ReaderStatus, out waiting, out progress, out done, out faild);

            ReadInProgressVisibility = progress;
            ReadWaitingVisibility = waiting;
            ReadDoneVisibility = done;
            ReadFaildVisibility = faild;
        }

        private void GetStatus(OperationStatus status, out Visibility waiting, out Visibility inProgress, out Visibility done, out Visibility faild)
        {
            Debug.WriteLine($"{nameof(IconsController)} : SetStatus to : {status}");
            switch (status)
            {
                case OperationStatus.ReadyToStart:
                    waiting = Visibility.Visible;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.InProgress:
                    waiting = Visibility.Hidden;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Visible;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.Complete:
                    waiting = Visibility.Hidden;
                    done = Visibility.Visible;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.Faild:
                    waiting = Visibility.Hidden;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(status.ToString());
            }
        }
    }
}
