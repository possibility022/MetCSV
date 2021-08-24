using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using METCSV.WPF.Interfaces;
using MET.Proxy.ProductReaders;
using System.Threading;
using MET.Data.Models;
using MET.Proxy.Interfaces;

namespace METCSV.WPF.ProductProvider
{
    class ABProductProvider : ProductProviderBase
    {
        protected override string ArchiveFileNamePrefix => "AB";

        public ABProductProvider(CancellationToken token) : base(token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.AB;
        }

        private IProductReader GetProductReader()
        {
            return new AbProductReader(App.Settings.ABDownloader, _token);
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
