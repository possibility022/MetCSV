using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MET.Domain;
using MET.Domain.Logic.GroupsActionExecutors;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class CompareDomainTest
    {

        IList<Product> shortProviderList;

        private PriceDomain priceDomain;

        static ZeroOutputFormatter _formatter;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _formatter = new ZeroOutputFormatter();
        }

        [TestInitialize]
        public void InitializeData()
        {
            priceDomain = new PriceDomain();
            shortProviderList = new List<Product>()
            {
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 10},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 4},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 5},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 6}
            };
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void EmptyWarehousesCanNotBeSelected()
        {
            // Act
            priceDomain.ExecuteAction("", shortProviderList, new List<Product>(), _formatter);

            //Assert
            Assert.IsFalse(shortProviderList[1].StatusProduktu);
            Assert.IsFalse(shortProviderList[2].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct()
        {
            // Act
            priceDomain.ExecuteAction("", shortProviderList, new List<Product>(), _formatter);

            foreach (var product in shortProviderList)
            {
                Console.WriteLine(product.CenaZakupuNetto);
            }

            // Assert
            Assert.IsTrue(shortProviderList[3].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectOnlyOneProduct()
        {
            // Arrange
            var bestProduct = shortProviderList[3];

            // Act
            priceDomain.ExecuteAction("", shortProviderList, new List<Product>(), _formatter);

            // Assert
            foreach (var product in shortProviderList)
            {
                if (object.ReferenceEquals(product, bestProduct))
                {
                    Assert.IsTrue(product.StatusProduktu);
                    continue;
                }

                Assert.IsFalse(product.StatusProduktu);
            }

        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void IfIdIsNullThenIgnore()
        {
            // Arrange
            var shortProviderList = new ConcurrentDictionary<string, IList<Product>>();

            shortProviderList.TryAdd(string.Empty, new List<Product>()
            {
                new Product(Providers.AB) { ID = null, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 1 }
            });
            
            // Act
            priceDomain.ExecuteAction("", this.shortProviderList, new List<Product>(), _formatter);

            var list = shortProviderList.Single().Value;

            // Assert
            Assert.IsTrue(list.All(p => p.StatusProduktu == false));
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowErrorIfListIsEmpty()
        {
            // Act
            priceDomain.ExecuteAction("", new List<Product>(), new List<Product>(), _formatter);

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses()
        {
            // Arrange
            foreach (var product in shortProviderList)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction("", shortProviderList, new List<Product>(), _formatter);

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectTheCheapestWhenAllAreNotAvailable()
        {
            // Arrange
            foreach (var product in shortProviderList)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction("", shortProviderList, new List<Product>(), _formatter);

            // Assert
            var theCheapest = shortProviderList.Single(r => r.StatusProduktu);
            Assert.AreEqual(shortProviderList[1], theCheapest);

            // Only the cheapest should have status set to true
            Assert.IsTrue(shortProviderList
                .Where(r => !ReferenceEquals(theCheapest, r))
                .All(p => !p.StatusProduktu));
        }
    }
}
