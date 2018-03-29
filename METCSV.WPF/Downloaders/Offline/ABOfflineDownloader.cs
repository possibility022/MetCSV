using System.IO;

namespace METCSV.WPF.Downloaders.Offline
{
    class ABOfflineDownloader : DownloaderBase
    {
        string folderToExtrac = "ExtractedFiles_AB";//todo move it to config

        protected override void Download()
        {
            DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
            DownloadedFiles = new[] { dir.GetFiles()[0].FullName };
        }
    }
}
