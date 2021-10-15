using MET.Data.Models;
using MET.Proxy.Configuration;

namespace MET.Proxy.Downloaders.Offline
{
    public class LamaOfflineDownloader : DownloaderBase
    {

        public LamaOfflineDownloader(ILamaDownloaderSettings lamaDownloaderSettings)
        {
            this.lamaDownloaderSettings = lamaDownloaderSettings;
        }

        public override Providers Provider => Providers.Lama;
        private readonly ILamaDownloaderSettings lamaDownloaderSettings;

        protected override bool Download()
        {
            DownloadedFiles = new[] { lamaDownloaderSettings.XmlFile, lamaDownloaderSettings.ManufacturersXmlFile };
            return true;
        }
    }
}
