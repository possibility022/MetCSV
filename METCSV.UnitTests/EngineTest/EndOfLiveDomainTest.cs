using System.Collections.Generic;
using System.Linq;
using MET.Domain;
using MET.Domain.Logic;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class EndOfLiveDomainTest
    {

        Product[] _shortMetList;
        Product[] _shortProviderList;

        static ZeroOutputFormatter formatter;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            formatter = new ZeroOutputFormatter();
        }

        [TestInitialize]
        public void InitializeData()
        {
            //Arrange
            _shortMetList = new[] {
                new Product(Providers.None) {SymbolSAP = "ABC1", NazwaProduktu  = "Produkt1", NazwaProducenta = "Producent1", OryginalnyKodProducenta = "A"},
                new Product(Providers.None) {SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "B"},
                new Product(Providers.None) {SymbolSAP = "ABC3", NazwaProduktu  = "Produkt3",  NazwaProducenta = "Producent3", OryginalnyKodProducenta = "C"}
            };

            _shortProviderList = new[] {
                new Product(Providers.None) { SymbolSAP = "ABC", NazwaProduktu  = "Produkt",  NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new Product(Providers.None) { SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
            };
        }

        [TestMethod]
        public void DomainWillNotRemoveProductsFromCurrentCollection()
        {
            // Arrange
            var met = Factory.GetMetProducts();
            var provider = Factory.GetABProducts();
            var metCount = met.Count;

            // Act
            EndOfLiveDomain domain = new EndOfLiveDomain(met, formatter, provider);

            // Assert
            Assert.AreEqual(metCount, met.Count);
        }

        [TestMethod]
        public void IfProvideListContainsSapMenuHashDoNotSetEOL()
        {
            // Arrange
            var productWithOutEol = _shortMetList[1];
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            // Act
            domain.SetEndOfLife();

            // Assert
            Assert.AreNotEqual("EOL", productWithOutEol.Kategoria);
        }

        [TestMethod]
        public void IfProvideListContainsManufacturerCodeDoNotSetEOL()
        {
            // Arrange
            var productWithOutEol = _shortMetList[0];
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            // Act
            domain.SetEndOfLife();

            // Assert
            Assert.AreNotEqual("EOL", productWithOutEol.Kategoria);
        }

        [TestMethod]
        public void IfThereIsNoSapManuHashOrManufacturerCodeToMatchSetEOL()
        {
            // Arrange
            var productWithEol = _shortMetList[2];
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            // Act
            domain.SetEndOfLife();

            // Assert
            Assert.AreEqual("EOL", productWithEol.Kategoria);
        }

        [TestMethod]
        public void SetEol_ToAllMetWhenNoVendors()
        {
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            domain.ExecuteAction(string.Empty, new List<Product>(), _shortMetList, formatter);

            foreach (var product in _shortMetList)
            {
                Assert.AreEqual("EOL", product.Kategoria);
            }
        }

        [TestMethod]
        public void DoNot_SetEol_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            domain.ExecuteAction(string.Empty, new List<Product>() { new Product(Providers.None) }, _shortMetList, formatter);

            foreach (var product in _shortMetList)
            {
                Assert.AreNotEqual("EOL", product.Kategoria);
            }
        }

        [TestMethod]
        public void AddNamePrefix_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            domain.ExecuteAction(string.Empty, new List<Product>(), _shortMetList, formatter);

            foreach (var product in _shortMetList)
            {
                Assert.IsTrue(product.NazwaProduktu.StartsWith(EndOfLiveDomain.EndOfLifeProductNamePrefix));
            }
        }

        [TestMethod]
        public void DoNot_AddNamePrefix_ToMetProductsWhenThereIsAtLeastOne()
        {
            var domain = new EndOfLiveDomain(_shortMetList, formatter, _shortProviderList);

            domain.ExecuteAction(string.Empty, new List<Product>() { new Product(Providers.None) }, _shortMetList, formatter);

            foreach (var product in _shortMetList)
            {
                Assert.IsFalse(product.NazwaProduktu.Contains(EndOfLiveDomain.EndOfLifeProductNamePrefix));
            }
        }

    }
}
