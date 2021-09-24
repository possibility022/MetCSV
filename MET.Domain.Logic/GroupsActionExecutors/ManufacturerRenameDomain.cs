using System;
using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class ManufacturerRenameDomain : IPreGrouppingExecuters
    {
        private IReadOnlyDictionary<string, string> renameDictionary;
        
        public void SetDictionary(IReadOnlyDictionary<string, string> dictionary)
        {
            renameDictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public void Execute(Product product)
        {
            if (renameDictionary.ContainsKey(product.NazwaProducenta))
            {
                var newName = renameDictionary[product.NazwaProducenta];
                product.NazwaProducenta = newName;
            }
        }
    }
}
