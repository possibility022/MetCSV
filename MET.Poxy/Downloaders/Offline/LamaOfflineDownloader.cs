using MET.Data.Models;

namespace MET.Proxy.Downloaders.Offline
{
    public class LamaOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.Lama;

        private const string FileName = "LamaDownloadedFile.xml"; //todo move it to config
        private const string CsvFileName = "LamaCSV.csv";

        protected override bool Download()
        {
            DownloadedFiles = new[] { FileName, CsvFileName };
            return true;
        }
    }
}
