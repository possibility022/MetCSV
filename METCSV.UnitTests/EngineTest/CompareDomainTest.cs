using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.GroupsActionExecutors;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class CompareDomainTest
    {

        IList<Product> _shortProviderList;
        AllPartNumbersDomain allPartNumbersDomain;

        private PriceDomain priceDomain;

        static ZeroOutputFormatter Formatter;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Formatter = new ZeroOutputFormatter();
        }

        [TestInitialize]
        public void InitializeData()
        {
            priceDomain = new PriceDomain();
            _shortProviderList = new List<Product>()
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
            priceDomain.ExecuteAction("", _shortProviderList, new List<Product>(), Formatter);

            //Assert
            Assert.IsFalse(_shortProviderList[1].StatusProduktu);
            Assert.IsFalse(_shortProviderList[2].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct()
        {
            // Act
            priceDomain.ExecuteAction("", _shortProviderList, new List<Product>(), Formatter);

            foreach (var product in _shortProviderList)
            {
                Console.WriteLine(product.CenaZakupuNetto);
            }

            // Assert
            Assert.IsTrue(_shortProviderList[3].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectOnlyOneProduct()
        {
            // Arrange
            var bestProduct = _shortProviderList[3];

            // Act
            priceDomain.ExecuteAction("", _shortProviderList, new List<Product>(), Formatter);

            // Assert
            foreach (var product in _shortProviderList)
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
            priceDomain.ExecuteAction("", _shortProviderList, new List<Product>(), Formatter);

            var list = shortProviderList.Single().Value;

            // Assert
            Assert.IsTrue(list.All(p => p.StatusProduktu == false));
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowErrorIfListIsEmpty()
        {
            // Act
            priceDomain.ExecuteAction("", new List<Product>(), new List<Product>(), Formatter);

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses()
        {
            // Arrange
            foreach (var product in _shortProviderList)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction("", _shortProviderList, new List<Product>(), Formatter);

            // Assert
            // Ok if no exceptions
        }
    }
}
