using MET.Domain; using MET.Workflows;
namespace METCSV.WPF.Downloaders.Offline
{
    class LamaOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.Lama;

        private string _fileName = "LamaDownloadedFile.xml"; //todo move it to config
        private string _csvFileName = "LamaCSV.csv";

        protected override void Download()
        {
            DownloadedFiles = new[] { _fileName, _csvFileName };
        }
    }
}
