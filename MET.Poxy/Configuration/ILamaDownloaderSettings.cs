namespace MET.Proxy.Configuration
{
    public interface ILamaDownloaderSettings
    {
        public string XmlFile { get; }

        public string CsvFile { get; }

        public string Url { get; }

        public string Login { get; }

        public string Password { get; }

        public string Request { get; }
    }
}