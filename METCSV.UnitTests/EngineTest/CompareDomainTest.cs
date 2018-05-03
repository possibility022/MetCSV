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
        IList<string> _partNumbersForOldLogic;

        [TestInitialize]
        public void InitializeData()
        {
            _shortProviderList = new[] {
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 10},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 4},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 0, CenaZakupuNetto = 5},
                new Product {ID = 1, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 6}
            };

            _partNumbersForOldLogic = _shortProviderList.Select(p => p.KodProducenta).ToList();
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
        public void IfIdIsNullThenIgnore()
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
            foreach (var p in _shortProviderList)
            {
                p.StanMagazynowy = 0;
            }

            CompareDomain d = new CompareDomain(_allPartNumbers);

            // Act
            d.Compare(new Product[] { }, new Product[] { }, _shortProviderList);

            // Assert
            // Ok if no exceptions
        }


        #region OldMethod

        [TestMethod]
        public void EmptyWarehousesCanNotBeSelected_Old()
        {
            // Act
            OldMethod();

            //Assert
            Assert.IsFalse(_shortProviderList[1].StatusProduktu);
            Assert.IsFalse(_shortProviderList[2].StatusProduktu);
        }

        [TestMethod]

        public void SelectCheapestProduct_Old()
        {
            // Act
            OldMethod();

            // Assert
            Assert.IsTrue(_shortProviderList[3].StatusProduktu);
        }

        [TestMethod]

        public void SelectOnlyOneProduct_Old()
        {
            // Arrange
            var bestProduct = _shortProviderList[3];

            // Act
            OldMethod();

            // Assert
            foreach (var p in _shortProviderList)
            {
                if (object.ReferenceEquals(p, bestProduct))
                    continue;

                Assert.IsFalse(p.StatusProduktu);
            }
        }

        [TestMethod]

        public void IfIdIsNullThenIgnore_Old()
        {
            // Arrange
            var shortProviderList = new List<Product>(_shortProviderList);
            shortProviderList.Add(new Product { ID = null, SymbolSAP = "ABC", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A", StanMagazynowy = 1, CenaZakupuNetto = 1 });
            _shortProviderList = shortProviderList.ToArray();

            // Act
            OldMethod();

            // Assert
            Assert.IsTrue(shortProviderList.All(p => p.StatusProduktu == false));
        }

        [TestMethod]

        public void DoNotThrowErrorIfListIsEmpty_Old()
        {
            // Arrange
            _shortProviderList = new Product[] { };

            // Act
            OldMethod();

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]

        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses_Old()
        {
            // Arrange
            foreach (var p in _shortProviderList)
            {
                p.StanMagazynowy = 0;
            }

            // Act
            OldMethod();

            // Assert
            // Ok if no exceptions
        }

        #region OldLogic


        private void OldMethod()
        {
            foreach (var partNumber in _partNumbersForOldLogic)
            {
                Compare_OldMethod(partNumber);
            }
        }

        private void Compare_OldMethod(string partNumber)
        {
            var productsToCompare = _shortProviderList.Where(p => p.KodProducenta == partNumber);

            if (productsToCompare.Count() > 0)
            {
                List<Product> tmpList = new List<Product>();
                tmpList.AddRange(productsToCompare);
                selectOneProduct(tmpList);
            }
        }

        private void selectOneProduct(List<Product> products)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            RemoveEmptyWarehouse(products);
            int cheapest = FindCheapestProduct(products);

            if (products[cheapest].ID != null)
                products[cheapest].StatusProduktu = true;
        }

        private void RemoveEmptyWarehouse(IList<Product> products)
        {
            int empty = 0;
            for (int i = 0; i < products.Count; i++)
                if (products[i].StanMagazynowy <= 0)
                    empty++;

            if (empty == products.Count)
                return;
            else
                for (int i = 0; i < products.Count; i++)
                    if (products[i].StanMagazynowy <= 0)
                    {
                        products.RemoveAt(i);
                        i--;
                    }
        }

        private int FindCheapestProduct(IList<Product> products)
        {
            int cheapest = 0;
            for (int i = 1; i < products.Count; i++)
            {
                if (products[i].CenaZakupuNetto < products[cheapest].CenaZakupuNetto)
                    cheapest = i;
            }

            return cheapest;
        }

        #endregion
        #endregion
    }
}
