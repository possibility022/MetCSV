namespace METCSV.WPF.Configuration
{
    public class LamaDownloaderSettings
    {
        public string XmlFile { get; set; } = "LamaDownloadedFile.xml";

        public string CsvFile { get; set; } = "LamaCSV.csv";

        public string Url { get; set; } = "http://www.lamaplus.com.pl/partner/export.php";

        public string Login { get; set; } = "60117701";

        public string Password { get; set; } = "10d94d324aeecb637000dd70682c0517";

        public string Request { get; set; } = "priceList";
    }
}
