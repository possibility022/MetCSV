using MET.Data.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class SapNumberDomain : IFinalProductConstructor
    {
        public const string Prefix = "SAP";

        public void ExecuteAction(Product source, Product final)
        {
            HelpMe.ThrowIfNull(source);
            HelpMe.ThrowIfNull(final);

            final.SymbolSAP = GetSapNumber(source);
        }

        private string GetSapNumber(Product product)
        {
            return $"{Prefix}_{product.SymbolSAP}";
        }
    }
}
