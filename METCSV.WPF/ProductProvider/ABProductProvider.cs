using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;

namespace METCSV.WPF.ProductProvider
{
    class ABProductProvider : ProductProviderBase
    {

        public ABProductProvider()
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
        }

        private IProductReader GetProductReader()
        {
            AbProductReader reader = new AbProductReader(_token);
            return reader;
        }

        private IDownloader GetDownloader()
        {
            return new AbDownloader(_token);
        }
    }
}
