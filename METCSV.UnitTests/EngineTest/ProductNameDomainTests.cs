using MET.Domain;
using MET.Domain.Logic;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductNameDomainTests
    {
        ProductNameDomain executor;

        Product[] list;

        [TestInitialize]
        public void TestInit()
        {
            executor = new ProductNameDomain();
            list = new[] {
                new Product (Providers.Lama){ UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" },
                new Product (Providers.AB){ UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2"   },
                new Product (Providers.TechData){ UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3" } };
        }

        [TestMethod]
        public void PrioritiesTest_LamaIs1st()
        {
            executor.ExecuteAction("XYZ",
                list,
            new Product[] { },
            new ZeroOutputFormatter());

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa1"));
        }

        [TestMethod]
        public void PrioritiesTest_TechDataIs2nd()
        {
            list = list.Where(r => r.Provider != Providers.Lama).ToArray();

            executor.ExecuteAction("XYZ",
                list,
                new Product[] { },
                new ZeroOutputFormatter());

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa3"));
        }


        [TestMethod]
        public void PrioritiesTest_TechDataIs3rd()
        {
            list = list.Where(r => r.Provider != Providers.Lama && r.Provider != Providers.TechData).ToArray();

            executor.ExecuteAction("XYZ",
                list,
                new Product[] { },
                new ZeroOutputFormatter());

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa2"));
        }

        [TestMethod]
        public void PrioritiesTest_MetTopHasPriority()
        {
            executor.ExecuteAction("XYZ",
                list,
                new Product[]
                {
                    new Product (Providers.MET){ UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa4" }
                },
                new ZeroOutputFormatter());

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa4"));
        }
    }
}
