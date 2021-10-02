using System.Collections.Generic;
using MET.Data.Models;

namespace METCSV.WPF.Models
{
    internal class CategoryCollection
    {
        public HashSet<string> Categories { get; }

        public Providers Provider { get; }

        public CategoryCollection(Providers provider, HashSet<string> categories)
        {   
            Provider = provider;
            Categories = categories;
        }
    }
}
