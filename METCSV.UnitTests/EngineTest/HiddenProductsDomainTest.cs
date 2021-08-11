using System;
using System.Collections.Concurrent;
using System.Linq;
using MET.Domain;
using MET.Domain.Logic;
using METCSV.Common.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class HiddenProductsDomainTest
    {
        private static IObjectFormatterConstructor<object> formatterConstructor = ZeroOutputFormatter.Instance;
        

        [TestMethod]
        public void RemoveAllHiddenProducts()
        {
            var lama = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            var td = new ConcurrentBag<Product>(Factory.GetTDProducts());
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            HiddenProductsDomain domain = new HiddenProductsDomain(formatterConstructor);

            var hiddenProducts = domain.CreateListOfHiddenProducts(met);

            var metClear = domain.RemoveHiddenProducts(met);
            var lamaClear = domain.RemoveHiddenProducts(lama);
            var tdClear = domain.RemoveHiddenProducts(td);
            var abClear = domain.RemoveHiddenProducts(ab);

            Assert.IsTrue(metClear.All(p => p.Hidden == false));
            Assert.IsTrue(lamaClear.All(p => p.Hidden == false));
            Assert.IsTrue(tdClear.All(p => p.Hidden == false));
            Assert.IsTrue(abClear.All(p => p.Hidden == false));
        }

        [TestMethod]
        public void ListOfHiddenProductsContainsOnlyHiddenProducts()
        {
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            HiddenProductsDomain domain = new HiddenProductsDomain(formatterConstructor);
            var hiddenProducts = domain.CreateListOfHiddenProducts(met);

            Assert.IsTrue(hiddenProducts.All(p => p.Value.Hidden));
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ThrowInvalidOperationExceptionIfHiddenListIsNull()
        {
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            HiddenProductsDomain domain = new HiddenProductsDomain(formatterConstructor);

            var abClear = domain.RemoveHiddenProducts(ab);
        }
    }
}
