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
                new Product(Providers.None) {SymbolSAP = "ABC1", NazwaProducenta = "Producent1", OryginalnyKodProducenta = "A"},
                new Product(Providers.None) {SymbolSAP = "ABC2", NazwaProducenta = "Producent2", OryginalnyKodProducenta = "B"},
                new Product(Providers.None) {SymbolSAP = "ABC3", NazwaProducenta = "Producent3", OryginalnyKodProducenta = "C"}
            };
            
            _shortProviderList = new[] {
                new Product(Providers.None) { SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new Product(Providers.None) { SymbolSAP = "ABC2", NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
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

    }
}
