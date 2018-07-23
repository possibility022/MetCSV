namespace METCSV.WPF.Configuration
{
    public class EngineSettings
    {
        private bool _offlineMode = true;

        public bool OfflineMode
        {
            get { return _offlineMode; }
            set { }
        }

        public bool SetProfits { get; set; } = true;
    }
}
