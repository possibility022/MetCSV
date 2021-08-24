using MET.Data.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class CopyPropertiesValues : IFinalProductConstructor
    {
        public void ExecuteAction(Product source, Product final)
        {
            final.ID = source.ID;
            final.StanMagazynowy = source.StanMagazynowy;
            final.Hidden = source.Hidden;
            final.Provider = source.Provider;
        }
    }
}
