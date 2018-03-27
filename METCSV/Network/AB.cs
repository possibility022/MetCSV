using METCSV.Database;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace METCSV.Network
{
    class AB : DownloadingThread
    {
        public AB(DownloadDone done)
        {
            this.done = done;
            this.fileName = "ab.csv";
        }

        public void downloadFile()
        {
            //startDownloading();
            task = new Task(startDownloading);
            task.Start();
        }

        private void startDownloading()
        {
            try
            {
                SetDownloadingResult(Global.Result.inProgress);
                if (File.Exists("OpenPop.dll") == false)
                    throw new FileNotFoundException("Nie znaleziono pliku OpenPop.dll");
                string zippedFile = "ab.zip";
                string folderToExtrac = "ExtractedFiles_AB";

                var client = new Pop3Client();
                client.Connect("mail.met.com.pl", 110, false);
                client.Authenticate("ab@met.com.pl", "^&$%GFDSW#asf");

                //deleteOldMessages(client);

                Message message = client.GetMessage(getNewestMessage(client));

                List<MessagePart> part = message.FindAllAttachments();
                MessagePart attachment = part.First();

                using (FileStream stream = new System.IO.FileStream(zippedFile, System.IO.FileMode.Create))
                {
                    BinaryWriter BinaryStream = new BinaryWriter(stream);
                    BinaryStream.Write(attachment.Body);
                    BinaryStream.Close();
                }

                if (Directory.Exists(folderToExtrac))
                    Directory.Delete(folderToExtrac, true);

                System.IO.Compression.ZipFile.ExtractToDirectory(zippedFile, folderToExtrac);
                DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
                fileName = dir.GetFiles()[0].FullName;
                client.Disconnect();
                SetDownloadingResult(Global.Result.complete);

            } catch (Exception ex)
            {
                Database.Log.log("Problem z pobieraniem pliku z AB. " + ex.Message);
                Log.Logging.LogException(ex);
                SetDownloadingResult(Global.Result.faild);
            }

            done();
        }

        private void deleteOldMessages(Pop3Client client)
        {
            int count = client.GetMessageCount();

            int newerMessage = getNewestMessage(client);

            for (int i = 1; i <= count; i++)
            {
                if (i != newerMessage) client.DeleteMessage(i);
            }
        }

        private int getNewestMessage(Pop3Client client)
        {
            int newerMessage = 1;
            int count = client.GetMessageCount();

            Message message = client.GetMessage(newerMessage);
            var newestDateTime = parseDate(message.Headers.Date);

            if (count > 1)
            {
                for (int i = 2; i <= count; i++)
                {
                    Message m = client.GetMessage(i);
                    var dt = parseDate(m.Headers.Date);

                    if (DateTime.Compare(newestDateTime, dt) < 1)
                    {
                        newerMessage = i;
                        newestDateTime = dt;
                    }
                }
            }

            return newerMessage;
        }

        private DateTime parseDate(string input)
        {
            string pattern = @"(([0-3][0-9])|([0-9])) [a-zA-Z]{1,4} 20[1-2][0-9] [0-9][0-9]:[0-9][0-9]";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(input);

            //20 Feb 2017 08:28

            CultureInfo provider = CultureInfo.GetCultures(CultureTypes.FrameworkCultures)[0];

            DateTime dateTime = new DateTime();

            try
            {
                dateTime = DateTime.ParseExact(matches[0].Value, "dd MMM yyyy hh:mm", provider);
                return dateTime;
            } catch (FormatException ex)
            { //to moglo sie zdazyc
            }

            try
            {
                dateTime = DateTime.ParseExact(matches[0].Value, "d MMM yyyy hh:mm", provider);
                return dateTime;
            } catch (FormatException ex)
            { Log.Logging.LogException(ex); }

            return new DateTime();
        }
    }
}
