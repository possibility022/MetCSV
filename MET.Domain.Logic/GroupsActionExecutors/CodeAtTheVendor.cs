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
            HelpMe.ThrowIfNull(source);

            final.KodDostawcy = GetCode(source);
        }
    }
}
