using MET.Data.Models;

namespace MET.Proxy.Downloaders.Offline
{
    public class MetOfflineDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.MET;

        private const string FileName = "met.csv"; //todo move to config

        private const string MetFileWithPrice = "metprice.csv";

        protected override bool Download()
        {
            DownloadedFiles = new[] { FileName, MetFileWithPrice };
            return true;
        }
    }
}
