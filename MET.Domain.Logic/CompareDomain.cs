using MET.Domain.Logic.Comparers;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class CompareDomain
    {
        ConcurrentBag<string> _allPartNumbers;
        ConcurrentDictionary<string, IList<Product>> _products;

        ProductByProductPrice _netPriceComparer = new ProductByProductPrice();

        IObjectFormatterConstructor<object> _objectFormatter;

        public CompareDomain(IDictionary<string, byte> allPartNumbers, IObjectFormatterConstructor<object> objectFormatter)
        {
            _allPartNumbers = new ConcurrentBag<string>(allPartNumbers.Keys);
            _objectFormatter = objectFormatter;
        }

        public void Compare(ConcurrentDictionary<string, IList<Product>> combinedCollection)
        {
            _products = combinedCollection;

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

            while ((partNumberTaken = _allPartNumbers.TryTake(out partNumber)) || _allPartNumbers.Count > 0)
            {
                if (partNumberTaken)
                {
                    var listTaken = _products.TryGetValue(partNumber, out var listToCompare);

                    if (listTaken)
                    {
                        SelectOneProduct(listToCompare, partNumber);
                    }
                    else if (_products.ContainsKey(partNumber))
                    {
                        _allPartNumbers.Add(partNumber);
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

            var formatter = _objectFormatter.GetNewInstance();

            formatter.WriteLine($"Zaczynam porównywać listę produktów dla PartNumberu [{partNumber}]: ");
            formatter.WriteObject(products);


            // Todo move it to new domain
            //RemoveEmptyWarehouse(products, formatter);

            if (products.All(r => r.StanMagazynowy <= 0))
            {
                formatter.WriteLine($"Wszystkie produkty dla {partNumber} są niedostępne");
                return;
            }

            if (products.Count == 0)
            {
                throw new Exception($"Something went wrong. List of products for {partNumber} is empty.");
            }

            formatter.WriteLine("Wybieram najtańszy produkt z listy:");
            formatter.WriteObject(products);

            var cheapest = FindCheapestProduct(products);

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

        private Product FindCheapestProduct(IList<Product> products)
        {
            var cheapest = products.First(r => r.StanMagazynowy > 0);
            for (int i = 1; i < products.Count; i++)
            {
                if (products[i].StanMagazynowy <= 0)
                    continue;

                var result = _netPriceComparer.Compare(products[i], cheapest);

                if (result == -1)
                {
                    cheapest = products[i];
                }
            }

            return cheapest;
        }

    }
}
