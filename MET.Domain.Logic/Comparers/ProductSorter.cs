using System.Collections.Generic;

namespace MET.Domain.Logic.Comparers
{
    public class ProductSorter : IComparer<Product>
    {
        private static ProductByPartNumber byPartNumber = new ProductByPartNumber();
        private static ProductByProductPrice byPrice= new ProductByProductPrice();
        private static ProductBySapManuHash bySapManu = new ProductBySapManuHash();

        public int Compare(Product x, Product y)
        {
            int result = byPartNumber.Compare(x, y);

            if (result != 0)
            {
                return result;
            }

            result = byPrice.Compare(x, y);

            if (result != 0)
            {
                return result;
            }

            result = bySapManu.Compare(x, y);

            return result;
        }
    }
}
