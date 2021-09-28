using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using MET.Proxy.Downloaders.Offline;
using MET.Proxy.Interfaces;
using MET.Proxy.ProductReaders;

namespace MET.Proxy.ProductProvider
{
    class LamaProductProvider : ProductProviderBase
    {
        private readonly ILamaDownloaderSettings lamaDownloaderSettings;
        private readonly ILamaReaderSettings lamaReaderSettings;
        protected override string ArchiveFileNamePrefix => "LAMA";

        public LamaProductProvider(ILamaDownloaderSettings lamaDownloaderSettings, ILamaReaderSettings lamaReaderSettings, bool offlineMode, CancellationToken token) : base (token)
        {
            this.lamaDownloaderSettings = lamaDownloaderSettings;
            this.lamaReaderSettings = lamaReaderSettings;
            
            SetProductDownloader(GetDownloader(offlineMode));
            SetProductReader(GetProductReader());
            Provider = Providers.Lama;
        }

        private IProductReader GetProductReader()
        {
            return new LamaProductReader(lamaReaderSettings, Token);
        }

        private IDownloader GetDownloader(bool offlineMode)
        {
            if (offlineMode)
            {
                return new LamaOfflineDownloader();
            }
            else
            {
                return new LamaDownloader(lamaDownloaderSettings);
            }
        }
    }
}
