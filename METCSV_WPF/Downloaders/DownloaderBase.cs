using System;
using System.Collections.Generic;
using System.Threading;
using METCSV_WPF.Enums;
using METCSV_WPF.Interfaces;

namespace METCSV_WPF.Downloaders
{
    abstract class DownloaderBase : IDownloader
    {
        public abstract void StartDownloading();

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
