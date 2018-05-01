using System.Collections.Generic;

namespace METCSV.Common.Comparers
{
    public class ProductBySapManuHash : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x.SapManuHash > y.SapManuHash)
                return 1;
            if (x.SapManuHash < y.SapManuHash)
                return -1;
            else
                return 0;
        }
    }
}
