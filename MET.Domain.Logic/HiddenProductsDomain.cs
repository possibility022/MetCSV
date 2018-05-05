using METCSV.Common;
using METCSV.Common.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace METCSV.Domain.Logic
{
    public class HiddenProductsDomain
    {

        ConcurrentDictionary<string, Product> _hidden;

        public HiddenProductsDomain()
        {
            
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
                    outProduct.Hidden = true;
                }

                outProduct = null;
            }
        }

        public ConcurrentDictionary<string, Product> CreateListOfHiddenProducts(IEnumerable<Product> products)
        {
            ConcurrentDictionary<string, Product> hidden = new ConcurrentDictionary<string, Product>();

            foreach (var p in products)
            {
                if (p.Hidden)
                {
                    hidden.TryAdd(p.SymbolSAP, p);
                }
            }

            _hidden = hidden;
            return hidden;
        }
    }
}
