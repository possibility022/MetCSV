using System.Collections.Concurrent;
using System.Linq;
using METCSV.Common;
using METCSV.WPF.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class AllPartNumbersDomainTest
    {
        [TestMethod]
        public void AllPartNumbersOnTheListShouldBeOnAtLeastOneOfTheRestLists()
        {
            var lama = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            var td = new ConcurrentBag<Product>(Factory.GetTDProducts());
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            var list = AllPartNumbersDomain.GetAllPartNumbers(lama, met, td, ab);

            foreach(var v in list)
            {
                Assert.IsTrue(
                    met.Any(p => p.KodProducenta == v.Key) 
                    || ab.Any(p => p.KodProducenta == v.Key) 
                    || td.Any(p => p.KodProducenta == v.Key) 
                    || lama.Any(p => p.KodProducenta == v.Key));
            }
        }


        [TestMethod]
        public void EachPartNumberShouldBeInNewList()
        {
            var lama = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            var td = new ConcurrentBag<Product>(Factory.GetTDProducts());
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            var list = AllPartNumbersDomain.GetAllPartNumbers(lama, met, td, ab);
            
            IsOnTheList(lama, list);
            IsOnTheList(met, list);
            IsOnTheList(td, list);
            IsOnTheList(ab, list);
        }

        private static void IsOnTheList(ConcurrentBag<Product> ab, ConcurrentDictionary<string, byte> list)
        {
            foreach (var v in ab)
            {
                Assert.IsTrue(list.ContainsKey(v.KodProducenta));
            }
        }
    }
}
