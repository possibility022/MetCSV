using System.Reflection;

namespace METCSV.WPF.Configuration
{
    public class EngineSettings
    {
        private bool _offlineMode = true;

        public bool OfflineMode
        {
            get { return _offlineMode; }
            set { _offlineMode = value; }
        }

        public bool SetProfits { get; set; } = true;
        public string NewVersionURL { get; set; } = "http://tbsys.ddns.net/metcsv/version.xml";

        public int MaximumPriceErrorDifference { get; set; } = 20;
    }
}
