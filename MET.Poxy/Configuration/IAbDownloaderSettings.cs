namespace MET.Proxy.Configuration
{
    public interface IAbDownloaderSettings
    {
        public string ZippedFile { get; }

        public string FolderToExtract { get; }

        public string EmailServerAddress { get; }

        public int EmailServerPort { get; }

        public bool EmailServerUseSSL { get; }

        public string EmailLogin { get; }

        public string EmailPassword { get; }

        public bool DeleteOldMessages { get; }
        
        public string SAPPrefix { get; }
        public string DateTimeFormat1 { get; }
        public string DateTimeFormat2 { get; }
        public string DateTimeRegexPattern { get; }
    }
}