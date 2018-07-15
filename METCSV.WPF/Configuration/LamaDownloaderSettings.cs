namespace METCSV.WPF.Configuration
{
    public class LamaDownloaderSettings
    {
        public string XmlFile { get; set; } = "LamaDownloadedFile.xml";

        public string CsvFile { get; set; } = "LamaCSV.csv";

        public string Url { get; set; } = "http://www.lamaplus.com.pl/partner/export.php";

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Request { get; set; } = "priceList";
        public string CsvDelimiter { get; set; } = ";";
        public string SAPPrefix { get; set; } = "Lama";
        public string CsvFileEncoding { get; set; } = "ISO-8859-2";
    }
}
