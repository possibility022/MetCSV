using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic.Comparers
{
    public class ProductByProductPrice : IComparer<ICheapestProductDomain>
    {
        public int Compare(ICheapestProductDomain x, ICheapestProductDomain y)
        {
            if (x.CenaNetto > y.CenaNetto)
                return 1;
            if (x.CenaNetto < y.CenaNetto)
                return -1;
            else
                return 0;
        }
    }
}
