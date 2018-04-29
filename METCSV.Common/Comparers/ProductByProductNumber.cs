using System.Collections.Generic;

namespace METCSV.Common.Comparers
{
    public class ProductByProductNumber : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x.CenaZakupuNetto > y.CenaZakupuNetto)
                return 1;
            if (x.CenaZakupuNetto < y.CenaZakupuNetto)
                return -1;
            else
                return 0;
        }
    }
}
