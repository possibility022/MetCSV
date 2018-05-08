﻿using System;
using System.Collections.Generic;
using System.Linq;
using METCSV.Common;
using METCSV.WPF.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductMergerTest
    {
        static ProductMerger _productMerger;

        List<Product> _workOnList;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _productMerger = new ProductMerger(
                Factory.GetMetProducts(),
                Factory.GetLamaProducts(),
                Factory.GetTDProducts(),
                Factory.GetABProducts());

            _productMerger.Generate();
        }

        [TestInitialize]
        public void Initialize()
        {
            _workOnList = new List<Product>(_productMerger.FinalList);
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
                foreach(var p in group)
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

        private Dictionary<int, IList<Product>> CreateGrouppedDict(IEnumerable<Product> products)
        {
            var dict = new Dictionary<int, IList<Product>>();

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