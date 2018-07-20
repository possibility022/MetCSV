using System.Collections.Generic;

namespace MET.Domain.Logic.Comparers
{
    public class ProductByPartNumber : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x.PartNumber > y.PartNumber)
                return 1;
            if (x.PartNumber < y.PartNumber)
                return -1;
            else
                return 0;
        }
    }
}
