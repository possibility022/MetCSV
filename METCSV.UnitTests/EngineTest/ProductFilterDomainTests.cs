using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductFilterDomainTests
    {
        private ProductFilterDomain filterDomain;

        [TestInitialize]
        public void TestInit()
        {
            filterDomain = new ProductFilterDomain();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            await filterDomain.RemoveProductsWithSpecificCode(new Products()
            {
                LamaProducts = new List<Product>()
                {
                    new Product(Providers.AB)
                    {
                        ID = 1, SymbolSAP = "ABC", 
                        NazwaProducenta = "Producent", 
                        OryginalnyKodProducenta = "A?x",
                        StanMagazynowy = 1, CenaZakupuNetto = 10
                    },
                }
            });
        }
    }
}
