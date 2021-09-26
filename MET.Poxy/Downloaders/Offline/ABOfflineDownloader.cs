using System.IO;
using MET.Data.Models;

namespace MET.Proxy.Downloaders.Offline
{
    public class ABOfflineDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.AB;

        string folderToExtract = "ExtractedFiles_AB";//todo move it to config

        protected override bool Download()
        {
            DirectoryInfo dir = new DirectoryInfo(folderToExtract);
            DownloadedFiles = new[] { dir.GetFiles()[0].FullName };
            return true;
        }
    }
}
