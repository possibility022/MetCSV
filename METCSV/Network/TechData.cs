using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace METCSV.Network
{
    class TechData : DownloadingThread
    {
        string encryptedUser = "9Vh/Fcrko4IRqanFGHUI3yyR0Zmvicf9TFCBJXC83Ek=";
        string encryptedPassword = "2nLilGIskyemdPFldgRpsEvAeKohBaQwmo0TvV8Naks=";

        string pattern = "(2017)-((0[0-9])|(1[0-2]))-(([0-2][0-9])|(3[0-1]))";

        string ftpAddres = "ftp2.techdata-it-emea.com";

        System.Windows.Forms.OpenFileDialog dialogMaterials;
        System.Windows.Forms.OpenFileDialog dialogPrices;

        public TechData(
            DownloadDone done,
            ref System.Windows.Forms.OpenFileDialog materials,
            ref System.Windows.Forms.OpenFileDialog prices)
        {
            this.done = done;
            this.dialogPrices = prices;
            this.dialogMaterials = materials;
        }

        public void downloadFile()
        {
            task = new Task(startDownloading);
            task.Start();
        }

        private void startDownloading()
        {
            try
            {
                SetDownloadingResult(DownloadingResult.inProgress);
                string folderToExtrac = "ExtractedFiles";
                string[] files = GetFileList();
                string file = getTheNewestFile(files);
                downloadFileByFtp(file);
                if (Directory.Exists(folderToExtrac))
                    Directory.Delete(folderToExtrac, true);
                System.IO.Compression.ZipFile.ExtractToDirectory(file, folderToExtrac);
                DirectoryInfo dir = new DirectoryInfo(folderToExtrac);

                string materials = findFile(dir, "TD_Material.csv");
                string prices = findFile(dir, "TD_Prices.csv");

                dialogMaterials.FileName = materials;
                dialogPrices.FileName = prices;
                SetDownloadingResult(DownloadingResult.complete);
            } catch (Exception ex)
            {
                Database.Log.log("Pobieranie techdaty nie powiodło się. " + ex.Message);
                SetDownloadingResult(DownloadingResult.faild);
            }
            done();
        }

        private string findFile(DirectoryInfo folder, string file)
        {
            FileInfo[] files = folder.GetFiles();

            foreach (FileInfo f in files)
            {
                if (f.Name == file)
                    return f.FullName;
            }
            throw new Exception("Nie znaleziono pliku:" + file + " Skontaktuj sie z Panem G :D!");
        }

        private string getTheNewestFile(string[] files)
        {
            DateTime dateTime;

            Regex regex = new Regex(this.pattern);
            Match match = regex.Match(files[0]);

            dateTime = getDate(match.Value);

            string file = files[0];

            foreach (string date in files)
            {
                DateTime newDateTime = getDate(regex.Match(date).Value);

                if (newDateTime > dateTime)
                {
                    dateTime = newDateTime;
                    file = date;
                }
            }

            return file;
        }

        private DateTime getDate(string date)
        {
            DateTime d;
            string[] parts = date.Split('-');
            d = new DateTime(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2]));

            return d;
        }

        private void downloadFileByFtp(string file)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpAddres + "/" + file));
                reqFTP.Credentials = new NetworkCredential(Global.Decrypt(encryptedUser), Global.Decrypt(encryptedPassword));
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                FileStream writeStream = new FileStream(file, FileMode.Create);
                int Length = 2048;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }
                writeStream.Close();
                response.Close();
            }
            catch (Exception ex)
            { }
        }

        private byte[] charTobyte(char[] chars)
        {
            byte[] bytes = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = Convert.ToByte(chars[i]);
            }

            return bytes;
        }

        public string[] GetFileList()
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpAddres + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Global.Decrypt(encryptedUser), Global.Decrypt(encryptedPassword));
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}
