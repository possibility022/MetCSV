using System.IO;
using System.Net;
using MET.Data.Models;
using MET.Proxy.Configuration;

namespace MET.Proxy.Downloaders
{
    public class MetDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.MET;

        private readonly string fileName;

        private readonly string url;

        public MetDownloader(IMetDownloaderSettings downloaderSettings)
        {
            fileName = downloaderSettings.CsvFile;
            url = downloaderSettings.Url;
        }

        protected override bool Download()
        {
            DownloadedFiles = new[] { string.Empty };

            using (var client = new WebClient())
            using (var webStream = client.OpenRead(url))
            using (var fileStream = new StreamWriter(fileName))
            {
                ThrowIfCanceled();
                if (webStream != null)
                    webStream.CopyTo(fileStream.BaseStream);
                else
                    return false;
            }

            DownloadedFiles[0] = fileName;
            return true;
        }
    }
}
