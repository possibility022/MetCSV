using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;

namespace METCSV.WPF.Downloaders
{
    abstract class DownloaderBase : IDownloader
    {
        public void StartDownloading()
        {
            try
            {
                Download();
            }
            catch (Exception ex)
            {
                Status = DownloadingStatus.Faild;
            }

            if (Status != DownloadingStatus.Faild)
            {
                Status = DownloadingStatus.Complete;
            }

            OnDownloadingFinish?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void Download();

        public CancellationToken CancellationToken { get; protected set; }

        public EventHandler OnDownloadingFinish { get; protected set; }

        public EventHandler OnDownloadingStatusChanged { get; protected set; }

        public DownloadingStatus Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    OnDownloadingStatusChanged(this, EventArgs.Empty);
                    _status = value;
                }
            }
        }

        private DownloadingStatus _status;

        public IEnumerable<string> DownloadedFiles { get; protected set; }
    }
}
