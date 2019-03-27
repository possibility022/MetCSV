using MET.Domain.Logic.Comparers;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public void Compare(IEnumerable<Product> ab, IEnumerable<Product> td, IEnumerable<Product> lama)
        {
            _products = new ConcurrentDictionary<string, IList<Product>>();

            Task[] tasks = new Task[3];
            tasks[0] = new Task(() => AddToCollection(ab));
            tasks[1] = new Task(() => AddToCollection(td));
            tasks[2] = new Task(() => AddToCollection(lama));

            tasks.StartAll();
            tasks.WaitAll();

            tasks = new Task[Environment.ProcessorCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(Compare);
            }

            tasks.StartAll();
            tasks.WaitAll();
        }

        private void AddToCollection(IEnumerable<Product> productsToAdd)
        {
            foreach (var p in productsToAdd)
            {
                _products.AddOrUpdate(
                    p.PartNumber,
                    new List<Product>() { p },
                    (key, oldValue) => { oldValue.Add(p); return oldValue; });
            }
        }

        private void Compare()
        {
            string partNumber;
            var partNumberTaken = false;

            while ((partNumberTaken = _allPartNumbers.TryTake(out partNumber)) || _allPartNumbers.Count > 0)
            {
                if (partNumberTaken)
                {
                    IList<Product> listToCompare = null;

                    var listTaken = _products.TryGetValue(partNumber, out listToCompare);

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

            RemoveEmptyWarehouse(products, formatter);

            if (products.Count == 0)
            {
                formatter.WriteLine($"List dla {partNumber} jest pusta. Wszystkie produkty są niedostępne?");
                return;
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
            var cheapest = products[0];
            for (int i = 1; i < products.Count; i++)
            {
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
