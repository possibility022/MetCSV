using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MET.Domain; using MET.Workflows;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Downloaders
{
    class TechDataDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.TechData;

        readonly string User = App.Settings.TDDownloader.Login;
        readonly string Password = App.Settings.TDDownloader.Password;

        readonly string Pattern = App.Settings.TDDownloader.Pattern;

        readonly string FtpAddres = App.Settings.TDDownloader.FtpAddress;

        readonly string FolderToExtract = App.Settings.TDDownloader.FolderToExtract;

        readonly string CsvMaterials = App.Settings.TDDownloader.CsvMaterials;

        readonly string CsvPrices = App.Settings.TDDownloader.CsvPrices;

        public TechDataDownloader(CancellationToken token)
        {
            SetCancellationToken(token);
        }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            string[] files = GetFileList();
            string file = GetTheNewestFile(files);
            DownloadFileByFtp(file);
            if (Directory.Exists(FolderToExtract))
                Directory.Delete(FolderToExtract, true);
            System.IO.Compression.ZipFile.ExtractToDirectory(file, FolderToExtract);
            DirectoryInfo dir = new DirectoryInfo(FolderToExtract);

            string materials = FindFile(dir, CsvMaterials);
            string prices = FindFile(dir, CsvPrices);

            DownloadedFiles = new[] { materials, prices };

            Status = OperationStatus.Complete;
        }

        private string FindFile(DirectoryInfo folder, string file)
        {
            foreach (FileInfo f in folder.GetFiles())
            {
                if (f.Name == file)
                    return f.FullName;
            }

            throw new FileNotFoundException($"Nie znaleziono pliku:{file} Skontaktuj sie z Panem G :D!");
        }

        private string GetTheNewestFile(string[] files)
        {
            DateTime dateTime;

            Regex regex = new Regex(Pattern);
            Match match = regex.Match(files[0]);

            dateTime = GetDate(match.Value);

            string file = files[0];

            foreach (string date in files)
            {
                DateTime newDateTime = GetDate(regex.Match(date).Value);

                if (newDateTime > dateTime)
                {
                    dateTime = newDateTime;
                    file = date;
                }
            }

            return file;
        }

        private DateTime GetDate(string date)
        {
            DateTime d;
            string[] parts = date.Split('-');
            d = new DateTime(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2]));

            return d;
        }

        private void DownloadFileByFtp(string file)
        {
            var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri($"ftp://{FtpAddres}/{file}"));
            reqFtp.Credentials = new NetworkCredential(User, Password);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFtp.UseBinary = true;
            reqFtp.Proxy = null;
            reqFtp.UsePassive = false;
            FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
            Stream responseStream = response.GetResponseStream();
            FileStream writeStream = new FileStream(file, FileMode.Create);
            int length = 2048;
            Byte[] buffer = new Byte[length];

            if (responseStream == null)
            {
                throw new InvalidOperationException("Nie otrzymano strumienia odpowiedzi. (Probowano pobrac plik (krok 2))");
            }

            int bytesRead = responseStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = responseStream.Read(buffer, 0, length);
            }
            writeStream.Close();
            response.Close();
        }

        public string[] GetFileList()
        {
            StringBuilder result = new StringBuilder();

            var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri($"ftp://{FtpAddres}/"));

            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(User, Password);
            reqFtp.Method = WebRequestMethods.Ftp.ListDirectory;
            reqFtp.Proxy = null;
            reqFtp.KeepAlive = false;
            reqFtp.UsePassive = false;
            var response = reqFtp.GetResponse();

            using (StreamReader reader
                = new StreamReader(response.GetResponseStream()
                ?? throw new InvalidOperationException("Nie otrzymano strumienia odpowiedzi. (Probowano pobrac plik (krok 1))")))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
            }

            // to remove the trailing '\n'
            result.Remove(result.ToString().LastIndexOf('\n'), 1);
            return result.ToString().Split('\n');
        }
    }
}
