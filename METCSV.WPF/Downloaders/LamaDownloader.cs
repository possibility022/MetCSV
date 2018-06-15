using System;
using System.IO;
using System.Net;
using System.Text;
using METCSV.WPF.Enums;
using METCSV.Common;
using System.Threading;
using METCSV.WPF.Configuration;

namespace METCSV.WPF.Downloaders
{
    class LamaDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.Lama;

        public string URLConnection { get; } = Settings.LamaDownloader.Url;

        private string _fileName = Settings.LamaDownloader.XmlFile;

        private string _csvFileName = Settings.LamaDownloader.CsvFile;

        public LamaDownloader(CancellationToken token)
        {
            SetCancellationToken(token);
            DownloadedFiles = new[] { string.Empty, _csvFileName };
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            var request = (HttpWebRequest)WebRequest.Create(URLConnection);

            var postData = "user=" + Settings.LamaDownloader.Login;

            postData += $"&pass={Settings.LamaDownloader.Password}"; //todo move it to config
            postData += $"&request={Settings.LamaDownloader.Request}";

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Status = OperationStatus.Faild;
                string message = $"Generwoanie licznika miedzy 14:00-16:00 jest niedostępne";
                LogError(ex, message);
                return;
            }

            using (Stream responseStream = response.GetResponseStream())
            using (var streamWriter = new FileStream(_fileName, FileMode.Create))
            {
                if (responseStream == null)
                {
                    Status = OperationStatus.Faild;
                    return;
                }

                int Length = 2048;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    streamWriter.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }

                responseStream.Close();
            }

            DownloadedFiles[0] = _fileName;
            Status = OperationStatus.Complete;
        }
    }
}
