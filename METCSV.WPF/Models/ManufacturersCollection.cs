using MET.Data.Models;

namespace METCSV.WPF.Models
{
    internal class ManufacturersCollection
    {
        public HashSet<string> Manufacturers { get; }

        public Providers Provider { get; }

        public ManufacturersCollection (Providers provider, HashSet<string> manufacturers)
        {
            Provider = provider;
            Manufacturers = manufacturers;
        }
    }
}
