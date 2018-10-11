using MET.Domain.Logic.Comparers;
using METCSV.Common;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class CompareDomain
    {
        ConcurrentBag<int> _allPartNumbers;
        ConcurrentDictionary<int, IList<Product>> _products;

        ProductByProductPrice _netPriceComparer = new ProductByProductPrice();

        IObjectFormatter<Product> _objectFormatter;

        public CompareDomain(IDictionary<int, byte> allPartNumbers, IObjectFormatter<Product> objectFormatter = null)
        {
            _allPartNumbers = new ConcurrentBag<int>(allPartNumbers.Keys);
            _objectFormatter = objectFormatter ?? new BasicJsonFormatter<Product>();
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
            int partNumber;
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

        private void SelectOneProduct(IList<Product> products, int partNumber)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            var sb = new StringBuilder();

            sb.AppendLine($"Zaczynam porównywać listę produktów dla PartNumberu [{partNumber}]: ");
            _objectFormatter.Get(sb, products);

            RemoveEmptyWarehouse(products, sb);

            if (products.Count == 0)
            {
                sb.AppendLine($"List dla {partNumber} jest pusta. Wszystkie produkty są niedostępne?");
                return;
            }

            sb.AppendLine("Wybieram najtańszy produkt z listy:");
            _objectFormatter.Get(sb, products);

            var cheapest = FindCheapestProduct(products);

            sb.AppendLine($"Najtańszy produkt dla PartNumberu [{partNumber}] to:");
            _objectFormatter.Get(sb, cheapest);

            if (cheapest.ID != null)
                cheapest.StatusProduktu = true;

            Log.LogProductInfo(sb.ToString());
        }

        private void RemoveEmptyWarehouse(IList<Product> products, StringBuilder sb)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].StanMagazynowy <= 0)
                {
                    sb.AppendLine("Stan magazynowy produktu jest pusty, usuwam go z listy: ");
                    _objectFormatter.Get(sb, products[i]);

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
