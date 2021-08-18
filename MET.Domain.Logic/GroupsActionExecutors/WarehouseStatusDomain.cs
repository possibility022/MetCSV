using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class WarehouseStatusDomain : IActionExecutor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            if (productGroup.FinalProduct.Kategoria == EndOfLiveDomain.EndOfLifeCategory)
            {
                productGroup.FinalProduct.StanMagazynowy = 0;
                return;
            }

            HelpMe.ThrowIfNull(productGroup.DataSourceProduct);

            productGroup.FinalProduct.StanMagazynowy = productGroup.DataSourceProduct.StanMagazynowy;
        }
    }
}
