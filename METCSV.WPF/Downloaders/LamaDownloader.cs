using System;
using System.IO;
using System.Net;
using System.Text;
using METCSV.WPF.Enums;
using MET.Domain; using MET.Workflows;
using System.Threading;

namespace METCSV.WPF.Downloaders
{
    class LamaDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.Lama;

        public string UrlConnection { get; } = App.Settings.LamaDownloader.Url;

        readonly string FileName = App.Settings.LamaDownloader.XmlFile;

        readonly string CsvFileName = App.Settings.LamaDownloader.CsvFile;

        readonly string Login = App.Settings.LamaDownloader.Login;

        readonly string Password = App.Settings.LamaDownloader.Password;

        readonly string Request = App.Settings.LamaDownloader.Request;

        public LamaDownloader(CancellationToken token)
        {
            SetCancellationToken(token);
            DownloadedFiles = new[] { string.Empty, CsvFileName };
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            var request = (HttpWebRequest)WebRequest.Create(UrlConnection);

            var postData = "user=" + Login;

            postData += $"&pass={Password}";
            postData += $"&request={Request}";

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
            using (var streamWriter = new FileStream(FileName, FileMode.Create))
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

            DownloadedFiles[0] = FileName;
            Status = OperationStatus.Complete;
        }
    }
}
