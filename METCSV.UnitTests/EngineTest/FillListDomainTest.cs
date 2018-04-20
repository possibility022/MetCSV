using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using METCSV.WPF.Engine;
using System.Linq;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class FillListDomainTest
    {

        ConcurrentBag<Product> listToFill;
        IEnumerable<Product> metList;

        IEnumerable<Product> _shortMetList;
        ConcurrentBag<Product> _shortListToFill;

        [TestInitialize]
        public void InitializeData()
        {
            listToFill = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            metList = Factory.GetMetProducts();

            //Arrange
            _shortMetList = new[] {
                new Product { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" },
                new Product { UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2"   },
                new Product { UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3"  }
            };

            _shortListToFill = new ConcurrentBag<Product>();
            _shortListToFill.Add(new Product() { SymbolSAP = "ABC", NazwaProducenta = "Producent" });


        }

        [TestMethod]
        public void SettingEOLtoNotSelectedProducts()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList);
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            domain.FillList(_shortListToFill);

            //Assert
            foreach (var p in _shortMetList)
            {
                if (ReferenceEquals(p, productWithNonNullUrl))
                {
                    continue;
                }
                else
                {
                    Assert.AreEqual("EOL", p.Kategoria);
                }
            }
        }

        [TestMethod]
        public void ProductWithURLCanNotHaveEOL()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList);
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            domain.FillList(_shortListToFill);

            //Assert
            Assert.AreNotEqual("EOL", productWithNonNullUrl.Kategoria);
        }

        [TestMethod]
        public void AllProductsShouldHaveEmptyURL()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList);
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            domain.FillList(_shortListToFill);

            //Assert
            Assert.IsTrue(_shortMetList.All(p => p.UrlZdjecia == string.Empty));
        }

        [TestMethod]
        public void CopyIdTest()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList);
            var selectedProduct = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            var newList = domain.FillList(_shortListToFill);

            //Assert
            var prod = newList.First();
            Assert.AreEqual(selectedProduct.ID, prod.ID);
        }

        [TestMethod]
        public void SetNameTest()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList);
            var selectedProduct = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            var newList = domain.FillList(_shortListToFill);

            //Assert
            var prod = newList.First();
            Assert.AreEqual(selectedProduct.NazwaProduktu, prod.NazwaProduktu);
        }
    }
}
