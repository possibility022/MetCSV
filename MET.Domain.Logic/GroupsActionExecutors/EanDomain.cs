using MET.Data.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class EanDomain : IFinalProductConstructor
    {
        public void ExecuteAction(Product source, Product final)
        {
            if (source.Ean != null)
                final.Ean = source.Ean;
        }
    }
}
