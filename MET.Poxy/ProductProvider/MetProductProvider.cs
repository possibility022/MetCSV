using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using MET.Proxy.Downloaders.Offline;
using MET.Proxy.Interfaces;
using MET.Proxy.ProductReaders;

namespace MET.Proxy.ProductProvider
{
    public class MetProductProvider : ProductProviderBase
    {
        private readonly IMetDownloaderSettings downloaderSettings;
        private readonly bool offlineMode;
        protected override string ArchiveFileNamePrefix => "MET";

        public MetProductProvider(IMetDownloaderSettings downloaderSettings, bool offlineMode ,CancellationToken token) : base(token)
        {
            this.downloaderSettings = downloaderSettings;
            this.offlineMode = offlineMode;
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.Met;
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
                return new MetDownloader(downloaderSettings);
            }
        }
    }
}
