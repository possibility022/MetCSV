using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.Common.Formatters;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class ProductFilterDomainTests
    {
        private ProductFilterDomain filterDomain;

        // Program musi ignorować produkty u każdego dystrybutora, które mają w kodzie producenta: ?dopisek. np.
        // 2VLADAWK#2819?SAN 
        // ALJIDAW?A
        // ?AZAZ

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            Log.ConfigureNLogForTests();
        }

        [TestInitialize]
        public void TestInit()
        {
            filterDomain = new ProductFilterDomain(ZeroOutputFormatter.Instance);
        }

        [TestMethod]
        public async Task ProductWillBeRemovedFromList()
        {
            // Arrange
            var products = GetDefaultList("ABC?XYZ");

            // Act
            await filterDomain.RemoveProductsWithSpecificCode(products);
            
            // Assett
            Assert.AreEqual(0, products.LamaProducts.Count);
        }

        [TestMethod]
        public async Task ProductWill_NOT_BeRemovedFromList()
        {
            // Arrange
            var products = GetDefaultList("ABC");

            // Act
            await filterDomain.RemoveProductsWithSpecificCode(products);

            // Assett
            Assert.AreEqual(1, products.LamaProducts.Count);
        }

        [TestMethod]
        public async Task ProductWill_NOT_BeRemovedFromList_WhenThereIsQuestionMarkButNoMoreChars()
        {
            // Arrange
            var products = GetDefaultList("ABC?");

            // Act
            await filterDomain.RemoveProductsWithSpecificCode(products);

            // Assett
            Assert.AreEqual(1, products.LamaProducts.Count);
        }

        [TestMethod]
        public async Task ProductWillBeRemovedFromList_WhenStartsWithQuestionMark()
        {
            // Arrange
            var products = GetDefaultList("?xyz");

            // Act
            await filterDomain.RemoveProductsWithSpecificCode(products);

            // Assett
            Assert.AreEqual(0, products.LamaProducts.Count);
        }


        private Products GetDefaultList(string productCode)
        {
            return new Products()
            {
                LamaProducts = new List<Product>()
                {
                    new Product(Providers.AB)
                    {
                        ID = 1, SymbolSAP = "ABC",
                        NazwaProducenta = "Producent",
                        OryginalnyKodProducenta = productCode,
                        StanMagazynowy = 1, CenaZakupuNetto = 10
                    },
                }
            };
        }
    }
}
