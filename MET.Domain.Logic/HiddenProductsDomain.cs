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
    public class HiddenProductsDomain
    {

        ConcurrentDictionary<string, Product> _hidden;

        IObjectFormatter<Product> _objectFormatter;

        public HiddenProductsDomain(IObjectFormatter<Product> objectFormatter = null)
        {
            _objectFormatter = objectFormatter ?? new BasicJsonFormatter<Product>();
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
            var sb = new StringBuilder();

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
                    sb.AppendLine($"Ustawiam flage HIDDEN produktu SymbolSap:[{outProduct.SymbolSAP}] na True");
                    outProduct.Hidden = true;
                    sb.AppendLine("Produkt po zmianie: ");
                    _objectFormatter.Get(sb, outProduct);
                }

                outProduct = null;
            }

            Log.LogProductInfo(sb.ToString());
        }

        public ConcurrentDictionary<string, Product> CreateListOfHiddenProducts(IEnumerable<Product> products)
        {
            ConcurrentDictionary<string, Product> hidden = new ConcurrentDictionary<string, Product>();

            var sb = new StringBuilder("Ukryte produkty, lista Symboli SAP:");

            foreach (var p in products)
            {
                if (p.Hidden)
                {
                    hidden.TryAdd(p.SymbolSAP, p);
                    sb.AppendLine(p.SymbolSAP);
                }
            }
            
            Log.LogProductInfo(sb.ToString());

            _hidden = hidden;
            return hidden;
        }
    }
}
