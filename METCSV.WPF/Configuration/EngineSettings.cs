using System.Reflection;

namespace METCSV.WPF.Configuration
{
    public class EngineSettings
    {
        private bool _offlineMode = false;

        public bool OfflineMode
        {
            get { return _offlineMode; }
            set { }
        }

        public bool SetProfits { get; set; } = true;
        public string NewVersionURL { get; set; } = "http://tbsys.ddns.net/metcsv/version.xml";
    }
}
