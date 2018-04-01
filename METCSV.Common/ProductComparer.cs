using System.Collections.Generic;

namespace METCSV.Common
{
    public class ProductComparer : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            return string.Compare(x.SymbolSAP, y.SymbolSAP);
        }
    }
}
