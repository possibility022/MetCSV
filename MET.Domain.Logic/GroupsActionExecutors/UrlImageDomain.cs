using MET.Data.Models;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class UrlImageDomain : IActionExecutor, IFinalProductConstructor
    {
        public void ExecuteAction(ProductGroup productGroup)
        {
            
        }

        public void ExecuteAction(Product source, Product final)
        {
            final.UrlZdjecia = string.Empty; //just for now
        }
    }
}
