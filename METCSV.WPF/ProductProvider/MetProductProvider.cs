using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using MET.Proxy.ProductReaders;
using System.Threading;
using MET.Proxy.Interfaces;

namespace METCSV.WPF.ProductProvider
{
    class MetProductProvider : ProductProviderBase
    {
        public MetProductProvider(CancellationToken token) : base(token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.MET;
        }

        private IProductReader GetProductReader()
        {
            return new MetProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new MetOfflineDownloader();
            }
            else
            {
                return new MetDownloader(App.Settings.MetDownlaoder, _token);
            }
        }
    }
}
