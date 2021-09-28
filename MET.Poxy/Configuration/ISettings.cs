namespace MET.Proxy.Configuration
{
    public interface ISettings
    {
        ITechDataSettings TechDataSettings { get; }
        ILamaSettings LamaSettings { get; }
        IAbDownloaderSettings AbDownloaderSettings { get; }
        IMetSettings MetSettings { get; }
    }
}