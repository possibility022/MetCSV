using System.Threading.Tasks;
using MET.Proxy.ProductProvider;

namespace MET.Proxy.Extensions
{
    public static class ProductProviderBaseExtensions
    {
        public static Task<bool> DownloadAndLoadAsync(this IProductProvider productProvider)
        {
            Task<bool> task = new Task<bool>(productProvider.DownloadAndLoad);
            task.Start();
            return task;
        }
    }
}
