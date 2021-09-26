using System.Threading;

namespace MET.Proxy
{
    public interface IDownloader
    {
        bool StartDownloading(CancellationToken toknen);

        string[] DownloadedFiles { get; }
    }
}
