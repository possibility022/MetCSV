using MET.Domain;
using MET.Proxy.Configuration;
using METCSV.Common;
using METCSV.Common.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MET.Proxy
{
    public class TechDataDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.TechData;

        readonly string User;
        readonly string Password;

        readonly string Pattern;

        readonly string FtpAddres;

        readonly string FolderToExtract;

        readonly string CsvMaterials;

        readonly string CsvPrices;

        public TechDataDownloader(TechDataDownloaderSettings settings, CancellationToken token)
        {
            SetCancellationToken(token);

            User = settings.Login;
            Password = settings.Password;
            Pattern = settings.Pattern;
            FtpAddres = settings.FtpAddress;
            FolderToExtract = settings.FolderToExtract;
            CsvMaterials = settings.CsvMaterials;
            CsvPrices = settings.CsvPrices;
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


                ThrowIfCanceled();

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

            using (FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream writeStream = new FileStream(file, FileMode.Create))
            {
                int length = 2048;
                Byte[] buffer = new Byte[length];

                if (responseStream == null)
                {
                    throw new InvalidOperationException("Nie otrzymano strumienia odpowiedzi. (Probowano pobrac plik (krok 2))");
                }

                int bytesRead = responseStream.Read(buffer, 0, length);
                while (bytesRead > 0)
                {

                    ThrowIfCanceled();

                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, length);
                }
            }
        }

        public string[] GetFileList()
        {
            StringBuilder result = new StringBuilder();

            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{FtpAddres}/"));

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
                    ThrowIfCanceled();

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
