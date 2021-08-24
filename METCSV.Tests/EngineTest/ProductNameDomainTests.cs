using MET.Data.Models;
using MET.Domain;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;

namespace METCSV.UnitTests.EngineTest
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
            productGroup.AddVendorProduct(new Product(Providers.Lama) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" });
            productGroup.AddVendorProduct(new Product(Providers.AB) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3" });
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa1");
        }

        [TestMethod]
        public void PrioritiesTest_TechDataIs2nd()
        {
            productGroup.AddVendorProduct(new Product(Providers.AB) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3" });
            
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa3");
        }

        [TestMethod]
        public void PrioritiesTest_AbIs3rd()
        {
            productGroup.AddVendorProduct(new Product(Providers.AB) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2" });
            
            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa2");
        }

        [TestMethod]
        public void PrioritiesTest_MetTopHasPriority()
        {
            productGroup.AddVendorProduct(new Product(Providers.Lama) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" });
            productGroup.AddVendorProduct(new Product(Providers.AB) { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2" });
            productGroup.AddVendorProduct(new Product(Providers.TechData) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3" });
            productGroup.AddMetProduct(new Product(Providers.MET) { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa4" });

            executor.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu == "Nazwa4");
        }
    }
}
