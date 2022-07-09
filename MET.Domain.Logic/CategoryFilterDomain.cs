using MET.Data.Models;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.Common.Formatters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class CategoryFilterDomain
    {
        private IObjectFormatterConstructor<object> constructor;
        private List<Task> tasks;
        private IReadOnlyCollection<IgnoreCategory> ignoreCategories;

        public CategoryFilterDomain(IObjectFormatterConstructor<object> objectFormatter)
        {
            this.constructor = objectFormatter;
        }

        internal async Task RemoveProductsWithIgnoredCategory(IProductsCollection product, IReadOnlyCollection<IgnoreCategory> ignoreCategories)
        {
            tasks = new List<Task>();
            this.ignoreCategories = ignoreCategories;

            AddTaskToList(product, Providers.AB);
            AddTaskToList(product, Providers.Lama);
            AddTaskToList(product, Providers.TechData);
            // AddTaskToList(tasks, products.MetProducts); // We don't want to edit met list here.

            await Task.WhenAll(tasks);

            tasks = null;
            this.ignoreCategories = null;
        }

        private void AddTaskToList(IProductsCollection products, Providers provider)
        {
            var productsList = products.SingleOrDefault(r => r.Key == provider).Value;
            if (productsList != null)
                tasks.Add(RemoveProductsWithCategory(productsList, provider));

        }

        private Task RemoveProductsWithCategory(ICollection<Product> products, Providers provider)
        {
            return Task.Run(() =>
            {
                var toRemove = new List<Product>();
                var categoriesToIgnore = ignoreCategories
                    .Where(r => r.Provider == provider.ToString())
                    .Select(r => r.CategoryName)
                    .ToHashSet();

                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.Kategoria)
                     && categoriesToIgnore.Contains(product.Kategoria)
                    )
                    {
                        var logger = constructor.GetNewInstance();

                        logger.WriteLine("Removing object as category is on black list");
                        logger.WriteObject(product);

                        toRemove.Add(product);
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
