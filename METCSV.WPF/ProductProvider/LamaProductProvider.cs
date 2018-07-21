using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using METCSV.WPF.Interfaces;
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
            Provider = Providers.Lama;
        }

        private IProductReader GetProductReader()
        {
            return new LamaProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new LamaOfflineDownloader();
            }
            else
            {
                return new LamaDownloader(App.Settings.LamaDownloader, _token);
            }
        }
    }
}
