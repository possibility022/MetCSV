using MET.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MET.Domain.Logic;
using METCSV.Common.Formatters;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class FillListDomainTest
    {

        ConcurrentBag<Product> listToFill;
        IEnumerable<Product> metList;

        IEnumerable<Product> _shortMetList;
        ConcurrentBag<Product> _shortListToFill;

        static readonly IObjectFormatterConstructor<object> FormatterConstructor = ZeroOutputFormatter.Instance;
        

        [TestInitialize]
        public void InitializeData()
        {
            listToFill = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            metList = Factory.GetMetProducts();

            //Arrange
            _shortMetList = new[] {
                new Product (Providers.AB){ UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 1, NazwaProduktu = "Nazwa1" },
                new Product (Providers.AB){ UrlZdjecia = string.Empty, SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 2, NazwaProduktu = "Nazwa2"   },
                new Product (Providers.AB){ UrlZdjecia = "SomeURL", SymbolSAP = "ABC", NazwaProducenta = "Producent", ID = 3, NazwaProduktu = "Nazwa3"  }
            };

            _shortListToFill = new ConcurrentBag<Product>();
            _shortListToFill.Add(new Product(Providers.AB) { SymbolSAP = "ABC", NazwaProducenta = "Producent" });


        }

        [TestMethod]
        public void SettingEOLtoNotSelectedProducts()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList, FormatterConstructor);
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
            var domain = new FillListDomain(_shortMetList, FormatterConstructor);
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            domain.FillList(_shortListToFill);

            //Assert
            Assert.AreNotEqual("EOL", productWithNonNullUrl.Kategoria);
        }

        [TestMethod]
        public void OutProductShouldHaveEmptyURLIfOneOnTheMetListContainsURL()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList, FormatterConstructor);
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            var newList = domain.FillList(_shortListToFill);

            //Assert
            Assert.AreEqual(string.Empty, newList.First().UrlZdjecia);
        }

        [TestMethod]
        public void CopyIdTest()
        {
            //Arrange
            var domain = new FillListDomain(_shortMetList, FormatterConstructor);
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
            var domain = new FillListDomain(_shortMetList, FormatterConstructor);
            var selectedProduct = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            var newList = domain.FillList(_shortListToFill);

            //Assert
            var prod = newList.First();
            Assert.AreEqual(selectedProduct.NazwaProduktu, prod.NazwaProduktu);
        }

        #region SameTestUsingOldLogic

        [TestMethod]
        public void SettingEOLtoNotSelectedProducts_OLD()
        {
            //Arrange
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            FillList_OldMethod(new List<Product>(_shortListToFill), _shortMetList);

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
        public void ProductWithURLCanNotHaveEOL_OLD()
        {
            //Arrange
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            FillList_OldMethod(new List<Product>(_shortListToFill), _shortMetList);

            //Assert
            Assert.AreNotEqual("EOL", productWithNonNullUrl.Kategoria);
        }

        [TestMethod]
        public void OutProductShouldHaveEmptyURLIfOneOnTheMetListContainsURL_OLD()
        {
            //Arrange
            var productWithNonNullUrl = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            FillList_OldMethod(new List<Product>(_shortListToFill), _shortMetList);

            //Assert
            Assert.AreEqual(string.Empty, _shortListToFill.First().UrlZdjecia);
        }

        [TestMethod]
        public void CopyIdTest_OLD()
        {
            //Arrange
            var selectedProduct = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            FillList_OldMethod(new List<Product>(_shortListToFill), _shortMetList);

            //Assert
            var prod = _shortListToFill.First();
            Assert.AreEqual(selectedProduct.ID, prod.ID);
        }

        [TestMethod]
        public void SetNameTest_OLD()
        {
            //Arrange
            var selectedProduct = _shortMetList.First(p => p.UrlZdjecia.Length > 0);

            //Act
            FillList_OldMethod(new List<Product>(_shortListToFill), _shortMetList);

            //Assert
            var prod = _shortListToFill.First();
            Assert.AreEqual(selectedProduct.NazwaProduktu, prod.NazwaProduktu);
        }

        #region OldMethods

        private void FillList_OldMethod(List<Product> list, IEnumerable<Product> METProducts)
        {
            for (int i = 0; i < list.Count; i++)
            {
                List<Product> products = METProducts.Where(p => p.SymbolSAP == list[i].SymbolSAP).ToList();

                int workon = 0;
                if (products.Count >= 2)        //To jest tak że produkty w pliku METCSV się powtarzają. I wybierany jest ten gdzie jest URL
                {
                    for (int metProductIndex = 0; metProductIndex < products.Count; metProductIndex++)
                    {
                        if (products[metProductIndex].UrlZdjecia.Length > 0)
                        {
                            workon = metProductIndex;

                            for (int setEOL = 0; setEOL < products.Count; setEOL++)
                            {
                                if (setEOL == workon)
                                    continue;

                                products[setEOL].Kategoria = "EOL";
                                products[setEOL].UrlZdjecia = "";
                            }

                            break;
                        }
                    }
                    //throw new Exception("Znaleziono dwa takie same produkty w pliku MET od tego samego dostawcy.");
                }

                if (products.Count == 1)
                {
                    workon = 0;
                }

                if (products.Count > 0)
                {
                    if (products[workon].UrlZdjecia.Length > 0)
                        //list[i].UrlZdjecia = products[workon].UrlZdjecia;
                        list[i].UrlZdjecia = ""; // TO JEST Tak że jeśli zdjęcie już jest to ustawiamy puste. Jeśli nie ma to zostawiamy to od dostawcy.

                    list[i].ID = products[workon].ID;

                    if (products[0].NazwaProduktu != "")
                        list[i].NazwaProduktu = products[workon].NazwaProduktu;
                }
            }
        }

        #endregion

        #endregion
    }
}
