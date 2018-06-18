using METCSV.Common;
using METCSV.WPF.Configuration;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Downloaders.Offline;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class ABProductProvider : ProductProviderBase
    {

        public ABProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Providers.AB;
        }

        private IProductReader GetProductReader()
        {
            return new AbProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new ABOfflineDownloader();
            }
            else
            {
                return new AbDownloader(_token);
            }
        }
    }
}
