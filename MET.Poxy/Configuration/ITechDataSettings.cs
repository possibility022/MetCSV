namespace MET.Proxy.Configuration
{
    public interface ITechDataSettings
    {
        public string Login { get; }

        public string Password { get; }

        public string FtpAddress { get; }

        public string Pattern { get; }

        public string FolderToExtract { get; }

        public string CsvMaterials { get; }

        public string CsvPrices { get; }

        public string CsvDelimiter { get; }

        public string SAPPrefix { get; }
    }
}