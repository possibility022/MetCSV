using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;

namespace METCSV.WPF.ProductProvider
{
    class MetProductProvider : ProductProvderBase
    {
        public MetProductProvider()
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
        }

        private IProductReader GetProductReader()
        {
            return new MetProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            return new MetDownloader(_token);
        }
    }
}
