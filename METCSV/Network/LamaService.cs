﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Web;

namespace METCSV.Network
{
    class LamaWebService : DownloadingThread
    {
        string encryptedLogin = "cdoZtGtCO/L7S9LQ03rSJg14wdYb9l1k3fX+t75eoyg=";

        public LamaWebService(DownloadDone downloadDone)
        {
            this.done = downloadDone;
            this.fileName = "LamaXml.xml";
            thread = new System.Threading.Thread(post);
        }

        public void downloadFile()
        {
            thread.Start();
        }


        private void post()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://www.lamaplus.com.pl/partner/export.php");

            var postData = "user=" + Global.Decrypt(encryptedLogin);
            postData += "&pass=629d048afcf8fbc56f594c7f25e243c2";
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
                System.Windows.Forms.MessageBox.Show(web.Message + "\nGenerwoanie licznika miedzy 14:00-16:00 jest niedostępne");
                return;
            }


            //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

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

            done();
        }
    }
}
