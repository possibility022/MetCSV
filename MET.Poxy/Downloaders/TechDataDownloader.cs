using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MET.Data.Models;
using MET.Proxy.Configuration;

namespace MET.Proxy.Downloaders
{
    public class TechDataDownloader : DownloaderBase
    {

        public override Providers Provider => Providers.TechData;

        private readonly string user;
        private readonly string password;

        private readonly string pattern;

        private readonly string ftpAddress;

        private readonly string folderToExtract;

        private readonly string csvMaterials;

        private readonly string csvPrices;

        public TechDataDownloader(ITechDataSettings settings)
        {
            user = settings.Login;
            password = settings.Password;
            pattern = settings.Pattern;
            ftpAddress = settings.FtpAddress;
            folderToExtract = settings.FolderToExtract;
            csvMaterials = settings.CsvMaterials;
            csvPrices = settings.CsvPrices;
        }

        protected override bool Download()
        {
            var files = GetFileList();
            var file = GetTheNewestFile(files);
            DownloadFileByFtp(file);
            if (Directory.Exists(folderToExtract))
                Directory.Delete(folderToExtract, true);
            System.IO.Compression.ZipFile.ExtractToDirectory(file, folderToExtract);
            var dir = new DirectoryInfo(folderToExtract);

            var materials = FindFile(dir, csvMaterials);
            var prices = FindFile(dir, csvPrices);

            DownloadedFiles = new[] { materials, prices };

            return true;
        }

        private string FindFile(DirectoryInfo folder, string file)
        {
            foreach (var f in folder.GetFiles())
            {
                if (f.Name == file)
                    return f.FullName;
            }

            throw new FileNotFoundException($"Nie znaleziono pliku:{file} Skontaktuj sie z Panem G :D!");
        }

        private string GetTheNewestFile(string[] files)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(files[0]);

            var dateTime = GetDate(match.Value);

            var file = files[0];

            foreach (var date in files)
            {
                var newDateTime = GetDate(regex.Match(date).Value);


                ThrowIfCanceled();

                if (newDateTime > dateTime)
                {
                    dateTime = newDateTime;
                    file = date;
                }
            }

            return file;
        }

        private DateTime GetDate(string input)
        {
            var parts = input.Split('-');
            var date = new DateTime(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2]));

            return date;
        }

        private void DownloadFileByFtp(string file)
        {
            var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri($"ftp://{ftpAddress}/{file}"));
            reqFtp.Credentials = new NetworkCredential(user, password);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFtp.UseBinary = true;
            reqFtp.Proxy = null;
            reqFtp.UsePassive = false;

            using var response = (FtpWebResponse)reqFtp.GetResponse();
            using var responseStream = response.GetResponseStream();
            using var writeStream = new FileStream(file, FileMode.Create);
            
            const int length = 2048;
            var buffer = new byte[length];

            if (responseStream == null)
            {
                throw new InvalidOperationException("Nie otrzymano strumienia odpowiedzi. (Probowano pobrac plik (krok 2))");
            }

            var bytesRead = responseStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {

                ThrowIfCanceled();

                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = responseStream.Read(buffer, 0, length);
            }
        }

        public string[] GetFileList()
        {
            var result = new StringBuilder();

            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{ftpAddress}/"));

            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(user, password);
            reqFtp.Method = WebRequestMethods.Ftp.ListDirectory;
            reqFtp.Proxy = null;
            reqFtp.KeepAlive = false;
            reqFtp.UsePassive = false;
            var response = reqFtp.GetResponse();

            using (var reader
                = new StreamReader(response.GetResponseStream()
                ?? throw new InvalidOperationException("Nie otrzymano strumienia odpowiedzi. (Probowano pobrac plik (krok 1))")))
            {
                var line = reader.ReadLine();
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
