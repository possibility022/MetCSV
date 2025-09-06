using System;
using MET.Data.Models;
using MET.Domain.Logic;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
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
            productGroup.DataSourceProduct = null;
            domain.ExecuteAction(productGroup);
        }

        [TestMethod]
        public void SetTheSameWarehouseValue()
        {
            productGroup.DataSourceProduct = new Product(Providers.None) { StanMagazynowy = 123 };
            domain.ExecuteAction(productGroup);

            Assert.AreEqual(123, productGroup.FinalProduct.StanMagazynowy);
        }

        [TestMethod]
        public void SetZeroWhenEol()
        {
            productGroup.DataSourceProduct = new Product(Providers.None) { StanMagazynowy = 123 };
            productGroup.FinalProduct.Kategoria = EndOfLiveDomain.EndOfLifeCategory;
            domain.ExecuteAction(productGroup);

            Assert.AreEqual(0, productGroup.FinalProduct.StanMagazynowy);
        }
    }
}
