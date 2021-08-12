using System;
using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class IdDomain : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            if (!productGroup.MetProducts.Any())
            {
                productGroup.FinalProduct.ID = null;
            }
            else
            {
                if (productGroup.MetProducts.Select(r => r.ID).Distinct().Count() > 1)
                    throw new InvalidOperationException(
                        "Wygląda na to, że w pliku met jest wiele produktów dla tej samej grupy. Nie wiem którego Id użyć.");
                var idToSet = productGroup.MetProducts.First().ID;

                productGroup.FinalProduct.ID = idToSet;
            }
        }
    }
}
