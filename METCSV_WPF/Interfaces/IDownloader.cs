﻿using System;
using System.Collections.Generic;
using System.Threading;
using METCSV_WPF.Enums;

namespace METCSV_WPF.Interfaces
{
    interface IDownloader
    {
        void StartDownloading();

        CancellationToken CancellationToken { get; }

        EventHandler OnDownloadingFinish { get; }

        EventHandler OnDownloadingStatusChanged { get; }

        DownloadingStatus Status { get; }

        IEnumerable<string> DownloadedFiles { get; }
    }
}
