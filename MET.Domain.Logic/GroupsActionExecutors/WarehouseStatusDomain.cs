using System;
using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class WarehouseStatusDomain : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            if (productGroup.MetProducts.Any(r =>
                r.Kategoria == EndOfLiveDomain.EndOfLifeCategory))
            {
                productGroup.FinalProduct.StanMagazynowy = 0;
                return;
            }

            HelpMe.ThrowIfNull(productGroup.CheapestProduct);

            productGroup.FinalProduct.StanMagazynowy = productGroup.CheapestProduct.StanMagazynowy;
        }
    }
}
