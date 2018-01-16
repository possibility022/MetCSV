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
            catch (Exception)
            {
                Status = OperationStatus.Faild;
            }

            if (Status != OperationStatus.Faild)
            {
                Status = OperationStatus.Complete;
            }

            OnDownloadingFinish?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void Download();

        public CancellationToken CancellationToken { get; protected set; }

        public EventHandler OnDownloadingFinish { get; protected set; }

        public EventHandler OnDownloadingStatusChanged { get; set; }

        public OperationStatus Status
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

        private OperationStatus _status;

        public IEnumerable<string> DownloadedFiles { get; protected set; }
        public void SetCancellationToken(CancellationToken token)
        {
            CancellationToken = token;
        }
    }
}
