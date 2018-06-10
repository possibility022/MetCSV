using System;
using System.Threading;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Interfaces
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
