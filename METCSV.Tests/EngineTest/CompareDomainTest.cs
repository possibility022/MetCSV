using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Data.Models.Profits;
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

        private const string PartNumber = "A_Producent";

        [TestInitialize]
        public void InitializeData()
        {
            priceDomain = new PriceDomain();
            productGroup = new ProductGroup(PartNumber, _formatter);

            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                Kategoria = "Kategoria",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 1,
                CenaZakupuNetto = 10
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                Kategoria = "Kategoria",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 0,
                CenaZakupuNetto = 4
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                Kategoria = "Kategoria",
                OryginalnyKodProducenta = "A",
                StanMagazynowy = 0,
                CenaZakupuNetto = 5
            });
            productGroup.AddVendorProduct(new Product(Providers.AB)
            {
                ID = 1,
                SymbolSAP = "ABC",
                NazwaProducenta = "Producent",
                Kategoria = "Kategoria",
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
            Assert.AreNotEqual(productGroup.VendorProducts[1].CenaNetto, productGroup.FinalProduct.CenaNetto);
            Assert.AreNotEqual(productGroup.VendorProducts[2].CenaNetto, productGroup.FinalProduct.CenaNetto);
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



        [TestMethod]
        public void CalculatePrice_UseDefault()
        {
            priceDomain.SetDefaultProfit(0.2);

            priceDomain.ExecuteAction(productGroup);

            foreach (var productGroupVendorProduct in productGroup.VendorProducts)
            {
                Assert.AreEqual(productGroupVendorProduct.CenaZakupuNetto + (productGroupVendorProduct.CenaZakupuNetto * 0.2), productGroupVendorProduct.CenaNetto);
            }
        }

        [TestMethod]
        public void CalculatePrice_UseCategory()
        {
            priceDomain.SetProfits(
                new List<CategoryProfit>()
                {
                    new CategoryProfit()
                    {
                        Category = "Kategoria",
                        Provider = Providers.AB,
                        Profit = 0.5,
                        Id = 1
                    }
                },
            new List<CustomProfit>()
            {

            },
                new List<ManufacturerProfit>());

            priceDomain.ExecuteAction(productGroup);

            var prod = productGroup.VendorProducts.Where(r => r.Kategoria == "Kategoria");

            foreach (var product in prod)
            {
                Assert.AreEqual(product.CenaZakupuNetto + (product.CenaZakupuNetto * 0.5), product.CenaNetto);
            }
        }

        [TestMethod]
        public void CalculatePrice_CustomProfitHasPriority()
        {
            priceDomain.SetProfits(
                new List<CategoryProfit>()
                {
                    new CategoryProfit()
                    {
                        Category = "Kategoria",
                        Provider = Providers.AB,
                        Profit = 0.5,
                        Id = 1
                    }
                },
                new List<CustomProfit>()
                {
                    new CustomProfit()
                    {
                        PartNumber = PartNumber,
                        Profit = 0.3
                    }
                }, new List<ManufacturerProfit>());

            priceDomain.ExecuteAction(productGroup);

            foreach (var product in productGroup.VendorProducts)
            {
                Assert.AreEqual(product.CenaZakupuNetto + (product.CenaZakupuNetto * 0.3), product.CenaNetto);
            }
        }

    }
}
