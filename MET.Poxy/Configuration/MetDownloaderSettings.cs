﻿namespace MET.Proxy.Configuration
{
    public class MetDownloaderSettings : IMetSettings
    {
        public string CsvFile { get; set; } = "met.csv";

        public string Url { get; set; } = "http://met.redcart.pl/export/d9b11de494035a84e68e5faa6063692a.csv";
    }
}
