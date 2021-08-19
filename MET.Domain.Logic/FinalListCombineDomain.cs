using System.Collections.Generic;
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

                if (productGroup.FinalProduct.SymbolSAP == null)
                {
                    var x = 0;
                }
                if (productGroup.FinalProduct.SymbolSAP?.Contains("WMHHIP0UC098653") == true)
                {
                    var a = 0;
                }

                list.Add(productGroup.FinalProduct);
            }

            return list;
        }
    }
}
