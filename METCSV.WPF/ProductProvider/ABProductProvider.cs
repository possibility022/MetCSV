using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class ABProductProvider : ProductProviderBase
    {

        public ABProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Enums.Providers.AB;
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
