using MET.Domain; using MET.Workflows;
namespace METCSV.WPF.Downloaders.Offline
{
    class MetOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.MET;

        private string _fileName = "met.csv"; //todo move to config

        protected override void Download()
        {
            DownloadedFiles = new[] { _fileName };
        }
    }
}
