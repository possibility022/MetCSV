using System.Collections.Generic;
using System.Threading.Tasks;
using MET.Domain.Logic.Models;
using METCSV.Common;

namespace MET.Domain.Logic
{
    public class ProductFilterDomain
    {
        public async Task RemoveProductsWithSpecificCode(Products products)
        {
            var tasks = new List<Task>();

            AddTaskToList(tasks, products.AbProducts);
            AddTaskToList(tasks, products.LamaProducts);
            AddTaskToList(tasks, products.MetProducts);
            AddTaskToList(tasks, products.TechDataProducts);
            AddTaskToList(tasks, products.AbProducts_Old);
            AddTaskToList(tasks, products.LamaProducts_Old);
            AddTaskToList(tasks, products.TechDataProducts_Old);

            await Task.WhenAll(tasks);
        }

        private void AddTaskToList(List<Task> tasks, ICollection<Product> products)
        {
            if (products != null)
                tasks.Add(RemoveProductsWithSpecificCode(products));
        }

        private Task RemoveProductsWithSpecificCode(ICollection<Product> products)
        {
            return Task.Run(() =>
            {
                var toRemove = new HashSet<Product>();

                foreach (var product in products)
                {
                    var lastIndex = product.OryginalnyKodProducenta?.LastIndexOf('?');
                    if (lastIndex >= 0)
                    {
                        if ((product.OryginalnyKodProducenta.Length -1) > (lastIndex))
                        {
                            toRemove.Add(product);
                        }
                        else
                        {
                            Log.Info($"Product has '?' at the end but nothing more. {product}");
                        }
                    }

                    
                }

                foreach (var product in toRemove)
                {
                    Log.Info($"Removing product {product}");
                    products.Remove(product);
                }
            });
        }


    }
}
