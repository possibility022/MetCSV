using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using METCSV.WPF.Enums;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace METCSV.WPF.Downloaders
{
    class AbDownloader : DownloaderBase
    {
        public AbDownloader(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }

        public ICollection<Product> GetResults { get; private set; }

        public EventHandler OnDownloadingFinish { get; private set; }

        public EventHandler OnDownloadingStatusChanged { get; private set; }

        public OperationStatus Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    OnDownloadingStatusChanged?.Invoke(this, EventArgs.Empty);
                    _status = value;
                }
            }
        }

        private OperationStatus _status;

        protected override void Download()
        {
            Status = OperationStatus.InProgress;
            if (File.Exists("OpenPop.dll") == false)
                throw new FileNotFoundException("Nie znaleziono pliku OpenPop.dll");
            string zippedFile = "ab.zip";
            string folderToExtrac = "ExtractedFiles_AB";

            DownloadedFiles = new[] { string.Empty };

            using (var client = new Pop3Client())
            {
                client.Connect("mail.met.com.pl", 110, false); //todo encrypt
                client.Authenticate("ab@met.com.pl", "^&$%GFDSW#asf"); //todo move it to config

                //deleteOldMessages(client);

                int theLatestMessage = GetNewestMessage(client);

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Message message = client.GetMessage(theLatestMessage);

                List<MessagePart> part = message.FindAllAttachments();
                MessagePart attachment = part.First();

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                ExportAttachmentToFile(zippedFile, attachment);

                if (Directory.Exists(folderToExtrac))
                    Directory.Delete(folderToExtrac, true);

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                ZipFile.ExtractToDirectory(zippedFile, folderToExtrac);
                DirectoryInfo dir = new DirectoryInfo(folderToExtrac);
                DownloadedFiles[0] = dir.GetFiles()[0].FullName;
                client.Disconnect();
            }
        }

        private static void ExportAttachmentToFile(string zippedFile, MessagePart attachment)
        {
            using (FileStream stream = new FileStream(zippedFile, FileMode.Create))
            {
                BinaryWriter binaryStream = new BinaryWriter(stream);
                binaryStream.Write(attachment.Body);
                binaryStream.Close();
            }
        }

        private int GetNewestMessage(Pop3Client client)
        {
            int newerMessage = 1;
            int count = client.GetMessageCount();

            Message message = client.GetMessage(newerMessage);
            var newestDateTime = ParseDate(message.Headers.Date);

            if (count > 1)
            {
                for (int i = 2; i <= count; i++)
                {
                    var header = client.GetMessageHeaders(i);
                    var dt = ParseDate(header.Date);

                    if (DateTime.Compare(newestDateTime, dt) < 1)
                    {
                        newerMessage = i;
                        newestDateTime = dt;
                    }
                }
            }

            return newerMessage;
        }

        private DateTime ParseDate(string input)
        {
            string pattern = @"(([0-3][0-9])|([0-9])) [a-zA-Z]{1,4} 20[1-2][0-9] [0-9][0-9]:[0-9][0-9]";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(input);

            //20 Feb 2017 08:28

            CultureInfo provider = CultureInfo.GetCultures(CultureTypes.FrameworkCultures)[0];

            DateTime dateTime;

            try
            {
                dateTime = DateTime.ParseExact(matches[0].Value, "dd MMM yyyy hh:mm", provider);
                return dateTime;
            }
            catch (FormatException)
            { //to moglo sie zdazyc
            }

            try
            {
                dateTime = DateTime.ParseExact(matches[0].Value, "d MMM yyyy hh:mm", provider);
                return dateTime;
            }
            catch (FormatException)
            { }

            return new DateTime();
        }
    }
}
