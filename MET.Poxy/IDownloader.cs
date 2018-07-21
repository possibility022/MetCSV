using System;
using System.Threading;
using METCSV.Common;

namespace MET.Proxy
{
    public interface IDownloader
    {
        void StartDownloading();

        EventHandler<OperationStatus> OnDownloadingStatusChanged { get; set; }

        OperationStatus Status { get; }

        string[] DownloadedFiles { get; }

        void SetCancellationToken(CancellationToken token);
    }
}
