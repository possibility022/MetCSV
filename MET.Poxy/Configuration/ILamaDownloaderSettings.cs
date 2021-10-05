namespace MET.Proxy.Configuration
{
    public interface ILamaDownloaderSettings
    {
        public string XmlFile { get; }

        public string ManufacturersXmlFile { get; }

        public string Url { get; }

        public string Login { get; }

        public string Password { get; }

        public string Request { get; }
        public string ManufacturersRequest { get; }
    }
}