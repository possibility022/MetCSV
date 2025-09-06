using System.Collections.Generic;
using MET.Data.Models;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic
{
    public class FinalListCombineDomain
    {
        public List<Product> CreateFinalList(IEnumerable<ProductGroup> productGroups)
        {
            var list = new List<Product>();
            
            foreach (var productGroup in productGroups)
            {
                list.Add(productGroup.FinalProduct);
            }

            return list;
        }
    }
}
