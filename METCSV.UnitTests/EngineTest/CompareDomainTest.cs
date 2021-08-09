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

        ConcurrentDictionary<string, IList<Product>> _shortProviderList;
        AllPartNumbersDomain allPartNumbersDomain;

        static ZeroOutputFormatter Formatter;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Formatter = new ZeroOutputFormatter();
        }

        [TestInitialize]
        public void InitializeData()
        {
            allPartNumbersDomain = new AllPartNumbersDomain();
            _shortProviderList = new ConcurrentDictionary<string, IList<Product>>();
            var success = _shortProviderList.TryAdd(string.Empty, new List<Product>()
            {
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 10},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 4},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 5},
                new Product(Providers.AB) {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 6}
            });

            if (success == false)
                throw new Exception("Something wrong with adding elements to list on test init.");

            allPartNumbersDomain.AddPartNumber("");
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void EmptyWarehousesCanNotBeSelected()
        {
            //Arrange
            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);

            // Act
            d.Compare(_shortProviderList);

            //Assert
            Assert.IsFalse(_shortProviderList[string.Empty][1].StatusProduktu);
            Assert.IsFalse(_shortProviderList[string.Empty][2].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct()
        {
            // Arrange
            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);

            // Act
            d.Compare(_shortProviderList);

            foreach (var product in _shortProviderList[string.Empty])
            {
                Console.WriteLine(product.CenaZakupuNetto);
            }

            // Assert
            Assert.IsTrue(_shortProviderList[string.Empty][3].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectOnlyOneProduct()
        {
            // Arrange
            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);
            var bestProduct = _shortProviderList[string.Empty][3];

            // Act
            d.Compare(_shortProviderList);

            // Assert
            foreach (var partNumber in _shortProviderList)
                foreach (var product in partNumber.Value)
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

            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);

            // Act
            d.Compare(shortProviderList);

            var list = shortProviderList.Single().Value;

            // Assert
            Assert.IsTrue(list.All(p => p.StatusProduktu == false));
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowErrorIfListIsEmpty()
        {
            // Arrange
            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);

            // Act
            d.Compare(new ConcurrentDictionary<string, IList<Product>>());

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses()
        {
            // Arrange
            foreach (var partNumber in _shortProviderList)
                foreach (var product in partNumber.Value)
                {
                    product.StanMagazynowy = 0;
                }

            PriceDomain d = new PriceDomain(allPartNumbersDomain, Formatter);

            // Act
            d.Compare(_shortProviderList);

            // Assert
            // Ok if no exceptions
        }
    }
}
