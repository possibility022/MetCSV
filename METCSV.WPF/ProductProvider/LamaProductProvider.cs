using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class LamaProductProvider : ProductProviderBase
    {
        public LamaProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Enums.Providers.Lama;
        }

        private IProductReader GetProductReader()
        {
            return new LamaProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            return new LamaDownloader(_token);
        }
    }
}
