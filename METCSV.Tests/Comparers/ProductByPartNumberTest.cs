using System.Diagnostics;
using MET.Data.Models;
using MET.Domain;
using MET.Domain.Logic.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparers
{
    [TestClass]
    public class ProductByPartNumberTest
    {

        static ProductByPartNumber comparer;
        static Product productAbc;
        static Product productXyz;

        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            comparer = new ProductByPartNumber();
            productAbc = new Product(Providers.Ab) { NazwaProducenta = "ABC" };
            productXyz = new Product(Providers.Ab) { NazwaProducenta = "XYZ" };
            Trace.WriteLine(productAbc.PartNumber);
        }

        [TestMethod]
        public void ReturnOneIfProductAIsGreater()
        {
            var result = comparer.Compare(productXyz, productAbc);

            if (string.Compare(productXyz.PartNumber, productAbc.PartNumber) > 0)
                Assert.AreEqual(1, result);
            else
                Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnMinusOneIfProductAIsNotGreater()
        {
            var result = comparer.Compare(productAbc, productXyz);

            if (string.Compare(productXyz.PartNumber, productAbc.PartNumber) > 0)
                Assert.AreEqual(-1, result);
            else
                Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ReturnZeroIfProductsAreEqual()
        {
            var result = comparer.Compare(productAbc, productAbc);

            Assert.AreEqual(0, result);
        }
    }
}
