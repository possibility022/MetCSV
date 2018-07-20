using MET.Domain.Logic.Comparers;
using METCSV.Common.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class CompareDomain
    {
        ConcurrentBag<int> _allPartNumbers;
        ConcurrentDictionary<int, IList<Product>> _products;

        ProductByProductPrice _netPriceComparer = new ProductByProductPrice();

        public CompareDomain(IDictionary<int, byte> allPartNumbers)
        {
            _allPartNumbers = new ConcurrentBag<int>(allPartNumbers.Keys);
        }

        public void Compare(IEnumerable<Product> ab, IEnumerable<Product> td, IEnumerable<Product> lama)
        {
            _products = new ConcurrentDictionary<int, IList<Product>>();

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
            int partNumber = int.MinValue;

            var partNumberTaken = false;

            while ((partNumberTaken = _allPartNumbers.TryTake(out partNumber)) || _allPartNumbers.Count > 0)
            {
                if (partNumberTaken)
                {
                    IList<Product> listToCompare = null;

                    var listTaken = _products.TryGetValue(partNumber, out listToCompare);

                    if (listTaken)
                    {
                        SelectOneProduct(listToCompare);
                    }
                }
            }
        }

        private void SelectOneProduct(IList<Product> products)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            RemoveEmptyWarehouse(products);

            if (products.Count == 0)
                return;

            var cheapest = FindCheapestProduct(products);

            if (cheapest.ID != null)
                cheapest.StatusProduktu = true;
        }

        private void RemoveEmptyWarehouse(IList<Product> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].StanMagazynowy <= 0)
                {
                    products.RemoveAt(i);
                    i--;
                }
            }
        }

        private Product FindCheapestProduct(IList<Product> products)
        {
            Product cheapest = products[0];
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
