using System;
using System.IO;
using System.Net;
using System.Text;
using METCSV.WPF.Enums;
using METCSV.Common;
using System.Threading;

namespace METCSV.WPF.Downloaders
{
    class LamaDownloader : DownloaderBase
    {
        const string EncryptedLogin = "cdoZtGtCO/L7S9LQ03rSJg14wdYb9l1k3fX+t75eoyg="; //todo move it to config

        public string URLConnection { get; } = "http://www.lamaplus.com.pl/partner/export.php";

        private string _fileName = "LamaDownloadedFile.xml"; //todo move it to config

        public LamaDownloader(CancellationToken token)
        {
            SetCancellationToken(token);
            DownloadedFiles = new[] { string.Empty };
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            var request = (HttpWebRequest)WebRequest.Create(URLConnection);

            var postData = "user=" + Encrypting.Decrypt(EncryptedLogin);

            postData += "&pass=***REMOVED***"; //todo move it to config
            postData += "&request=priceList";

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
            catch (WebException web)
            {
                Status = OperationStatus.Faild;
                //string message = "Lama " + web.Message + "\nGenerwoanie licznika miedzy 14:00-16:00 jest niedostępne"; //todo show this error
                //Database.Log.log(message);
                //System.Windows.Forms.MessageBox.Show(message);
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
