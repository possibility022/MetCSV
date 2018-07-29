using AutoUpdaterDotNET;

namespace MET.Proxy.AutoUpdate
{
    public static class PerformUpdate
    {
        public static void Update()
        {
            AutoUpdater.Start("http://192.168.0.6/metcsv/version.xml");
        }
    }
}
