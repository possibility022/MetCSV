namespace METCSV.WPF.Configuration
{
    public class AbDownloaderSettings
    {

        public string ZippedFile { get; set; } = "ab.zip";

        public string FolderToExtract { get; set; } = "ExtractedFiles_AB";

        public string EmailServerAddress { get; set; } = "mail.met.com.pl";

        public int EmailServerPort { get; set; } = 110;

        public bool EmailServerUseSSL { get; set; } = false;

        public string EmailLogin { get; set; } = string.Empty;

        public string EmailPassword { get; set; } = string.Empty;

        public bool DeleteOldMessages { get; set; } = true;
    }
}
