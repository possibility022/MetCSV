using System.Collections.Concurrent;
using System.Collections.Generic;
using METCSV.Common;
using METCSV.WPF.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Comparing
{
    [TestClass]
    public class AllPartNumbersDomainTests
    {

        [TestMethod]
        public void AllPartNumbersTest()
        {
            var met = Factory.GetMetProducts();
            var lama = Factory.GetLamaProducts();
            var td = Factory.GetTDProducts();
            var ab = Factory.GetABProducts();

            var allPartNumbers = BuildPartNumberList(lama, td, ab, met);

            var allPartNumbers_new = AllPartNumbersDomain.GetAllPartNumbers(new ConcurrentBag<Product>(met), new ConcurrentBag<Product>(lama), new ConcurrentBag<Product>(td), new ConcurrentBag<Product>(ab));

            Assert.AreEqual(allPartNumbers.Count, allPartNumbers_new.Count);

            foreach (var p in allPartNumbers)
            {
                Assert.IsTrue(allPartNumbers_new.ContainsKey(p));
            }
        }

        // Old method
        private HashSet<string> BuildPartNumberList(IEnumerable<Product> lama, IEnumerable<Product> techData, IEnumerable<Product> ab, IEnumerable<Product> metProducts)
        {
            HashSet<string> list = new HashSet<string>();

            foreach (Product p in lama)
                AddIfDoesNotExists(p.KodProducenta, ref list);


            foreach (Product p in techData)
                AddIfDoesNotExists(p.KodProducenta, ref list);


            foreach (Product p in ab)
                AddIfDoesNotExists(p.KodProducenta, ref list);


            if (metProducts != null)
                foreach (Product p in metProducts)
                    AddIfDoesNotExists(p.KodProducenta, ref list);

            return list;
        }

        private void AddIfDoesNotExists(string what, ref HashSet<string> targetList)
        {
            if (targetList.Contains(what) == false)
                targetList.Add(what);
        }
    }
}
