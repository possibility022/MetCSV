using METCSV.Common.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class EndOfLiveDomain
    {
        public const string EndOfLifeCategory = "EOL";

        ConcurrentBag<Product> _metProducts;
        ConcurrentDictionary<int, Product> _sapManuHash;
        ConcurrentDictionary<string, Product> _manufacturerCode;

        public EndOfLiveDomain(IEnumerable<Product> metProducts, params IEnumerable<Product>[] providers)
        {
            _metProducts = new ConcurrentBag<Product>(metProducts);
            _sapManuHash = new ConcurrentDictionary<int, Product>();
            _manufacturerCode = new ConcurrentDictionary<string, Product>();

            foreach (var provider in providers)
            {
                FillList(provider);
            }
        }

        private void FillList(IEnumerable<Product> products)
        {
            foreach (var p in products)
            {
                _sapManuHash.TryAdd(p.SapManuHash, p);
                _manufacturerCode.TryAdd(p.KodProducenta, p);
            }
        }

        public void SetEndOfLife()
        {
            Task[] tasks = new Task[Environment.ProcessorCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => SetEndOfLife_Logic(_metProducts, _manufacturerCode, _sapManuHash));
            }

            tasks.StartAll();
            tasks.WaitAll();
        }

        private void SetEndOfLife_Logic(ConcurrentBag<Product> metProducts, ConcurrentDictionary<string, Product> manufacturersCode, ConcurrentDictionary<int, Product> sapManuHash)
        {

            while (metProducts.TryTake(out Product p) || metProducts.Count > 0)
            {
                if (p != null)
                {
                    if (
                        manufacturersCode.TryGetValue(p.KodProducenta, out Product _) == false
                        && sapManuHash.TryGetValue(p.SapManuHash, out Product _) == false)
                    {
                        if (manufacturersCode.ContainsKey(p.KodProducenta) == false
                            && sapManuHash.ContainsKey(p.SapManuHash) == false)
                        {
                            SetEndOfLife(ref p);
                        }
                    }
                }
            }
        }

        public static void SetEndOfLife(ref Product p)
        {
            p.Kategoria = EndOfLifeCategory;
            p.StatusProduktu = false;
            p.SetCennaNetto(0);
            p.CenaZakupuNetto = 0;
        }
    }
}
