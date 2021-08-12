using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.Extensions;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class EndOfLiveDomainTest
    {

        Product[] shortMetList;
        Product[] shortProviderList;

        static ZeroOutputFormatter _formatter = ZeroOutputFormatter.Instance;
        private ProductGroup productGroup;

        [TestInitialize]
        public void InitializeData()
        {
            //Arrange
            shortMetList = new[] {
                new Product(Providers.MET) {SymbolSAP = "ABC1", NazwaProduktu  = "Produkt1", NazwaProducenta = "Producent1", OryginalnyKodProducenta = "A"},
                new Product(Providers.MET) {SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "B"},
                new Product(Providers.MET) {SymbolSAP = "ABC3", NazwaProduktu  = "Produkt3",  NazwaProducenta = "Producent3", OryginalnyKodProducenta = "C"}
            };

            shortProviderList = new[] {
                new Product(Providers.None) { SymbolSAP = "ABC", NazwaProduktu  = "Produkt",  NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new Product(Providers.None) { SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
            };

            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddVendorProducts(shortProviderList);
            productGroup.AddMetProducts(shortMetList);
        }

        [TestMethod]
        public void SetEol_ToAllMetWhenNoVendors()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);

            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.AreEqual("EOL", product.Kategoria);
            }
        }

        [TestMethod]
        public void DoNot_SetEol_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);
            productGroup.AddVendorProduct(new Product(Providers.AB));

            // act
            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.AreNotEqual("EOL", product.Kategoria);
            }
        }

        [TestMethod]
        public void AddNamePrefix_ToAllMetWhenNoVendors()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);

            // act
            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.IsTrue(product.NazwaProduktu.StartsWith(EndOfLiveDomain.EndOfLifeProductNamePrefix));
            }
        }

        [TestMethod]
        public void DoNot_AddNamePrefix_WhenPrefixIsSet()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);

            // act
            domain.ExecuteAction(productGroup);
            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.IsFalse(product.NazwaProduktu.StartsWith(EndOfLiveDomain.EndOfLifeProductNamePrefix + EndOfLiveDomain.EndOfLifeProductNamePrefix));
            }
        }

        [TestMethod]
        public void DoNot_AddNamePrefix_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain();

            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);
            productGroup.AddVendorProduct(new Product(Providers.AB));

            // act
            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.IsFalse(product.NazwaProduktu.Contains(EndOfLiveDomain.EndOfLifeProductNamePrefix));
            }
        }

        [TestMethod]
        public void SetWarehouseToZero_IfProductIsSetToEol()
        {
            var domain = new EndOfLiveDomain();
            productGroup = new ProductGroup(string.Empty, _formatter);
            productGroup.AddMetProducts(shortMetList);

            domain.ExecuteAction(productGroup);

            foreach (var product in shortMetList)
            {
                Assert.AreEqual(0, product.StanMagazynowy);
            }
        }

    }
}
