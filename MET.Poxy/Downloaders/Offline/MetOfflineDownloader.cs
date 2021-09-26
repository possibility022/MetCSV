using MET.Data.Models;
using MET.Domain;
using MET.Proxy.Downloaders;

namespace MET.Proxy.Offline
{
    public class MetOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.MET;

        private string _fileName = "met.csv"; //todo move to config

        protected override void Download()
        {
            DownloadedFiles = new[] { _fileName };
        }
    }
}
