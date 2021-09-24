using System;
using System.Collections.Generic;
using MET.Data.Models;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class ManufacturerRenameDomain : IActionExecutor
    {
        private IReadOnlyDictionary<string, string> renameDictionary;

        public ManufacturerRenameDomain()
        {
        }

        public void SetDictionary(IReadOnlyDictionary<string, string> dictionary)
        {
            renameDictionary = dictionary;
        }


        public void ExecuteAction(ProductGroup productGroup)
        {
            foreach (var product in productGroup.MetProducts)
            {
                Rename(product, productGroup.ObjectFormatter);
            }

            foreach (var product in productGroup.VendorProducts)
            {
                Rename(product, productGroup.ObjectFormatter);
            }
        }

        private void Rename(Product product, IObjectFormatter<object> formatter)
        {
            if (renameDictionary.ContainsKey(product.NazwaProducenta))
            {
                var newName = renameDictionary[product.NazwaProducenta];
                formatter.WriteLine($"Zmianiam nazwe producenta z {product.NazwaProducenta} na {newName}");
                product.NazwaProducenta = newName;
            }
        }
    }
}
