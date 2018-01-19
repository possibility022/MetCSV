using System;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;

namespace METCSV.Network
{
    class LamaWebService : DownloadingThread
    {
        string encryptedLogin = "cdoZtGtCO/L7S9LQ03rSJg14wdYb9l1k3fX+t75eoyg=";

        public LamaWebService(DownloadDone downloadDone)
        {
            this.done = downloadDone;
            this.fileName = "LamaXml.xml";
            task = new Task(post);
        }

        public void downloadFile()
        {
            task.Start();
        }


        private void post()
        {
            try
            {
                SetDownloadingResult(Global.Result.inProgress);
                var request = (HttpWebRequest)WebRequest.Create("http://www.lamaplus.com.pl/partner/export.php");

                var postData = "user=" + Global.Decrypt(encryptedLogin);
                //postData += "&pass=629d048afcf8fbc56f594c7f25e243c2"; //old password
                postData += "&pass=***REMOVED***";
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
                    SetDownloadingResult(Global.Result.faild);
                    string message = "Lama " + web.Message + "\nGenerwoanie licznika miedzy 14:00-16:00 jest niedostępne";
                    Database.Log.log(message);
                    System.Windows.Forms.MessageBox.Show(message);
                    return;
                }

                Stream responseStream = response.GetResponseStream();

                using (var streamWriter = new FileStream(fileName, FileMode.Create))
                {
                    int Length = 2048;
                    Byte[] buffer = new Byte[Length];
                    int bytesRead = responseStream.Read(buffer, 0, Length);
                    while (bytesRead > 0)
                    {
                        streamWriter.Write(buffer, 0, bytesRead);
                        bytesRead = responseStream.Read(buffer, 0, Length);
                    }
                }

                responseStream.Close();
                SetDownloadingResult(Global.Result.complete);
            } catch (Exception ex)
            {
                Database.Log.log("Pobieranie Lamy nie powiodło się. " + ex.Message);
                SetDownloadingResult(Global.Result.faild);
            }

            done();
        }
    }
}
