﻿using System.Collections.Generic;
using System.ComponentModel;
using MET.Data.Models;
using MET.Domain;
using MET.Proxy;
using MET.Proxy.Interfaces;
using METCSV.Common;

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

        ICollection<Product> LoadOldProducts();
        void SaveAsOldProducts(ICollection<Product> products);
    }
}
