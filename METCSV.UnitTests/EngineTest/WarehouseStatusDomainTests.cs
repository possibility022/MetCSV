using System;
using MET.Domain;
using MET.Domain.Logic;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class WarehouseStatusDomainTests
    {
        private WarehouseStatusDomain domain;
        private ProductGroup productGroup;

        [TestInitialize]
        public void TestInit()
        {
            domain = new WarehouseStatusDomain();
            productGroup = new ProductGroup(string.Empty, ZeroOutputFormatter.Instance);
        }


        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ThrowWhenCheapestProductIsNotSet()
        {
            productGroup.CheapestProduct = null;
            domain.ExecuteAction(productGroup);
        }

        [TestMethod]
        public void SetTheSameWarehouseValue()
        {
            productGroup.CheapestProduct = new Product(Providers.None) { StanMagazynowy = 123 };
            domain.ExecuteAction(productGroup);

            Assert.AreEqual(123, productGroup.FinalProduct.StanMagazynowy);
        }

        [TestMethod]
        public void SetZeroWhenEol()
        {
            productGroup.CheapestProduct = new Product(Providers.None) { StanMagazynowy = 123 };
            productGroup.AddMetProduct(new Product(Providers.MET) { Kategoria = EndOfLiveDomain.EndOfLifeCategory });
            domain.ExecuteAction(productGroup);

            Assert.AreEqual(0, productGroup.FinalProduct.StanMagazynowy);
        }
    }
}
