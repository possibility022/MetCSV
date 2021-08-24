using METCSV.Common;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MET.Data.Models;

namespace MET.Domain.Logic
{
    public class HiddenProductsDomain
    {

        ConcurrentDictionary<string, Product> _hidden;

        IObjectFormatterConstructor<object> _objectFormatter;

        public HiddenProductsDomain(IObjectFormatterConstructor<object> objectFormatter)
        {
            _objectFormatter = objectFormatter;
        }

        public ConcurrentBag<Product> RemoveHiddenProducts(ConcurrentBag<Product> products)
        {
            Task[] tasks = new Task[Environment.ProcessorCount];

            ConcurrentBag<Product> finalList = new ConcurrentBag<Product>();

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => RemoveHiddenProducts_Logic(products, finalList));
            }

            tasks.StartAll();
            tasks.WaitAll();
            return finalList;
        }

        private void RemoveHiddenProducts_Logic(ConcurrentBag<Product> products, ConcurrentBag<Product> finalList)
        {
            Product outProduct = null;
            var formatter = _objectFormatter.GetNewInstance();

            if (_hidden == null)
            {
                throw new InvalidOperationException("You should create list of hidden products first.");
            }

            while (products.TryTake(out outProduct) || products.Count > 0)
            {
                if (outProduct == null)
                    continue;

                if (_hidden.ContainsKey(outProduct.SymbolSAP) == false)
                {
                    finalList.Add(outProduct);
                }
                else
                {
                    formatter.WriteLine($"Ustawiam flage HIDDEN produktu SymbolSap:[{outProduct.SymbolSAP}] na True");
                    outProduct.Hidden = true;
                    formatter.WriteLine("Produkt po zmianie: ");
                    formatter.WriteObject(outProduct);
                }

                outProduct = null;
            }

            formatter.Flush();
        }

        public ConcurrentDictionary<string, Product> CreateListOfHiddenProducts(IEnumerable<Product> products)
        {
            ConcurrentDictionary<string, Product> hidden = new ConcurrentDictionary<string, Product>();

            var formatter = _objectFormatter.GetNewInstance();

            formatter.WriteLine("Ukryte produkty: ");

            foreach (var p in products)
            {
                if (p.Hidden)
                {
                    hidden.TryAdd(p.SymbolSAP, p);
                    formatter.WriteLine(p.SymbolSAP);
                }
            }

            formatter.Flush();

            _hidden = hidden;
            return hidden;
        }
    }
}
