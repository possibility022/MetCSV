using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class MetProductProvider : ProductProviderBase
    {
        public MetProductProvider(CancellationToken token)
        {
            _token = token;
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
