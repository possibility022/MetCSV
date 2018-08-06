using System;
using System.Threading;
using MET.Domain;
using METCSV.Common;
using METCSV.Common.Exceptions;

namespace MET.Proxy
{
    public abstract class DownloaderBase : IDownloader
    {
        public void StartDownloading()
        {
            try
            {
                LogInfo("Rozpoczynam pobieranie.");
                Download();
                LogInfo("Pobieranie ukończone.");
            }
            catch (Exception ex)
            {
                LogError(ex, "Pobieranie nie powiodło się.");
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

        public abstract Providers Provider { get; }

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
                    OnDownloadingStatusChanged?.Invoke(this, _status);
                }
            }
        }

        private OperationStatus _status;

        public string[] DownloadedFiles { get; protected set; }

        public void SetCancellationToken(CancellationToken token)
        {
            CancellationToken = token;
        }

        protected void LogError(Exception ex, string message)
        {
            Log.Error(ex, FormatMessage(message));
        }

        protected void LogError(string message)
        {
            Log.Error(FormatMessage(message));
        }

        protected void LogInfo(string message)
        {
            Log.Info(FormatMessage(message));
        }

        private string FormatMessage(string message)
        {
            return $"[{Provider}] - {message}";
        }

        protected void ThrowIfCanceled()
        {
            if (CancellationToken.IsCancellationRequested)
                throw new CancelledException();
        }
    }
}
