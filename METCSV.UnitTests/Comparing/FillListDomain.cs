using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparing
{
    [TestClass]
    public class FillListDomain
    {

        ConcurrentBag<Product> met = new ConcurrentBag<Product>(
                new[] { new Product
                    {
                        UrlZdjecia = "some url",
                        SymbolSAP = "B"
                    },
                     new Product
                    {
                        UrlZdjecia = string.Empty,
                        SymbolSAP = "A"
                    },
                     new Product
                    {
                        UrlZdjecia = "some url",
                        SymbolSAP = "A"
                    },
        });

        ConcurrentBag<Product> lama = new ConcurrentBag<Product>(
            new[] { new Product
                {
                    UrlZdjecia = string.Empty,
                    SymbolSAP = "B"
                },
                 new Product
                {
                    UrlZdjecia = string.Empty,
                    SymbolSAP = "A"
                },
                 new Product
                {
                    UrlZdjecia = "URL",
                    SymbolSAP = "A"
                },
                    });

        [TestMethod]
        public void SelectCorrectProductAndSetEOLToOthers()
        {
            WPF.Engine.FillListDomain domain = new WPF.Engine.FillListDomain(ConvertToDictionary(met));
            domain.FillList(lama);

            foreach (var p in lama)
            {
                if (string.IsNullOrEmpty(p.UrlZdjecia))
                {
                    Assert.AreEqual("EOL", p.Kategoria);
                }
            }
        }

        private ConcurrentDictionary<string, IList<Product>> ConvertToDictionary(ConcurrentBag<Product> products)
        {
            Dictionary<string, IList<Product>> newDictionary = new Dictionary<string, IList<Product>>();

            Product p = null;

            while (products.TryTake(out p) || products.Count > 0)
            {
                if (newDictionary.ContainsKey(p.SymbolSAP))
                {
                    newDictionary[p.SymbolSAP].Add(p);
                }
                else
                {
                    newDictionary.Add(p.SymbolSAP, new List<Product> { p });
                }
            }

            return new ConcurrentDictionary<string, IList<Product>>(newDictionary);
        }
    }
}
