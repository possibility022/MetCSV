using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public interface IActionExecutor
    {
        void ExecuteAction(ProductGroup productGroup);
    }
}
