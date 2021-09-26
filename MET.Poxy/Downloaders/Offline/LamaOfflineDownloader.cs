using MET.Data.Models;
using MET.Domain;
using MET.Proxy.Downloaders;

namespace MET.Proxy.Offline
{
    public class LamaOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.Lama;

        private string _fileName = "LamaDownloadedFile.xml"; //todo move it to config
        private string _csvFileName = "LamaCSV.csv";

        protected override void Download()
        {
            DownloadedFiles = new[] { _fileName, _csvFileName };
        }
    }
}
