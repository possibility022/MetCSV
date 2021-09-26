namespace MET.Proxy.Configuration
{
    public interface ILamaSettings
    {
        public string XmlFile { get; }

        public string CsvFile { get; }

        public string Url { get; }

        public string Login { get; }

        public string Password { get; }

        public string Request { get; }
        public string CsvDelimiter { get; }
        public string SAPPrefix { get; }
        public string CsvFileEncoding { get; }
    }
}