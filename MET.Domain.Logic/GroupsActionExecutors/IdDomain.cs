using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class IdDomain : IActionExecutor
    {
        private readonly bool ignoreDuplicates;
        private readonly Providers[] providersPriority;

        public IdDomain(bool ignoreDuplicates)
        {
            this.ignoreDuplicates = ignoreDuplicates;
            providersPriority = new[] { Providers.Lama, Providers.TechData, Providers.AB };
        }


        public void ExecuteAction(ProductGroup productGroup)
        {
            if (!productGroup.MetProducts.Any())
            {
                productGroup.FinalProduct.ID = null;
            }
            else
            {
                int idToSet;

                if (productGroup.MetProducts.Select(r => r.ID).Distinct().Count() > 1)
                {
                    var sourceOfId = GetSourceByVendor(productGroup.MetProducts);
                    if (sourceOfId?.ID != null)
                    {
                        idToSet = sourceOfId.ID.Value;
                        productGroup.FinalProduct.ID = idToSet;
                        return;
                    }
                }
                idToSet = productGroup.MetProducts.First().ID.Value;
                productGroup.FinalProduct.ID = idToSet;
            }
        }

        private Product GetSourceByVendor(IReadOnlyList<Product> metProducts)
        {
            foreach (var p in providersPriority)
            {
                var prod = metProducts.FirstOrDefault(r => r.OriginalSource == p);
                if (prod != null)
                    return prod;
            }

            return null;
        }
    }
}
