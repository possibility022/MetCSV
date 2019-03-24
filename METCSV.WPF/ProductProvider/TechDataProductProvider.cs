﻿using MET.Domain;
using MET.Proxy;
using MET.Proxy.Offline;
using MET.Proxy.ProductReaders;
using System.Threading;
using MET.Proxy.Interfaces;

namespace METCSV.WPF.ProductProvider
{
    class TechDataProductProvider : ProductProviderBase
    {
        protected override string ArchiveFileNamePrefix => "TechData";

        public TechDataProductProvider(CancellationToken token) : base(token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            Provider = Providers.TechData;
        }

        private IProductReader GetProductReader()
        {
            return new TechDataProductReader(App.Settings.TDDownloader, _token);
        }

        private IDownloader GetDownloader()
        {
            if (App.Settings.Engine.OfflineMode)
            {
                return new TechDataOfflineDownloader();
            }
            else
            {
                return new TechDataDownloader(App.Settings.TDDownloader, _token);
            }
        }
    }
}
