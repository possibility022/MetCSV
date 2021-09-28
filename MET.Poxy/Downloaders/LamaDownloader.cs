using System;
using System.IO;
using System.Net;
using System.Text;
using MET.Data.Models;
using MET.Proxy.Configuration;

namespace MET.Proxy.Downloaders
{
    public class LamaDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.Lama;

        public string UrlConnection { get; }

        private readonly string fileName;

        private readonly string login;

        private readonly string password;

        private readonly string request;

        public LamaDownloader(ILamaDownloaderSettings downloaderSettings)
        {
            UrlConnection = downloaderSettings.Url;
            fileName = downloaderSettings.XmlFile;
            var csvFileName = downloaderSettings.CsvFile;
            login = downloaderSettings.Login;
            password = downloaderSettings.Password;
            request = downloaderSettings.Request;

            DownloadedFiles = new[] { string.Empty, csvFileName };
        }

        protected override bool Download()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(UrlConnection);

            var postData = "user=" + login;

            postData += $"&pass={password}";
            postData += $"&request={this.request}";

            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            httpWebRequest.ContentLength = data.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                string message = $"Generwoanie licznika miedzy 14:00-16:00 jest niedostępne";
                LogError(ex, message);
                return false;
            }

            using (Stream responseStream = response.GetResponseStream())
            using (var streamWriter = new FileStream(fileName, FileMode.Create))
            {
                byte[] buffer = new Byte[2048];
                int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    ThrowIfCanceled();
                    streamWriter.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                }

                responseStream.Close();
            }

            DownloadedFiles[0] = fileName;
            return true;
        }
    }
}
