using MET.Domain;
using System.IO;
using MET.Data.Models;
using MET.Proxy.Downloaders;

namespace MET.Proxy.Offline
{
    public class TechDataOfflineDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.TechData;

        protected override void Download()
        {
            string folderToExtrac = "ExtractedFiles"; //todo move to config
            DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
            string materials = FindFile(dir, "TD_Material.csv"); //todo move to config
            string prices = FindFile(dir, "TD_Prices.csv"); //todo move to config
            DownloadedFiles = new[] { materials, prices };
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
