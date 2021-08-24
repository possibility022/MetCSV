using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic.Comparers
{
    public class ProductByProductPrice : IComparer<Product>
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

        // TODO Jeśli cena jest taka sama to zastosuj priorytety!
        // TODO zaimplementuj priorytety
    }
}
