﻿using METCSV.WPF.Downloaders;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductReaders;
using System.Threading;

namespace METCSV.WPF.ProductProvider
{
    class TechDataProductProvider : ProductProviderBase
    {
        public TechDataProductProvider(CancellationToken token)
        {
            SetProductDownloader(GetDownloader());
            SetProductReader(GetProductReader());
            _token = token;
            Provider = Enums.Providers.TechData;
        }

        private IProductReader GetProductReader()
        {
            IProductReader reader = new TechDataProductReader(_token);
            return reader;
        }

        private IDownloader GetDownloader()
        {
            return new TechDataDownloader(_token);
        }
    }
}
