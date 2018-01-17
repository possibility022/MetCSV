using System.Collections.Generic;
using System.ComponentModel;
using METCSV.WPF.Enums;
using METCSV.WPF.Models;

namespace METCSV.WPF.Interfaces
{
    interface IProductProvider : INotifyPropertyChanged
    {
        IEnumerable<Product> GetProducts();

        void SetProductDownloader(IDownloader downloader);

        void SetProductReader(IProductReader reader);

        OperationStatus DownloaderStatus { get; }

        OperationStatus ReaderStatus { get; }
    }
}
