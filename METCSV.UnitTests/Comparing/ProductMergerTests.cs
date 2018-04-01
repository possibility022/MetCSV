using METCSV.Common;
using METCSV.WPF.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.UnitTests.Comparing
{
    [TestClass]
    class ProductMergerTests
    {

        [TestMethod]
        public void RemoveHiddenProducts()
        {
            var met = Factory.GetMetProducts();

            // Check if there is any hidden product.
            Assert.IsTrue(met.Any(p => p.Hidden));


            // Build list
            HiddenProductsDomain hiddenProducts = new HiddenProductsDomain();
            hiddenProducts.CreateListOfHiddenProducts(met);

            ConcurrentBag<Product> lamaProducts = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            ConcurrentBag<Product> tdProducts = new ConcurrentBag<Product>(Factory.GetTDProducts());
            ConcurrentBag<Product> abProducts = new ConcurrentBag<Product>(Factory.GetABProducts());

            ConcurrentBag<Product> clearList = hiddenProducts.RemoveHiddenProducts(lamaProducts);
            Assert.IsTrue(clearList.All(p => p.Hidden == false));

            clearList = hiddenProducts.RemoveHiddenProducts(tdProducts);
            Assert.IsTrue(clearList.All(p => p.Hidden == false));

            clearList = hiddenProducts.RemoveHiddenProducts(abProducts);
            Assert.IsTrue(clearList.All(p => p.Hidden == false));
        }
    }
}
