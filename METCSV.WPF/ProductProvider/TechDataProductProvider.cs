using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;

namespace METCSV.WPF.ProductProvider
{
    class TechDataProductProvider : ProductProvderBase
    {
        public TechDataProductProvider()
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
        }

        private IProductReader GetProductReader()
        {
            IProductReader reader = new TechDataProductReader(_token);
            return reader;
        }

        private IDownloader GetDownloader()
        {
            return new TechDataDownloader(_token);
        }
    }
}
