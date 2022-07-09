using MET.Domain.Logic.Models;
using System.Linq;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    internal class CategoryDomain : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            var metProduct = productGroup.MetProducts.FirstOrDefault();
            if (metProduct != null && metProduct.Kategoria != string.Empty)
            {
                productGroup.FinalProduct.Kategoria = metProduct.Kategoria;
            }
        }
    }
}
