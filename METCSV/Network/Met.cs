using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Network
{
    class Met : DownloadingThread
    {
        public Met(DownloadDone done)
        {
            this.done = done;
            this.fileName = "met.csv";
        }

        public void downloadFile()
        {
            this.task = new Task(startDownloading);
            this.task.Start();
        }

        private void startDownloading()
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv", fileName);
                } catch (System.Net.WebException ex)
                {
                    System.Windows.Forms.MessageBox.Show("Problem z plikiem: " + fileName + ". Sprawdz czy nie jest on otwarty w jakims programie");
                    throw ex;
                }
            }


            done();
        }
    }
}
