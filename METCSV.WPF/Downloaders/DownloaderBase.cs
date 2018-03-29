using System;
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
            finally
            {
                if (Status != OperationStatus.Faild)
                {
                    Status = OperationStatus.Complete;
                }
            }
        }

        protected abstract void Download();

        public CancellationToken CancellationToken { get; protected set; }
        
        public EventHandler<OperationStatus> OnDownloadingStatusChanged { get; set; }

        public OperationStatus Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    OnDownloadingStatusChanged(this, _status);
                }
            }
        }

        private OperationStatus _status;

        public string[] DownloadedFiles { get; protected set; }

        public void SetCancellationToken(CancellationToken token)
        {
            CancellationToken = token;
        }
    }
}
