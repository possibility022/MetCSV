using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using MET.Proxy.Downloaders.Offline;
using MET.Proxy.Interfaces;
using MET.Proxy.ProductReaders;

namespace MET.Proxy.ProductProvider
{
    class MetProductProvider : ProductProviderBase
    {
        private readonly IMetSettings settings;
        private readonly bool offlineMode;
        protected override string ArchiveFileNamePrefix => "MET";

        public MetProductProvider(IMetSettings settings, bool offlineMode ,CancellationToken token) : base(token)
        {
            this.settings = settings;
            this.offlineMode = offlineMode;
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.MET;
        }

        private IProductReader GetProductReader()
        {
            return new MetProductReader(Token);
        }

        private IDownloader GetDownloader()
        {
            if (offlineMode)
            {
                return new MetOfflineDownloader();
            }
            else
            {
                return new MetDownloader(settings);
            }
        }
    }
}
