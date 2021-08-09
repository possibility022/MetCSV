using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MET.Domain.Logic.Comparers;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class CompareDomain : IActionExecutor
    {
        ConcurrentQueue<string> allPartNumbers;
        ConcurrentDictionary<string, IList<Product>> products;

        readonly ProductByProductPrice netPriceComparer = new ProductByProductPrice();

        private readonly IAllPartsNumberDomain allPartNumbersDomain;
        readonly IObjectFormatterConstructor<object> objectFormatter;

        public CompareDomain(IAllPartsNumberDomain allPartNumbersDomain, IObjectFormatterConstructor<object> objectFormatter)
        {
            this.allPartNumbersDomain = allPartNumbersDomain;
            this.objectFormatter = objectFormatter;
        }

        public void Compare(ConcurrentDictionary<string, IList<Product>> combinedCollection)
        {
            products = combinedCollection;
            allPartNumbers = new ConcurrentQueue<string>(allPartNumbersDomain.GetAllPartNumbers().Keys);

            var tasks = new Task[Environment.ProcessorCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(Compare);
            }

            tasks.StartAll();
            tasks.WaitAll();
        }

        private void Compare()
        {
            string partNumber;
            bool partNumberTaken;

            while ((partNumberTaken = allPartNumbers.TryDequeue(out partNumber)) || allPartNumbers.Count > 0)
            {
                if (partNumberTaken)
                {
                    var listTaken = products.TryGetValue(partNumber, out var listToCompare);

                    if (listTaken)
                    {
                        SelectOneProduct(listToCompare, partNumber);
                    }
                    else if (products.ContainsKey(partNumber))
                    {
                        allPartNumbers.Enqueue(partNumber);
                    }
                }
            }
        }

        private void SelectOneProduct(IList<Product> products, string partNumber)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            var formatter = objectFormatter.GetNewInstance();

            formatter.WriteLine($"Zaczynam porównywać listę produktów dla PartNumberu [{partNumber}]: ");
            formatter.WriteObject(products);


            // Todo move it to new domain
            //RemoveEmptyWarehouse(products, formatter);

            var availableProducts = products.Where(r => r.StanMagazynowy > 0).ToList();
            var includeAll = !products.Any();

            var workOnThisList = includeAll ? products : availableProducts;

            if (!includeAll)
            {
                formatter.WriteLine($"Wszystkie produkty dla {partNumber} są niedostępne");
            }

            formatter.WriteLine("Wybieram najtańszy produkt z listy:");
            formatter.WriteObject(workOnThisList);

            var cheapest = FindCheapestProduct(workOnThisList, includeAll);

            formatter.WriteLine($"Najtańszy produkt dla PartNumberu [{partNumber}] to:");
            formatter.WriteObject(cheapest);

            if (cheapest.ID != null)
                cheapest.StatusProduktu = true;

            formatter.Flush();
        }

        private void RemoveEmptyWarehouse(IList<Product> products, IObjectFormatter<object> formatter)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].StanMagazynowy <= 0)
                {
                    formatter.WriteLine("Stan magazynowy produktu jest pusty, usuwam go z listy: ");
                    formatter.WriteObject(products[i]);

                    products.RemoveAt(i);
                    i--;
                }
            }
        }

        private Product FindCheapestProduct(IList<Product> products, bool includeAll)
        {
            var cheapest = products.First(product => ProductFilter(product, includeAll));
            for (int i = 1; i < products.Count; i++)
            {
                if (!ProductFilter(products[i], includeAll))
                    continue;

                var result = netPriceComparer.Compare(products[i], cheapest);

                if (result == -1)
                {
                    cheapest = products[i];
                }
            }

            return cheapest;
        }

        private static bool ProductFilter(Product p, bool includeNotAvailable)
        {
            if (includeNotAvailable)
                return true;

            return p.StanMagazynowy > 0;
        }

        public void ExecuteAction(string partNumber, ICollection<Product> vendorProducts, ICollection<Product> metProducts,
            IObjectFormatter<object> objectFormatter)
        {
            throw new NotImplementedException();
        }
    }
}
