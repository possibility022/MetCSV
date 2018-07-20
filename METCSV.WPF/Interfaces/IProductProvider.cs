using System.Collections.Generic;
using System.ComponentModel;
using MET.Domain; using MET.Workflows;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Interfaces
{
    public interface IProductProvider : INotifyPropertyChanged
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
