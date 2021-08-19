using System.Collections.Generic;
using System.Linq;

namespace MET.Domain.Logic
{
    public class MetCustomProductsDomain
    {
        public List<Product> ModifyList(ICollection<Product> metProducts)
        {
            var toRemove = new HashSet<Product>();
            foreach (var product in metProducts)
            {
                if (product.SymbolSAP.StartsWith("MET"))
                    toRemove.Add(product);
            }

            foreach (var product in toRemove)
            {
                metProducts.Remove(product);
            }

            return toRemove.ToList();
        }
    }
}
