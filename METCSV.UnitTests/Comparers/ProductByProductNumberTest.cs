﻿using System;
using METCSV.Common;
using METCSV.Common.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparers
{
    [TestClass]
    public class ProductByProductNumberTest
    {

        static ProductByProductNumber _comparer;
        static Product _cheap;
        static Product _expensive;


        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            _comparer = new ProductByProductNumber();
            _cheap = new Product() { CenaZakupuNetto = 100 };
            _expensive = new Product() { CenaZakupuNetto = 999 };
        }

        [TestMethod]
        public void ReturnOneIfProductAIsGreater()
        {
            var result = _comparer.Compare(_expensive, _cheap);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ReturnMinusOneIfProductAIsNotGreater()
        {
            var result = _comparer.Compare(_cheap, _expensive);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnZeroIfProductsAreEqual()
        {
            var result = _comparer.Compare(_cheap, _cheap);

            Assert.AreEqual(0, result);
        }
    }
}
