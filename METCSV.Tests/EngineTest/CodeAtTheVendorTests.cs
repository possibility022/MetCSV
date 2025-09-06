using System;
using MET.Data.Models;
using MET.Domain.Logic.GroupsActionExecutors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class CodeAtTheVendorTests
    {
        private CodeAtTheVendor domain;
        private Product final;

        [TestInitialize]
        public void TestInit()
        {
            final = new Product(Providers.None);
            domain = new CodeAtTheVendor();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ThrowWhenCheapestProductIsNotSet()
        {
            domain.ExecuteAction(null, new Product(Providers.Met));
        }

        [TestMethod]
        public void HasCorrectPrefixValue()
        {
            domain.ExecuteAction(new Product(Providers.None), final);

            Assert.IsTrue(final.KodDostawcy.StartsWith(Providers.None.ToString() + "_"));
        }

        [TestMethod]
        public void HasCorrectSuffixValue()
        {
            var source = new Product(Providers.None) { KodDostawcy = "12345" };
            domain.ExecuteAction(source, final);

            Assert.IsTrue(final.KodDostawcy.EndsWith("_12345"));
        }
    }
}
