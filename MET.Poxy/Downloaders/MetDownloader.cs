using System.IO;
using System.Net;
using System.Threading;
using MET.Data.Models;
using MET.Domain;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
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
            using (var webStream = client.OpenRead(Url))
            using (var fileStream = new StreamWriter(FileName))
            {

                byte[] buffer = new byte[2048];

                int readed = 0;

                while((readed = webStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ThrowIfCanceled();
                    fileStream.BaseStream.Write(buffer, 0, readed);
                }
            }

            DownloadedFiles[0] = FileName;
            Status = OperationStatus.Complete;
        }
    }
}
