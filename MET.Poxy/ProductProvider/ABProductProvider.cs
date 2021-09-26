using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using METCSV.WPF.Interfaces;
using MET.Proxy.ProductReaders;
using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Interfaces;

namespace METCSV.WPF.ProductProvider
{
    public class ABProductProvider : ProductProviderBase
    {
        private readonly IAbSettings settings;
        protected override string ArchiveFileNamePrefix => "AB";

        public ABProductProvider(IAbSettings settings, bool offlineMode, CancellationToken token) : base(token)
        {
            this.settings = settings;
            SetProductDownloader(GetDownloader(offlineMode));
            SetProductReader(GetProductReader());
            Provider = Providers.AB;
        }

        private IProductReader GetProductReader()
        {
            return new AbProductReader(settings, _token);
        }

        private IDownloader GetDownloader(bool offlineMode)
        {
            if (offlineMode)
            {
                return new ABOfflineDownloader();
            }
            else
            {
                return new AbDownloader(settings, _token);
            }
        }
    }
}
