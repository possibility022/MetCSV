using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Configuration
{
    class TechDataDownloaderSettings
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FtpAddress { get; set; } = "ftp2.techdata-it-emea.com";

        public string Pattern { get; set; } = "(20[1-5][0-9])-((0[0-9])|(1[0-2]))-(([0-2][0-9])|(3[0-1]))";

        public string FolderToExtract { get; set; } = "ExtractedFiles";

        public string CsvMaterials { get; set; } = "TD_Material.csv";

        public string CsvPrices { get; set; } = "TD_Prices.csv";
    }
}
