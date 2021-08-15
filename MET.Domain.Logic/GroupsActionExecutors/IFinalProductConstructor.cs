namespace MET.Domain.Logic.GroupsActionExecutors
{
    public interface IFinalProductConstructor
    {
        void ExecuteAction(Product source, Product final);
    }
}