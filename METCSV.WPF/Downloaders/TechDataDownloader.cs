using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using METCSV.Common;
using METCSV.WPF.Enums;

namespace METCSV.WPF.Downloaders
{
    class TechDataDownloader : DownloaderBase
    {

        const string EncryptedUser = "9Vh/Fcrko4IRqanFGHUI3yyR0Zmvicf9TFCBJXC83Ek="; //todo move to config
        const string EncryptedPassword = "2nLilGIskyemdPFldgRpsEvAeKohBaQwmo0TvV8Naks=";

        const string Pattern = "(20[1-5][0-9])-((0[0-9])|(1[0-2]))-(([0-2][0-9])|(3[0-1]))";

        const string FtpAddres = "ftp2.techdata-it-emea.com";


        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            string folderToExtrac = "ExtractedFiles"; //todo move to config
            string[] files = GetFileList();
            string file = GetTheNewestFile(files);
            DownloadFileByFtp(file);
            if (Directory.Exists(folderToExtrac))
                Directory.Delete(folderToExtrac, true);
            System.IO.Compression.ZipFile.ExtractToDirectory(file, folderToExtrac);
            DirectoryInfo dir = new DirectoryInfo(folderToExtrac);

            string materials = FindFile(dir, "TD_Material.csv"); //todo move to config
            string prices = FindFile(dir, "TD_Prices.csv"); //todo move to config

            DownloadedFiles = new[] {materials, prices};

            Status = OperationStatus.Complete;
        }

        private string FindFile(DirectoryInfo folder, string file)
        {
            FileInfo[] files = folder.GetFiles();

            foreach (FileInfo f in files)
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
            var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri($"ftp://{FtpAddres}/{file}")); //todo to config
            reqFtp.Credentials = new NetworkCredential(Encrypting.Decrypt(EncryptedUser), Encrypting.Decrypt(EncryptedPassword));
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
            
            var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri($"ftp://{FtpAddres}/")); //todo to config

            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(Encrypting.Decrypt(EncryptedUser), Encrypting.Decrypt(EncryptedPassword));
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
