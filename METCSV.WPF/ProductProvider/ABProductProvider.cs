using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using METCSV.WPF.Interfaces;
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
            Provider = Providers.AB;
        }

        private IProductReader GetProductReader()
        {
            return new AbProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new ABOfflineDownloader();
            }
            else
            {
                return new AbDownloader(App.Settings.ABDownloader, _token);
            }
        }
    }
}
