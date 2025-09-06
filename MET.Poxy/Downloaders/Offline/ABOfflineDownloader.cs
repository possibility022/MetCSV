using System.IO;
using MET.Data.Models;

namespace MET.Proxy.Downloaders.Offline
{
    public class AbOfflineDownloader : DownloaderBase
    {
        public override Providers Provider => Providers.Ab;

        string folderToExtract = "ExtractedFiles_AB";//todo move it to config

        protected override bool Download()
        {
            DirectoryInfo dir = new DirectoryInfo(folderToExtract);
            DownloadedFiles = new[] { dir.GetFiles()[0].FullName };
            return true;
        }
    }
}
