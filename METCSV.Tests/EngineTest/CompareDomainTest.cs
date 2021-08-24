using MET.Data.Models;
using MET.Domain;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class CompareDomainTest
    {
        private PriceDomain priceDomain;

        static ZeroOutputFormatter _formatter = ZeroOutputFormatter.Instance;

        private ProductGroup productGroup;

        [TestInitialize]
        public void InitializeData()
        {
            priceDomain = new PriceDomain();
            productGroup = new ProductGroup(null, _formatter);

            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 1,
                CenaZakupuNetto = 10
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 0,
                CenaZakupuNetto = 4
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 0,
                CenaZakupuNetto = 5
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 1,
                CenaZakupuNetto = 6
            });
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void EmptyWarehousesCanNotBeSelected()
        {
            // Act
            priceDomain.ExecuteAction(productGroup);

            //Assert
            Assert.AreNotEqual(productGroup.VendorProducts[1].CenaNetto, productGroup.FinalProduct);
            Assert.AreNotEqual(productGroup.VendorProducts[2].CenaNetto, productGroup.FinalProduct);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct()
        {
            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            Assert.AreEqual(productGroup.VendorProducts[3].CenaNetto, productGroup.FinalProduct.CenaNetto);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectCheapestProduct_IsSetInProductGroup()
        {
            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            Assert.AreEqual(productGroup.VendorProducts[3], productGroup.CheapestProduct);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowErrorIfListIsEmpty()
        {
            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void DoNotThrowExceptionIfListContainsOnlyEmptyWarehouses()
        {
            // Arrange
            foreach (var product in productGroup.VendorProducts)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            // Ok if no exceptions
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectTheCheapestWhenAllAreNotAvailable()
        {
            // Arrange
            foreach (var product in productGroup.VendorProducts)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            Assert.AreEqual(productGroup.VendorProducts[1].CenaNetto, productGroup.FinalProduct.CenaNetto);
        }

        [TestMethod]
        [Timeout(5 * 1000)]
        public void SelectTheCheapestWhenAllAreNotAvailable_IsSetInProductGroup()
        {
            // Arrange
            foreach (var product in productGroup.VendorProducts)
            {
                product.StanMagazynowy = 0;
            }

            // Act
            priceDomain.ExecuteAction(productGroup);

            // Assert
            Assert.AreEqual(productGroup.VendorProducts[1], productGroup.CheapestProduct);
        }
    }
}
