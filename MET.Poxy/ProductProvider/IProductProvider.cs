using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Proxy.ProductProvider
{
    public interface IProductProvider
    {
        IList<Product> GetProducts();

        bool DownloadAndLoad();

        Providers Provider { get; }

        ICollection<Product> LoadOldProducts();
    }
}
