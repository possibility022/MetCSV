using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class ProductStatusDomain : IActionExecutor, IFinalProductConstructor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            if (productGroup.MetProducts.Any())
            {
                var status = productGroup.MetProducts.First().StatusProduktu;
                productGroup.DataSourceProduct.StatusProduktu = status;
            }
        }

        public void ExecuteAction(Product source, Product final)
        {
            final.StatusProduktu = source.StatusProduktu;
        }
    }
}
