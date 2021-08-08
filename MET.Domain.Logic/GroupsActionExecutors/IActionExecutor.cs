using System.Collections.Generic;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public interface IActionExecutor
    {
        void ExecuteAction(string partNumber, ICollection<Product> vendorProducts, ICollection<Product> metProducts, IObjectFormatter<object> objectFormatter);
    }
}
