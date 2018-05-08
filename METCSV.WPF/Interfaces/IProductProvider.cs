using System.Collections.Generic;
using System.ComponentModel;
using METCSV.Common;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Interfaces
{
    interface IProductProvider : INotifyPropertyChanged
    {
        IList<Product> GetProducts();

        bool DownloadAndLoad();

        void SetProductDownloader(IDownloader downloader);

        void SetProductReader(IProductReader reader);

        OperationStatus DownloaderStatus { get; }

        OperationStatus ReaderStatus { get; }

        Providers Provider { get; }
    }
}
