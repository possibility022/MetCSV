using System;
using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Domain;
using MET.Domain.Logic;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class PriceErrorTests
    {

        List<Product> oldProduct;
        List<Product> newProduct;

        PriceErrorDomain domain;


        [TestInitialize]
        public void Initialize()
        {
            oldProduct = new List<Product>()
            {
                new Product(Providers.AB)
                {
                    SymbolSAP = "SAP_123",
                    NazwaDostawcy = "DostawcaA",
                    CenaZakupuNetto = 100,
                    StanMagazynowy = 1
                },
                new Product(Providers.AB)
                {
                    SymbolSAP = "SAP_XYZ",
                    NazwaDostawcy = "DostawcaA",
                    CenaZakupuNetto = 200,
                    StanMagazynowy = 1
                }
            };

            newProduct = new List<Product>()
            {
                new Product(Providers.AB)
                {
                    SymbolSAP = "SAP_123",
                    NazwaDostawcy = "DostawcaA",
                    CenaZakupuNetto = 79,
                    StanMagazynowy = 2
                },
                new Product(Providers.AB)
                {
                    SymbolSAP = "SAP_XYZ",
                    NazwaDostawcy = "DostawcaA",
                    CenaZakupuNetto = 159,
                    StanMagazynowy = 2
                }
            };

            domain = new PriceErrorDomain(oldProduct, newProduct, 20, ZeroOutputFormatter.Instance);
        }
                


        [TestMethod]
        public void ShouldSetWarehouseToZero()
        {
            domain.ValidateSingleProduct();

            Assert.IsTrue(newProduct.All(r => r.StanMagazynowy == 0));
        }
    }
}
