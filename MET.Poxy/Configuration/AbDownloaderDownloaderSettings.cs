namespace MET.Proxy.Configuration
{
    public class AbDownloaderDownloaderSettings : IAbDownloaderSettings, IAbReaderSettings
    {
        public string ZippedFile { get; set; } = "ab.zip";

        public string FolderToExtract { get; set; } = "ExtractedFiles_AB";

        public string EmailServerAddress { get; set; } = "mail.met.com.pl";

        public int EmailServerPort { get; set; } = 110;

        public bool EmailServerUseSSL { get; set; } = false;

        public string EmailLogin { get; set; } = string.Empty;

        public string EmailPassword { get; set; } = string.Empty;

        public bool DeleteOldMessages { get; set; } = true;

        public string CsvFileEncoding { get; set; } = "windows-1250";

        public string CsvDelimiter { get; set; } = ";";

        public string SAPPrefix { get; set; } = "AB";
        public string DateTimeFormat1 { get; set; } = "dd MMM yyyy hh:mm";
        public string DateTimeFormat2 { get; set; } = "d MMM yyyy hh:mm";
        public string DateTimeRegexPattern { get; set; } = @"(([0-3][0-9])|([0-9])) [a-zA-Z]{1,4} 20[1-2][0-9] [0-9][0-9]:[0-9][0-9]";
    }
}
