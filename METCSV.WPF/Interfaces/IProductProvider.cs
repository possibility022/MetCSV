using System.Collections.Generic;
using System.ComponentModel;
using METCSV.WPF.Enums;
using METCSV.WPF.ProductProvider;

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
