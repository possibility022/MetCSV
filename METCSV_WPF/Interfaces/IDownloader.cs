using System;
using System.Collections.Generic;
using System.Threading;
using METCSV_WPF.Enums;
using METCSV_WPF.Models;

namespace METCSV_WPF.Interfaces
{
    interface IDownloader
    {
        void StartDownloading();

        CancellationToken CancellationToken { get; }

        ICollection<Product> GetResults { get; }

        EventHandler OnDownloadingFinish { get; }

        EventHandler OnDownloadingStatusChanged { get; }

        DownloadingStatus Status { get; }

        IEnumerable<string> DownloadedFiles { get; }
    }
}
