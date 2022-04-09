using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Proxy.Configuration;

namespace MET.Proxy.Downloaders
{
    public class MetDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.MET;

        private readonly string fileName;

        private readonly string url;

        private readonly string metPriceFileName;

        private readonly string metPriceUrl;

        public MetDownloader(IMetDownloaderSettings downloaderSettings)
        {
            fileName = downloaderSettings.CsvFile;
            url = downloaderSettings.Url;
            metPriceFileName = downloaderSettings.MetPriceCsvFile;
            metPriceUrl = downloaderSettings.MetPriceUrl;
        }

        protected override bool Download()
        {
            DownloadedFiles = new[] { string.Empty, string.Empty };

            using (var client = new WebClient()) //todo replace webClient
            using (var webStream = client.OpenRead(url))
            using (var fileStream = new StreamWriter(fileName))
            {
                ThrowIfCanceled();
                if (webStream != null)
                    webStream.CopyTo(fileStream.BaseStream);
                else
                    return false;
            }

            var downloadPrices = DownloadMetProductsWithPricesAsync();
            var t = downloadPrices.ConfigureAwait(false);
            t.GetAwaiter().GetResult();

            DownloadedFiles[0] = fileName;
            DownloadedFiles[1] = metPriceFileName;
            return true;
        }

        protected async Task DownloadMetProductsWithPricesAsync()
        {
            using (var client = new HttpClient())
            using (var file = new StreamWriter(metPriceFileName))
            {
                ThrowIfCanceled();
                var stream = await client.GetStreamAsync(metPriceUrl);
                stream.CopyTo(file.BaseStream);
            }
        }
    }
}
