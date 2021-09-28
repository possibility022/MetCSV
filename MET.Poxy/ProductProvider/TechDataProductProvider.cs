using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using MET.Proxy.Downloaders.Offline;
using MET.Proxy.Interfaces;
using MET.Proxy.ProductReaders;

namespace MET.Proxy.ProductProvider
{
    class TechDataProductProvider : ProductProviderBase
    {
        private readonly ITechDataReaderSettings readerSettings;
        private readonly ITechDataDownloaderSettings downloaderSettings;
        private readonly bool offlineMode;
        protected override string ArchiveFileNamePrefix => "TechData";

        public TechDataProductProvider(ITechDataReaderSettings readerSettings, ITechDataDownloaderSettings downloaderSettings, bool offlineMode, CancellationToken token) : base(token)
        {
            this.readerSettings = readerSettings;
            this.downloaderSettings = downloaderSettings;
            this.offlineMode = offlineMode;
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.TechData;
        }

        private IProductReader GetProductReader()
        {
            return new TechDataProductReader(readerSettings, Token);
        }

        private IDownloader GetDownloader()
        {
            if (offlineMode)
            {
                return new TechDataOfflineDownloader();
            }
            else
            {
                return new TechDataDownloader(downloaderSettings);
            }
        }
    }
}
