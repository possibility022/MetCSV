using System;
using System.Diagnostics;
using METCSV.Common;
using METCSV.Common.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparers
{
    [TestClass]
    public class ProductBySapManuHashTest
    {
        static ProductBySapManuHash _comparer;
        static Product _productABC;
        static Product _productXYZ;

        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            _comparer = new ProductBySapManuHash();
            _productABC = new Product() { NazwaProducenta = "ABC" };
            _productXYZ = new Product() { NazwaProducenta = "XYZ" };
            Trace.WriteLine(_productABC.SapManuHash);
        }

        [TestMethod]
        public void ReturnOneIfProductAIsGreater()
        {
            var result = _comparer.Compare(_productXYZ, _productABC);

            if (_productXYZ.SapManuHash > _productABC.SapManuHash)
                Assert.AreEqual(1, result);
            else
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnMinusOneIfProductAIsNotGreater()
        {
            var result = _comparer.Compare(_productABC, _productXYZ);

            if (_productXYZ.SapManuHash > _productABC.SapManuHash)
                Assert.AreEqual(-1, result);
            else
                Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ReturnZeroIfProductsAreEqual()
        {
            var result = _comparer.Compare(_productABC, _productABC);

            Assert.AreEqual(0, result);
        }
    }
}
