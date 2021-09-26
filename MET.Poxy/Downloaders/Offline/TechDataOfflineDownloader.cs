using System.IO;
using MET.Data.Models;

namespace MET.Proxy.Downloaders.Offline
{
    public class TechDataOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.TechData;

        protected override bool Download()
        {
            const string folderToExtrac = "ExtractedFiles"; //todo move to config
            DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
            string materials = FindFile(dir, "TD_Material.csv"); //todo move to config
            string prices = FindFile(dir, "TD_Prices.csv"); //todo move to config
            DownloadedFiles = new[] { materials, prices };
            return true;
        }

        private string FindFile(DirectoryInfo folder, string file)
        {
            FileInfo[] files = folder.GetFiles();

            foreach (FileInfo f in files)
            {
                if (f.Name == file)
                    return f.FullName;
            }

            throw new FileNotFoundException($"Nie znaleziono pliku:{file} Skontaktuj sie z Panem T :D!");
        }
    }
}
