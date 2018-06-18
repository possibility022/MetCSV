﻿using METCSV.Common;
using METCSV.WPF.Configuration;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Downloaders.Offline;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class MetProductProvider : ProductProviderBase
    {
        public MetProductProvider(CancellationToken token)
        {
            _token = token;
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.MET;
        }

        private IProductReader GetProductReader()
        {
            return new MetProductReader(_token);
        }

        private IDownloader GetDownloader()
        {
            if (App.SETTINGS.Engine.OfflineMode)
            {
                return new MetOfflineDownloader();
            }
            else
            {
                return new MetDownloader(_token);
            }
        }
    }
}
