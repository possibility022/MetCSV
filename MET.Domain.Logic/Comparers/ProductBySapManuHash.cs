using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic.Comparers
{
    public class ProductBySapManuHash : IComparer<Product>
    {
        public int Compare(Product x, Product y) => string.Compare(x.SapManuHash, y.SapManuHash);

    }
}
