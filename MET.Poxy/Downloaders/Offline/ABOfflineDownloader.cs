using MET.Domain;
using System.IO;

namespace MET.Proxy.Offline
{
    public class ABOfflineDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.AB;

        string folderToExtrac = "ExtractedFiles_AB";//todo move it to config

        protected override void Download()
        {
            DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
            DownloadedFiles = new[] { dir.GetFiles()[0].FullName };
        }
    }
}
