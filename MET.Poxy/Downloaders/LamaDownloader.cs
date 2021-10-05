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

        private readonly string productsFile;

        private readonly string login;

        private readonly string password;

        private readonly string request;
        private readonly string manufacturersRequest;
        private readonly string manufacturersXmlFile;

        const string manufacturersZipFile = "lamaManufacturers.zip";

        public LamaDownloader(ILamaDownloaderSettings downloaderSettings)
        {
            UrlConnection = downloaderSettings.Url;
            productsFile = downloaderSettings.XmlFile;
            manufacturersXmlFile = downloaderSettings.ManufacturersXmlFile;
            login = downloaderSettings.Login;
            password = downloaderSettings.Password;
            request = downloaderSettings.Request;
            manufacturersRequest = downloaderSettings.ManufacturersRequest;
        }

        protected override bool Download()
        {
            var priceList = DownloadFile(this.request, productsFile);
            var manufacturers = DownloadFile(this.manufacturersRequest, manufacturersZipFile);

            var extractedFile = ExtractZipFile();
            File.Move(extractedFile, manufacturersXmlFile, true);

            DownloadedFiles = new[] { productsFile, manufacturersXmlFile };

            return priceList && manufacturers;
        }

        private string ExtractZipFile()
        {
            string tempFolder = Path.GetFullPath("lamaExtractedFiles");

            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            System.IO.Compression.ZipFile.ExtractToDirectory(manufacturersZipFile, tempFolder);

            return FindFile(tempFolder);
        }

        private string FindFile(string folder)
        {
            var files = Directory.GetFiles(folder);
            if (files.Length != 1)
            {
                throw new InvalidDataException($"File could not be found. Only one file expected within folder {folder}.");
            }

            return Path.GetFullPath(files[0]);
        }

        private bool DownloadFile(string request, string file)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(UrlConnection);

            var postData = "user=" + login;

            postData += $"&pass={password}";
            postData += $"&request={request}";

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
            using (var streamWriter = new FileStream(file, FileMode.Create))
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

            return true;
        }
    }
}
