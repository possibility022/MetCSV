using System;
using MET.Data.Models;
using MET.Domain.Logic.GroupsActionExecutors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class SapNumberDomainTests
    {
        private SapNumberDomain domain;
        private Product final;

        [TestInitialize]
        public void TestInit()
        {
            final = new Product(Providers.None);
            domain = new SapNumberDomain();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ThrowWhenSourceIsNull()
        {
            domain.ExecuteAction(null, new Product(Providers.MET));
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ThrowWhenFinalIsNull()
        {
            domain.ExecuteAction(new Product(Providers.MET), null);
        }

        [TestMethod]
        public void HasCorrectPrefixValue()
        {
            domain.ExecuteAction(new Product(Providers.None), final);

            Assert.IsTrue(final.SymbolSAP.StartsWith(SapNumberDomain.Prefix + "_"));
        }

        [TestMethod]
        public void HasCorrectSuffixValue()
        {
            var source = new Product(Providers.None) { SymbolSAP = "12345" };
            domain.ExecuteAction(source, final);

            Assert.IsTrue(final.SymbolSAP.EndsWith("_12345"));
        }
    }
}
