using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;
using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
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

        [TestMethod]
        public async Task RemoveHiddenProducts()
        {
            var products = GetDefaultList("ABC");
            products.LamaProducts.First().Hidden = true;

            await filterDomain.RemoveProductsWithSpecificCode(products);

            Assert.AreEqual(0, products.LamaProducts.Count);
        }


        private Products GetDefaultList(string productCode)
        {
            return new Products()
            {
                LamaProducts = new List<Product>()
                {
                    new Product(Providers.Ab)
                    {
                        Id = 1, SymbolSap = "ABC",
                        NazwaProducenta = "Producent",
                        OryginalnyKodProducenta = productCode,
                        StanMagazynowy = 1, CenaZakupuNetto = 10
                    },
                }
            };
        }
    }
}
