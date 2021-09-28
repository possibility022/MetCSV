using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using MET.Proxy.Downloaders.Offline;
using MET.Proxy.Interfaces;
using MET.Proxy.ProductReaders;

namespace MET.Proxy.ProductProvider
{
    public class ABProductProvider : ProductProviderBase
    {
        private readonly IAbReaderSettings readerSettings;
        private readonly IAbDownloaderSettings downloaderDownloaderSettings;
        protected override string ArchiveFileNamePrefix => "AB";

        public ABProductProvider(IAbReaderSettings readerSettings, IAbDownloaderSettings downloaderDownloaderSettings, bool offlineMode, CancellationToken token) : base(token)
        {
            this.readerSettings = readerSettings;
            this.downloaderDownloaderSettings = downloaderDownloaderSettings;
            SetProductDownloader(GetDownloader(offlineMode));
            SetProductReader(GetProductReader());
            Provider = Providers.AB;
        }

        private IProductReader GetProductReader()
        {
            return new AbProductReader(readerSettings, Token);
        }

        private IDownloader GetDownloader(bool offlineMode)
        {
            if (offlineMode)
            {
                return new ABOfflineDownloader();
            }
            else
            {
                return new AbDownloader(downloaderDownloaderSettings, Token);
            }
        }
    }
}
