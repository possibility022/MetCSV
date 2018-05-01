using System.Collections.Generic;
using System.Linq;
using METCSV.Common;
using METCSV.WPF.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class CompareDomainTest
    {

        Product[] _shortProviderList;
        Dictionary<int, byte> _allPartNumbers;

        [TestInitialize]
        public void InitializeData()
        {
            _shortProviderList = new[] {
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 10},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 4},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 5},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 6}
            };

            _allPartNumbers = new Dictionary<int, byte>();

            byte b = new byte();

            foreach (var p in _shortProviderList)
            {
                _allPartNumbers[p.PartNumber] = b;
            }
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void EmptyWarehousesCanNotBeSelected()
        {
            //Arrange
            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(_shortProviderList, new Product[] { }, new Product[] { });

            //Assert
            Assert.IsFalse(_shortProviderList[1].StatusProduktu);
            Assert.IsFalse(_shortProviderList[2].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct()
        {
            // Arrange
            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(_shortProviderList, new Product[] { }, new Product[] { });

            // Assert
            Assert.IsTrue(_shortProviderList[3].StatusProduktu);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectOnlyOneProduct()
        {
            // Arrange
            CompareDomain d = new CompareDomain(_allPartNumbers);
            var bestProduct = _shortProviderList[3];

            // Act
            d.Compare(_shortProviderList, new Product[] { }, new Product[] { });

            // Assert
            foreach (var p in _shortProviderList)
            {
                if (object.ReferenceEquals(p, bestProduct))
                    continue;

                Assert.IsFalse(p.StatusProduktu);
            }
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void IfIdIsEqualToZeroThenIgnore()
        {
            // Arrange
            var shortProviderList = new List<Product>(_shortProviderList);
            shortProviderList.Add(new Product { ID = null, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 1 });

            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(shortProviderList, new Product[] { }, new Product[] { });

            // Assert
            Assert.IsTrue(shortProviderList.All(p => p.StatusProduktu == false));
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowErrorIfListIsEmpty()
        {
            // Arrange
            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(new Product[] { }, new Product[] { }, new Product[] { });

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses()
        {
            // Arrange
            foreach(var p in _shortProviderList)
            {
                p.StanMagazynowy = 0;
            }

            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(new Product[] { }, new Product[] { }, _shortProviderList);

            // Assert
            // Ok if no exceptions
        }
    }
}
