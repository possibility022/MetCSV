﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MET.CSV.Generator;
using MET.Data.Models;
using MET.Data.Storage;
using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;
using METCSV.WPF.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductMergerTest
    {
        static ProgramFlow _productMerger;

        List<Product> _workOnList;

        static string sapManuHashOfProductsWhichHasPriceError;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var met = Factory.GetMetProducts();
            var lama = Factory.GetLamaProducts();
            var td = Factory.GetTDProducts();
            var ab = Factory.GetABProducts();

            var ab_old = Factory.GetABProducts();
            var someProduct = ab_old.Last(r => r.StanMagazynowy > 0);
            someProduct.CenaZakupuNetto = someProduct.CenaZakupuNetto + (30 * someProduct.CenaZakupuNetto / 100) - 1;
            sapManuHashOfProductsWhichHasPriceError = someProduct.SapManuHash;

            var products = new Products()
            {
                MetProducts = met,
                LamaProducts = lama,
                TechDataProducts = td,
                AbProducts = ab,
                AbProducts_Old = ab_old
            };

            _productMerger = new ProgramFlow(
                new StorageService(new StorageContext()),
                new Settings(),
                true,
                20,
                CancellationToken.None,
                ZeroOutputFormatter.Instance
            );

            var t = _productMerger.FirstStep();
            t.Wait();
        }

        [TestInitialize]
        public void Initialize()
        {
            _workOnList = new List<Product>(_productMerger.GetFinalList(true));
        }

        [TestMethod]
        public void ValidateThatProductWithPriceErrorIsNotInWarehouse()
        {
            var product = _productMerger
                .GetFinalList(true)
                .Single(r => r.SapManuHash == sapManuHashOfProductsWhichHasPriceError && r.Provider == Providers.AB);

            Assert.AreEqual(0, product.StanMagazynowy);
        }

        [TestMethod]
        public void AllSelectedProductsWillHaveWarehousGreaterThanOne()
        {
            // Assert
            var result = ValidateList(_workOnList, ValidateGroupForEmptyWarehouse);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SelectedProductMustBeCheapest()
        {
            // Assert
            var result = ValidateList(_workOnList, ValidateGroupForCheapesProduct);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OnlyOneProductCanBeSelected()
        {
            // Assert
            var result = ValidateList(_workOnList, ValidateGroupForOnlyOneSelectedGroup);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProductsWithEOLCanNotBeVisible()
        {
            // Assert
            var result = ValidateList(_workOnList, ValidateGroupForEOLVisibility);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TechDataProductsWillHaveStatus0()
        {
            // Assert
            var result = ValidateList(_workOnList, ValidateForTechDataEOL);
            Assert.IsTrue(result);
        }

        private bool ValidateForTechDataEOL(IEnumerable<Product> products)
        {
            foreach (var p in products)
            {
                if (p.Provider == Providers.TechData && p.OryginalnyKodProducenta.Contains("?TN"))
                {
                    if (p.Kategoria != "EOL_TN" || p.StatusProduktu || p.CenaNetto > 0 || p.CenaZakupuNetto > 0)
                        return false;
                }
            }

            return true;
        }

        private bool ValidateGroupForEOLVisibility(IList<Product> group)
        {
            return group.Where(p => p.Kategoria == "EOL").All(p2 => !p2.StatusProduktu);
        }

        private bool ValidateGroupForEmptyWarehouse(IList<Product> group)
        {
            foreach (var p in group)
            {
                if (p.StatusProduktu && p.StanMagazynowy < 1)
                    return false;
            }

            return true;
        }

        private bool ValidateGroupForCheapesProduct(IList<Product> group)
        {
            var selected = group.FirstOrDefault(p => p.StatusProduktu);

            if (selected != null)
            {
                foreach (var p in group)
                {
                    if (!ReferenceEquals(p, selected))
                    {
                        if (selected.CenaNetto > p.CenaNetto)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool ValidateGroupForOnlyOneSelectedGroup(IList<Product> group)
        {
            return group.Count(p => p.StatusProduktu) <= 1;
        }

        private bool ValidateList(IEnumerable<Product> products, Func<IList<Product>, bool> validatingFunction)
        {
            var dict = CreateGrouppedDict(products);

            foreach (var list in dict)
            {
                if (!validatingFunction(list.Value))
                    return false;
            }

            return true;
        }

        private Dictionary<string, IList<Product>> CreateGrouppedDict(IEnumerable<Product> products)
        {
            var dict = new Dictionary<string, IList<Product>>();

            foreach (var p in products)
            {
                if (p.Provider == Providers.MET)
                    continue;

                if (dict.ContainsKey(p.PartNumber))
                {
                    dict[p.PartNumber].Add(p);
                }
                else
                {
                    dict.Add(p.PartNumber, new List<Product>() { p });
                }
            }

            return dict;
        }
    }
}
