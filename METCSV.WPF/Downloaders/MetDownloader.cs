using System.Net;
using System.Threading;
using METCSV.Common;
using METCSV.WPF.Configuration;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Downloaders
{
    class MetDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.MET;

        private string _fileName = Settings.MetDownlaoder.CsvFile;

        public MetDownloader(CancellationToken token)
        {
            SetCancellationToken(token);
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            DownloadedFiles = new[] { string.Empty };

            using (var client = new WebClient())
            {
                client.DownloadFile(Settings.MetDownlaoder.Url, _fileName);

                //todo implement cancelation token
                
            
                // "http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv" // Stary link ale jest uzwyany?
                //client.DownloadFile("http://met.redcart.pl/export/9900a7cdd99448e6d1080827e09c73da.csv", fileName); //Nowy link
            }

            DownloadedFiles[0] = _fileName;
            Status = OperationStatus.Complete;
        }
    }
}
