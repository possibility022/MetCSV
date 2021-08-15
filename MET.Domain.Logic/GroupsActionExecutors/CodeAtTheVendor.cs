using System;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class CodeAtTheVendor: IFinalProductConstructor
    {
        private string GetCode(Product product)
        {
            return $"{product.Provider}_{product.KodDostawcy}";
        }

        public void ExecuteAction(Product source, Product final)
        {
            if (source == null)
                throw new InvalidOperationException(
                    "Can not set 'Code at the vendor' as the cheapest product is null.");

            final.KodDostawcy = GetCode(source);
        }
    }
}
