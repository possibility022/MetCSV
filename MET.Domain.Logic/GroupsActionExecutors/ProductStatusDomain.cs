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

            if (source.PartNumber == "0B31236_||_HGST")
            {
                var v = 0;
            }

            if (source.StanMagazynowy < 1)
                final.StatusProduktu = false;
            else
                final.StatusProduktu = source.StatusProduktu;
        }
    }
}
