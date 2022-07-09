using System.Collections.Generic;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public class ProductFilterDomain
    {
        private readonly IObjectFormatterConstructor<object> constructor;

        public ProductFilterDomain(IObjectFormatterConstructor<object> constructor)
        {
            this.constructor = constructor;
        }

        public Task RemoveProductsWithSpecificCode(Products products)
        {
            var tasks = new List<Task>();

            AddTaskToList(tasks, products.AbProducts);
            AddTaskToList(tasks, products.LamaProducts);
            AddTaskToList(tasks, products.MetProducts);
            AddTaskToList(tasks, products.TechDataProducts);
            AddTaskToList(tasks, products.AbProducts_Old);
            AddTaskToList(tasks, products.LamaProducts_Old);
            AddTaskToList(tasks, products.TechDataProducts_Old);

            return Task.WhenAll(tasks);
        }

        public Task RemoveProductsWithSpecificCode(IProductsCollection productsList)
        {
            var tasks = new List<Task>(productsList.Count);

            foreach (var collection in productsList)
            {
                AddTaskToList(tasks, collection.Value);
            }

            return Task.WhenAll(tasks);
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
                    var logger = constructor.GetNewInstance();
                    var lastIndex = product.OryginalnyKodProducenta?.LastIndexOf('?');
                    if (lastIndex >= 0)
                    {
                        if ((product.OryginalnyKodProducenta.Length -1) > (lastIndex))
                        {
                            toRemove.Add(product);
                            continue;
                        }
                        else
                        {
                            toRemove.Add(product);
                            Log.Info($"Product has '?' at the end but nothing more. {product}");
                            continue;
                        }
                    }

                    if (product.OryginalnyKodProducenta.Contains("?TN"))
                    {
                        product.StatusProduktu = false;
                        product.CenaZakupuNetto = 0;
                        product.SetCennaNetto(0);
                        product.Kategoria = "EOL_TN";
                        toRemove.Add(product);



                        logger.WriteLine("Produkt z oryginalnym kodem producenta: [] posiadał wartość ?TN. Ustawiam Status Produktu, Cene zakupu netto, Cene netto i kategorię. Produkt zostanie również usunięty z listy wejściowej.");
                        logger.WriteLine("Produkt po zmianach:");
                        logger.WriteObject(product);
                        continue;
                    }

                    if (product.Hidden)
                    {
                        toRemove.Add(product);
                        logger.WriteLine("Produkt ukryty.");
                        continue;
                    }

                    if (product.CenaZakupuNetto <= 0)
                    {
                        toRemove.Add(product);
                        logger.WriteLine($"Usuwam produkt {product} jako, że ma cene mniejszą lub równą 0.");
                        continue;
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
