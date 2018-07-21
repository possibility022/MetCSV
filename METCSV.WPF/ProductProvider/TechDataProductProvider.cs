using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class TechDataProductProvider : ProductProviderBase
    {
        public TechDataProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Providers.TechData;
        }

        private IProductReader GetProductReader()
        {
            return new TechDataProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new TechDataOfflineDownloader();
            }
            else
            {
                return new TechDataDownloader(App.Settings.TDDownloader, _token);
            }
        }
    }
}
