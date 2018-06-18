using METCSV.Common;
using METCSV.WPF.Configuration;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Downloaders.Offline;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class LamaProductProvider : ProductProviderBase
    {
        public LamaProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Providers.Lama;
        }

        private IProductReader GetProductReader()
        {
            return new LamaProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new LamaOfflineDownloader();
            }
            else
            {
                return new LamaDownloader(_token);
            }
        }
    }
}
