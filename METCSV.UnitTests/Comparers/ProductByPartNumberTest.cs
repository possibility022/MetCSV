using System.Diagnostics;
using METCSV.Common;
using METCSV.Common.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparers
{
    [TestClass]
    public class ProductByPartNumberTest
    {

        static ProductByPartNumber _comparer;
        static Product _productABC;
        static Product _productXYZ;

        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            _comparer = new ProductByPartNumber();
            _productABC = new Product(Providers.AB) { NazwaProducenta = "ABC" };
            _productXYZ = new Product(Providers.AB) { NazwaProducenta = "XYZ" };
            Trace.WriteLine(_productABC.PartNumber);
        }

        [TestMethod]
        public void ReturnOneIfProductAIsGreater()
        {
            var result = _comparer.Compare(_productXYZ, _productABC);

            if (_productXYZ.PartNumber > _productABC.PartNumber)
                Assert.AreEqual(1, result);
            else
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnMinusOneIfProductAIsNotGreater()
        {
            var result = _comparer.Compare(_productABC, _productXYZ);

            if (_productXYZ.PartNumber > _productABC.PartNumber)
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
