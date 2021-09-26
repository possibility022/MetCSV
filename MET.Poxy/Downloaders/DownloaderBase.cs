using System;
using System.Threading;
using MET.Data.Models;
using METCSV.Common;
using METCSV.Common.Exceptions;

namespace MET.Proxy.Downloaders
{
    public abstract class DownloaderBase : IDownloader
    {
        public bool StartDownloading(CancellationToken token)
        {
            try
            {
                this.CancellationToken = token;
                LogInfo("Rozpoczynam pobieranie.");
                var results = Download();
                LogInfo("Pobieranie ukończone.");
                return results;
            }
            catch (CancelledException)
            {
                LogInfo("Anulowano przez użytkownika.");
                return false;
            }
            catch (Exception ex)
            {
                LogError(ex, "Pobieranie nie powiodło się.");
                return false;
            }
        }

        public abstract Providers Provider { get; }

        protected abstract bool Download();

        public CancellationToken CancellationToken { get; protected set; }
        
        public string[] DownloadedFiles { get; protected set; }
        
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
