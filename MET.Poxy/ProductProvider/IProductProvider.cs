using System.Collections.Generic;
using MET.Data.Models;
using MET.Proxy.Interfaces;

namespace MET.Proxy.ProductProvider
{
    public interface IProductProvider
    {
        IList<Product> GetProducts();

        bool DownloadAndLoad();

        void SetProductDownloader(IDownloader downloader);

        void SetProductReader(IProductReader reader);

        Providers Provider { get; }

        ICollection<Product> LoadOldProducts();
        void SaveAsOldProducts(ICollection<Product> products);
    }
}
