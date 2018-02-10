using METCSV.WPF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
