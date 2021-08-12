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

            if (productGroup.CheapestProduct == null)
                throw new InvalidOperationException(
                    "Cheapest product was not set for product group. We can not set warehouse status.");

            productGroup.FinalProduct.StanMagazynowy = productGroup.CheapestProduct.StanMagazynowy;
        }
    }
}
