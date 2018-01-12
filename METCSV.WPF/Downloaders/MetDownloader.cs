﻿using System.Net;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Downloaders
{
    class MetDownloader : DownloaderBase
    {
        private string _fileName = "met.csv"; //todo move to config

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            using (var client = new WebClient())
            {
                client.DownloadFile("http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv", _fileName); //Stary link 
                
                //todo implement cancelation token
                
                //client.DownloadFile("http://met.redcart.pl/export/9900a7cdd99448e6d1080827e09c73da.csv", fileName); //Nowy link
            }

            Status = OperationStatus.Complete;
        }
    }
}