using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic
{
    public class EndOfLiveDomain : IActionExecutor
    {
        public const string EndOfLifeCategory = "EOL";
        public const string EndOfLifeProductNamePrefix = "EOL - wycofany z oferty - ";

        readonly ConcurrentBag<Product> metProducts;
        readonly ConcurrentDictionary<string, Product> sapManuHash;
        readonly ConcurrentDictionary<string, Product> manufacturerCode;

        private Task[] tasks;

        readonly IObjectFormatterConstructor<object> formatterFactory;

        public EndOfLiveDomain(IEnumerable<Product> metProducts, IObjectFormatterConstructor<object> formatterFactory, params IEnumerable<Product>[] providers)
        {
            this.metProducts = new ConcurrentBag<Product>(metProducts);
            sapManuHash = new ConcurrentDictionary<string, Product>();
            manufacturerCode = new ConcurrentDictionary<string, Product>();

            this.formatterFactory = formatterFactory;

            foreach (var provider in providers)
            {
                FillList(provider);
            }
        }

        private void FillList(IEnumerable<Product> products)
        {
            foreach (var p in products)
            {
                sapManuHash.TryAdd(p.SapManuHash, p);
                manufacturerCode.TryAdd(p.KodProducenta, p);
            }
        }

        public void SetEndOfLife()
        {
            if (tasks != null)
                throw new InvalidOperationException("It looks like another thread has already started. Tasks are not empty.");

            tasks = new Task[Environment.ProcessorCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => SetEndOfLife_Logic());
            }

            tasks.StartAll();
            tasks.WaitAll();

            tasks = null;
        }

        private void SetEndOfLife_Logic()
        {
            var formatter = this.formatterFactory.GetNewInstance();

            while (metProducts.TryTake(out Product p) || metProducts.Count > 0)
            {
                if (p != null)
                {
                    if (
                        manufacturerCode.TryGetValue(p.KodProducenta, out Product _) == false
                        && sapManuHash.TryGetValue(p.SapManuHash, out Product _) == false)
                    {
                        if (manufacturerCode.ContainsKey(p.KodProducenta) == false
                            && sapManuHash.ContainsKey(p.SapManuHash) == false)
                        {
                            formatter.WriteLine($"Ustawiam: KodProducenta: [{p.KodProducenta}] SAP: [{p.SymbolSAP}] na EOL, nie można go znaleźć w finalnej liście.");
                            SetEndOfLife(p);
                        }
                    }
                }
            }
        }

        public static void SetEndOfLife(Product p)
        {
            p.Kategoria = EndOfLifeCategory;
            p.StatusProduktu = false;
            p.SetCennaNetto(0);
            p.CenaZakupuNetto = 0;
        }

        private static void AddPrefixToProductName(Product p)
        {
            if (p.NazwaProduktu != null)
                p.NazwaProduktu = $"{EndOfLifeProductNamePrefix}{p.NazwaProduktu}";
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            var formatter = productGroup.ObjectFormatter;
            formatter.WriteLine("Sprawdzam czy ustawić status EOL.");

            if (!productGroup.VendorProducts.Any())
            {
                if (metProducts.Any())
                {
                    foreach (var product in metProducts)
                    {
                        formatter.WriteLine($"Brakuje produktów u dostawcow. Ustawiam EOL dla {product}: ");
                        formatter.WriteObject(product);
                        SetEndOfLife(product);
                        AddPrefixToProductName(product);
                    }
                }
            }
        }
    }
}
