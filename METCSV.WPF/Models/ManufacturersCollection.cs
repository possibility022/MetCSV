using MET.Domain; using MET.Workflows;
using System.Collections.Generic;

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
