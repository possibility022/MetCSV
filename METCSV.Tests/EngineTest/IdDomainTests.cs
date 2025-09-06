using System;
using MET.Data.Models;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using METCSV.Tests.EngineTest.Extensions;
using METCSV.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class IdDomainTests
    {
        private IdDomain idDomain;
        private ProductGroup productGroup;

        [TestInitialize]
        public void TestInit()
        {
            idDomain = new IdDomain(false);
            productGroup = new ProductGroup(string.Empty, ZeroOutputFormatter.Instance);
        }

        [TestMethod]
        public void IdIsTakenFromMetProduct()
        {
            productGroup.AddMetProduct(new Product(Providers.Met) { Id = 123, SymbolSap = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });

            idDomain.ExecuteAction(productGroup);

            Assert.AreEqual(123, productGroup.FinalProduct.Id);
        }

        [TestMethod]
        public void IdIsNullWhenProductIsNew()
        {
            productGroup.AddVendorProducts(Factory.GetShortVendorList());

            idDomain.ExecuteAction(productGroup);

            Assert.AreEqual(null, productGroup.FinalProduct.Id);
        }

        [TestMethod]
        public void DoNotThrow_WhenFlagIsOn()
        {
            productGroup.AddMetProduct(new Product(Providers.Met) { Id = 123, SymbolSap = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });
            productGroup.AddMetProduct(new Product(Providers.Met) { Id = 1234, SymbolSap = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });
            idDomain = new IdDomain(true);

            idDomain.ExecuteAction(productGroup);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throw_WhenFlagIsOn()
        {
            productGroup.AddMetProduct(new Product(Providers.Met) { Id = 123, SymbolSap = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });
            productGroup.AddMetProduct(new Product(Providers.Met) { Id = 1234, SymbolSap = "ABC", NazwaProduktu = "Produkt", NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" });

            idDomain.ExecuteAction(productGroup);
        }

    }
}
