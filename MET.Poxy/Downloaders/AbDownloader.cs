using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MET.Proxy.Configuration;
using METCSV.Common;
using System.IO.Compression;
using MailKit.Net.Pop3;
using MET.Data.Models;
using MimeKit;

namespace MET.Proxy
{
    public class AbDownloader : DownloaderBase
    {
        readonly string EmailServerAddress;
        readonly int EmailServerPort;
        readonly bool UseSSL;

        readonly string EmailLogin;
        readonly string EmailPassword;

        readonly string ZippedFile;
        readonly string FolderToExtract;

        readonly bool DeleteOld;
        

        private DateTimeParser dateTimeParser;

        public AbDownloader(IAbSettings settings, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            EmailServerAddress = settings.EmailServerAddress;
            EmailServerPort = settings.EmailServerPort;
            UseSSL = settings.EmailServerUseSSL;

            EmailLogin = settings.EmailLogin;
            EmailPassword = settings.EmailPassword;

            ZippedFile = settings.ZippedFile;
            FolderToExtract = settings.FolderToExtract;
            DeleteOld = settings.DeleteOldMessages;

            dateTimeParser = new DateTimeParser(settings.DateTimeRegexPattern, settings.DateTimeFormat1,
                settings.DateTimeFormat2);
        }

        Pop3Client _client;

        public override Providers Provider => Providers.AB;

        public ICollection<Product> GetResults { get; private set; }

        public EventHandler OnDownloadingFinish { get; private set; }

        protected override void Download()
        {
            Status = OperationStatus.InProgress;

            DownloadedFiles = new[] { string.Empty };

            using (_client = new Pop3Client())
            {
                _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                _client.Connect(
                    EmailServerAddress,
                    EmailServerPort,
                    UseSSL,
                    CancellationToken);
                
                _client.Authenticate(EmailLogin, EmailPassword, CancellationToken);

                if (DeleteOld)
                {
                    LogInfo("This is not implemented yet");
                    //deleteOldMessages(client); //todo implement and allow to manage from config.
                }

                int theLatestMessage = GetNewestMessageIndex(_client);

                if (theLatestMessage < 0)
                {
                    Status = OperationStatus.Faild;
                    LogInfo("Nie znaleziono najnowszej wiadomości.");
                    return;
                }

                var message = _client.GetMessage(theLatestMessage, CancellationToken);

                var attachment = message.Attachments.First() as MimePart;

                if (attachment == null)
                {
                    Status = OperationStatus.Faild;
                    LogError("Problem z załącznikiem w mailu. Sprawdz czy załącznik istnieje");
                }

                ExportAttachmentToFile(ZippedFile, attachment);

                if (Directory.Exists(FolderToExtract))
                    Directory.Delete(FolderToExtract, true);

                ZipFile.ExtractToDirectory(ZippedFile, FolderToExtract);
                DirectoryInfo dir = new DirectoryInfo(FolderToExtract);
                DownloadedFiles[0] = dir.GetFiles().First().FullName;
                _client.Disconnect(true);
            }

            ThrowIfCanceled();
        }

        private void ExportAttachmentToFile(string zippedFile, MimePart attachment)
        {
            using (var stream = new StreamWriter(zippedFile))
            {
                attachment.Content.DecodeTo(stream.BaseStream, CancellationToken);
            }
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
