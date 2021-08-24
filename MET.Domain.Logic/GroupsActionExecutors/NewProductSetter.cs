using MET.Data.Models;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    class NewProductSetter : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            productGroup.SetFinalProduct(new Product(Providers.None));
        }
    }
}
