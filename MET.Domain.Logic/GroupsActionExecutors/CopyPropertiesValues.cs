using MET.Data.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class CopyPropertiesValues : IFinalProductConstructor
    {
        public void ExecuteAction(Product source, Product final)
        {
            if (final.ID == null)
                final.ID = source.ID;
            
            if (final.EndOfLive == true)
                return;

            final.StanMagazynowy = source.StanMagazynowy;
            final.Hidden = source.Hidden;
            final.Provider = source.Provider;
        }
    }
}
