using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;

namespace METCSV.WPF.ProductProvider
{
    class LamaProductProvider : ProductProvderBase
    {
        public LamaProductProvider()
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
        }

        private IProductReader GetProductReader()
        {
            return new LamaProductReader();
        }

        private IDownloader GetDownloader()
        {
            return new LamaDownloader();
        }
    }
}
