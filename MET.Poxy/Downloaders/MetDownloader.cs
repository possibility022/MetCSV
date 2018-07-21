using System.Net;
using System.Threading;
using MET.Domain;
using MET.Proxy.Configuration;
using METCSV.Common;

namespace MET.Proxy
{
    public class MetDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.MET;

        private readonly string FileName;

        readonly string Url;

        public MetDownloader(MetDownloaderSettings settings, CancellationToken token)
        {
            FileName = settings.CsvFile;
            Url = settings.Url;
            SetCancellationToken(token);
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            DownloadedFiles = new[] { string.Empty };

            using (var client = new WebClient())
            {
                client.DownloadFile(Url, FileName);

                //todo implement cancelation token


                // "http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv" // Stary link ale jest uzwyany?
                //client.DownloadFile("http://met.redcart.pl/export/9900a7cdd99448e6d1080827e09c73da.csv", fileName); //Nowy link
            }

            DownloadedFiles[0] = FileName;
            Status = OperationStatus.Complete;
        }
    }
}
