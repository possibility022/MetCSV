using MET.Data.Models;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;
using METCSV.Tests.EngineTest.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class EndOfLiveDomainTest
    {

        Product[] shortMetList;
        Product[] shortProviderList;

        static ZeroOutputFormatter formatter = ZeroOutputFormatter.Instance;
        private ProductGroup productGroup;

        [TestInitialize]
        public void InitializeData()
        {
            //Arrange
            shortMetList = new[] {
                new Product(Providers.Met) {SymbolSap = "ABC1", NazwaProduktu  = "Produkt1", NazwaProducenta = "Producent1", OryginalnyKodProducenta = "A"},
                new Product(Providers.Met) {SymbolSap = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "B"},
                new Product(Providers.Met) {SymbolSap = "ABC3", NazwaProduktu  = "Produkt3",  NazwaProducenta = "Producent3", OryginalnyKodProducenta = "C"}
            };

            shortProviderList = new[] {
                new Product(Providers.None) { SymbolSap = "ABC", NazwaProduktu  = "Produkt",  NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new Product(Providers.None) { SymbolSap = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
            };

            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddVendorProducts(shortProviderList);
            productGroup.AddMetProducts(shortMetList);
        }

        [TestMethod]
        public void SetEol_ToFinalProduct()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);

            domain.ExecuteAction(productGroup);

            Assert.AreEqual("EOL", productGroup.FinalProduct.Kategoria);
        }

        [TestMethod]
        public void DoNot_SetEol_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);
            productGroup.AddVendorProduct(new Product(Providers.Ab));

            // act
            domain.ExecuteAction(productGroup);

            Assert.AreNotEqual("EOL", productGroup.FinalProduct.Kategoria);
        }

        [TestMethod]
        public void AddNamePrefix_ToAllMetWhenNoVendors()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);

            // act
            domain.ExecuteAction(productGroup);

            Assert.IsTrue(productGroup.FinalProduct.NazwaProduktu.StartsWith(EndOfLiveDomain.EndOfLifeProductNamePrefix));
        }

        [TestMethod]
        public void DoNot_AddNamePrefix_WhenPrefixIsSet()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);

            // act
            domain.ExecuteAction(productGroup);
            domain.ExecuteAction(productGroup);

            Assert.IsFalse(productGroup.FinalProduct.NazwaProduktu.StartsWith(EndOfLiveDomain.EndOfLifeProductNamePrefix + EndOfLiveDomain.EndOfLifeProductNamePrefix));
        }

        [TestMethod]
        public void DoNot_AddNamePrefix_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain();

            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);
            productGroup.AddVendorProduct(new Product(Providers.Ab));

            // act
            domain.ExecuteAction(productGroup);

            Assert.IsFalse(productGroup.FinalProduct.NazwaProduktu.Contains(EndOfLiveDomain.EndOfLifeProductNamePrefix));
        }

        [TestMethod]
        public void SetWarehouseToZero_IfProductIsSetToEol()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, formatter);
            productGroup.AddMetProducts(shortMetList);

            domain.ExecuteAction(productGroup);

            Assert.AreEqual(0, productGroup.FinalProduct.StanMagazynowy);
        }

    }
}
