namespace METCSV.WPF.Downloaders.Offline
{
    class LamaOfflineDownloader : DownloaderBase
    {
        private string _fileName = "LamaDownloadedFile.xml"; //todo move it to config
        private string _csvFileName = "LamaCSV.csv";

        protected override void Download()
        {
            DownloadedFiles = new[] { _fileName, _csvFileName };
        }
    }
}
