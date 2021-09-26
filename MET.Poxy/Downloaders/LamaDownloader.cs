﻿using System;
using System.IO;
using System.Net;
using System.Text;
using MET.Domain;
using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Downloaders;
using METCSV.Common;

namespace MET.Proxy
{
    public class LamaDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.Lama;

        public string UrlConnection { get; }

        readonly string FileName;

        readonly string CsvFileName;

        readonly string Login;

        readonly string Password;

        readonly string Request;

        public LamaDownloader(LamaDownloaderSettings settings, CancellationToken token)
        {
            SetCancellationToken(token);

            UrlConnection = settings.Url;
            FileName = settings.XmlFile;
            CsvFileName = settings.CsvFile;
            Login = settings.Login;
            Password = settings.Password;
            Request = settings.Request;

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
                    Log.Error("Strumień odpowiedzi jest pusty.");
                    Status = OperationStatus.Faild;
                    return;
                }

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

            DownloadedFiles[0] = FileName;
            Status = OperationStatus.Complete;
        }
    }
}
