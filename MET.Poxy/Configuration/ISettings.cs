namespace MET.Proxy.Configuration
{
    public interface ISettings
    {
        ITechDataDownloaderSettings TechDataDownloaderSettings { get; }
        ILamaDownloaderSettings LamaDownloaderSettings { get; }
        IAbDownloaderSettings AbDownloaderSettings { get; }
        IMetDownloaderSettings MetDownloaderSettings { get; }

        ITechDataReaderSettings TechDataReaderSettings { get; }

        ILamaReaderSettings LamaReaderSettings { get; }

        IAbReaderSettings AbReaderSettings { get; }
    }
}