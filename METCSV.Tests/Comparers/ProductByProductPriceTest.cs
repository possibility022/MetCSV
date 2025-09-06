using MET.Data.Models;
using MET.Domain;
using MET.Domain.Logic.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparers
{
    [TestClass]
    public class ProductByProductPriceTest
    {

        static ProductByProductPrice comparer;
        static Product cheap;
        static Product expensive;


        [ClassInitialize]
        static public void Initialize(TestContext context)
        {
            comparer = new ProductByProductPrice();
            cheap = new Product(Providers.Ab) { CenaNetto = 100 };
            expensive = new Product(Providers.Ab) { CenaNetto= 999 };
        }

        [TestMethod]
        public void ReturnOneIfProductAIsGreater()
        {
            var result = comparer.Compare(expensive, cheap);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ReturnMinusOneIfProductAIsNotGreater()
        {
            var result = comparer.Compare(cheap, expensive);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnZeroIfProductsAreEqual()
        {
            var result = comparer.Compare(cheap, cheap);

            Assert.AreEqual(0, result);
        }
    }
}
