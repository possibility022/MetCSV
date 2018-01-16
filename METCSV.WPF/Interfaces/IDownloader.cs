using System;
using System.Collections.Generic;
using System.Threading;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Interfaces
{
    interface IDownloader
    {
        void StartDownloading();

        EventHandler OnDownloadingFinish { get; }

        EventHandler OnDownloadingStatusChanged { get; set; }

        OperationStatus Status { get; }

        IEnumerable<string> DownloadedFiles { get; }

        void SetCancellationToken(CancellationToken token);
    }
}
