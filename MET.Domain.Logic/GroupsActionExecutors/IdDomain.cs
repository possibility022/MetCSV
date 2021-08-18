using System;
using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class IdDomain : IActionExecutor
    {
        private readonly bool ignoreDuplicates;

        public IdDomain(bool ignoreDuplicates)
        {
            this.ignoreDuplicates = ignoreDuplicates;
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            if (!productGroup.MetProducts.Any())
            {
                productGroup.FinalProduct.ID = null;
            }
            else
            {
                if (productGroup.MetProducts.Select(r => r.ID).Distinct().Count() > 1)
                {
                    if (ignoreDuplicates)
                    {
                        productGroup.ObjectFormatter.WriteLine(
                            "Mamy więcej niż jedno Id dla tego produktu. Flaga jest ustawiona na ignorowanie takich przypadków. Id będzie nullem.");
                        return;
                    }

                    throw new InvalidOperationException(
                        "Wygląda na to, że w pliku met jest wiele produktów dla tej samej grupy. Nie wiem którego Id użyć.");
                }

                var idToSet = productGroup.MetProducts.First().ID;

                productGroup.FinalProduct.ID = idToSet;
            }
        }
    }
}
