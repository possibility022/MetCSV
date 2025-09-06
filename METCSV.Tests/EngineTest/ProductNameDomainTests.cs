using MET.Data.Models;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class ProductNameDomainTests
    {
        ProductNameDomain executor;
        private ProductGroup productGroup;

        [TestInitialize]
        public void TestInit()
        {
            executor = new ProductNameDomain();
            productGroup = new ProductGroup(string.Empty, ZeroOutputFormatter.Instance);
        }

        [TestMethod]
        public void PrioritiesTest_LamaIs1st()
        {
            productGroup.AddVendorProduct(new Product(Providers.Lama) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 1, NazwaProduktu = "Nazwa1" });
            productGroup.AddVendorProduct(new Product(Providers.Ab) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 3, NazwaProduktu = "Nazwa3" });
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa1");
        }

        [TestMethod]
        public void PrioritiesTest_TechDataIs2nd()
        {
            productGroup.AddVendorProduct(new Product(Providers.Ab) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 3, NazwaProduktu = "Nazwa3" });
            
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa3");
        }

        [TestMethod]
        public void PrioritiesTest_AbIs3rd()
        {
            productGroup.AddVendorProduct(new Product(Providers.Ab) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 2, NazwaProduktu = "Nazwa2" });
            
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa2");
        }

        [TestMethod]
        public void PrioritiesTest_MetTopHasPriority()
        {
            productGroup.AddVendorProduct(new Product(Providers.Lama) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 1, NazwaProduktu = "Nazwa1" });
            productGroup.AddVendorProduct(new Product(Providers.Ab) { UrlZdjecia = string.Empty, SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 3, NazwaProduktu = "Nazwa3" });
            productGroup.AddMetProduct(new Product(Providers.Met) { UrlZdjecia = "SomeURL", SymbolSap = "ABC", NazwaProducenta = "Producent", Id = 3, NazwaProduktu = "Nazwa4" });

            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa4");
        }
    }
}
