using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class SourceProductSelector : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            if (productGroup.FinalProduct.Kategoria == EndOfLiveDomain.EndOfLifeCategory)
            {
                productGroup.DataSourceProduct = productGroup.MetProducts.First();
            }
            else
            {
                productGroup.DataSourceProduct = productGroup.CheapestProduct;
            }
        }
    }
}
