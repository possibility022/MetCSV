using MET.Domain;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductNameDomainTests
    {
        ProductNameDomain executor;

        Product[] list;
        private ProductGroup productGroup;

        [TestInitialize]
        public void TestInit()
        {
            executor = new ProductNameDomain();

            productGroup = new ProductGroup(string.Empty, new ZeroOutputFormatter());
            productGroup.AddVendorProduct(new Product(Providers.Lama) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" });
            productGroup.AddVendorProduct(new Product(Providers.AB) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3" });
        }

        [TestMethod]
        public void PrioritiesTest_LamaIs1st()
        {
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa1"));
        }

        [TestMethod]
        public void PrioritiesTest_TechDataIs2nd()
        {
            list = list.Where(r => r.Provider != Providers.Lama).ToArray();

            executor.ExecuteAction(productGroup);

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa3"));
        }


        [TestMethod]
        public void PrioritiesTest_TechDataIs3rd()
        {
            list = list.Where(r => r.Provider != Providers.Lama && r.Provider != Providers.TechData).ToArray();

            executor.ExecuteAction(productGroup);

            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa2"));
        }

        [TestMethod]
        public void PrioritiesTest_MetTopHasPriority()
        {
            productGroup.AddMetProduct(new Product(Providers.MET) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa4" });
            Assert.IsTrue(list.All(r => r.NazwaProduktu == "Nazwa4"));
        }
    }
}
