using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using MailKit.Net.Pop3;
using MET.Data.Models;
using MET.Proxy.Configuration;
using METCSV.Common;
using MimeKit;

namespace MET.Proxy.Downloaders
{
    public class AbDownloader : DownloaderBase
    {
        private readonly string emailServerAddress;
        private readonly int emailServerPort;
        private readonly bool useSsl;

        private readonly string emailLogin;
        private readonly string emailPassword;

        private readonly string zippedFile;
        private readonly string folderToExtract;

        private readonly bool deleteOld;
        

        private readonly DateTimeParser dateTimeParser;

        private Pop3Client client;
        public override Providers Provider => Providers.AB;


        public AbDownloader(IAbDownloaderSettings downloaderSettings, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            emailServerAddress = downloaderSettings.EmailServerAddress;
            emailServerPort = downloaderSettings.EmailServerPort;
            useSsl = downloaderSettings.EmailServerUseSSL;

            emailLogin = downloaderSettings.EmailLogin;
            emailPassword = downloaderSettings.EmailPassword;

            zippedFile = downloaderSettings.ZippedFile;
            folderToExtract = downloaderSettings.FolderToExtract;
            deleteOld = downloaderSettings.DeleteOldMessages;

            dateTimeParser = new DateTimeParser(downloaderSettings.DateTimeRegexPattern, downloaderSettings.DateTimeFormat1,
                downloaderSettings.DateTimeFormat2);
        }
        
        protected override bool Download()
        {
            DownloadedFiles = new[] { string.Empty };

            using (client = new Pop3Client())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(
                    emailServerAddress,
                    emailServerPort,
                    useSsl,
                    CancellationToken);
                
                client.Authenticate(emailLogin, emailPassword, CancellationToken);

                if (deleteOld)
                {
                    LogInfo("This is not implemented yet");
                    //deleteOldMessages(client); //todo implement and allow to manage from config.
                }

                int theLatestMessage = GetNewestMessageIndex(client);

                if (theLatestMessage < 0)
                {
                    LogInfo("Nie znaleziono najnowszej wiadomości.");
                    return false;
                }

                var message = client.GetMessage(theLatestMessage, CancellationToken);

                var attachment = message.Attachments.First() as MimePart;

                if (attachment == null)
                {
                    LogError("Problem z załącznikiem w mailu. Sprawdz czy załącznik istnieje");
                    return false;
                }

                ExportAttachmentToFile(zippedFile, attachment);

                if (Directory.Exists(folderToExtract))
                    Directory.Delete(folderToExtract, true);

                ZipFile.ExtractToDirectory(zippedFile, folderToExtract);
                DirectoryInfo dir = new DirectoryInfo(folderToExtract);
                DownloadedFiles[0] = dir.GetFiles().First().FullName;
                client.Disconnect(true);
            }

            ThrowIfCanceled();
            return true;
        }

        private void ExportAttachmentToFile(string zippedFile, MimePart attachment)
        {
            using var stream = new StreamWriter(zippedFile);
            attachment.Content.DecodeTo(stream.BaseStream, CancellationToken);
        }

        private int GetNewestMessageIndex(Pop3Client client)
        {
            int newerMessage = -1;
            DateTime date = new DateTime(1, 1, 1);
            
            var headers = client.GetMessageHeaders(0, client.Count, CancellationToken);

            for (int i = 0; i < headers.Count; i++)
            {
                ThrowIfCanceled();

                HeaderList header = headers[i];
                if (header.Contains(HeaderId.Date))
                {
                    var headerValue = header[header.IndexOf(HeaderId.Date)].Value;
                    var emailDate = dateTimeParser.ParseDateTime(headerValue);
                    if (emailDate > date)
                    {
                        date = emailDate;
                        newerMessage = i;
                    }
                }
            }
            
            return newerMessage;
        }
    }
}
