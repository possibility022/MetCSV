using MET.Domain;
using MET.Domain.Logic.Extensions;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class IdDomainTests
    {
        private IdDomain idDomain;
        private ProductGroup productGroup;

        [TestInitialize]
        public void TestInit()
        {
            idDomain = new IdDomain();
            productGroup = new ProductGroup(string.Empty, ZeroOutputFormatter.Instance);
        }

        [TestMethod]
        public void IdIsTakenFromMetProduct()
        {
            productGroup.AddMetProduct(new Product(Providers.MET) { ID = 123, SymbolSAP = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });

            idDomain.ExecuteAction(productGroup);

            Assert.AreEqual(123, productGroup.FinalProduct.ID);
        }

        [TestMethod]
        public void IdIsNullWhenProductIsNew()
        {
            productGroup.AddVendorProducts(Factory.GetShortVendorList());

            idDomain.ExecuteAction(productGroup);

            Assert.AreEqual(null, productGroup.FinalProduct.ID);
        }

    }
}
