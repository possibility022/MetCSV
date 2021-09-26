namespace MET.Proxy.Configuration
{
    public interface ISettings
    {
        ITechDataSettings TechDataSettings { get; }
        ILamaSettings LamaSettings { get; }
        IAbSettings AbSettings { get; }
        IMetSettings MetSettings { get; }
    }
}