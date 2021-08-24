using System.Collections.Concurrent;
using System.Linq;
using MET.Data.Models;
using MET.Domain;
using MET.Domain.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.EngineTest
{
    [TestClass]
    public class AllPartNumbersDomainTest
    {
        private AllPartNumbersDomain allPartNumbers;

        [TestInitialize]
        public void Initialize()
        {
            allPartNumbers = new AllPartNumbersDomain();
        }

        [TestMethod]
        [Ignore] // this test take up to 4 min
        public void AllPartNumbersOnTheListShouldBeOnAtLeastOneOfTheRestLists()
        {
            var lama = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            var td = new ConcurrentBag<Product>(Factory.GetTDProducts());
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            allPartNumbers.AddPartNumbers(lama, met, td, ab);

            foreach(var v in allPartNumbers.GetAllPartNumbers())
            {
                Assert.IsTrue(
                    met.Any(p => p.PartNumber == v.Key) 
                    || ab.Any(p => p.PartNumber == v.Key) 
                    || td.Any(p => p.PartNumber == v.Key) 
                    || lama.Any(p => p.PartNumber == v.Key));
            }
        }


        [TestMethod]
        public void EachPartNumberShouldBeInNewList()
        {
            var lama = new ConcurrentBag<Product>(Factory.GetLamaProducts());
            var met = new ConcurrentBag<Product>(Factory.GetMetProducts());
            var td = new ConcurrentBag<Product>(Factory.GetTDProducts());
            var ab = new ConcurrentBag<Product>(Factory.GetABProducts());

            allPartNumbers.AddPartNumbers(lama, met, td, ab);
            var list = allPartNumbers.GetAllPartNumbers();

            IsOnTheList(lama, list);
            IsOnTheList(met, list);
            IsOnTheList(td, list);
            IsOnTheList(ab, list);
        }

        private static void IsOnTheList(ConcurrentBag<Product> sourceList, ConcurrentDictionary<string, byte> list)
        {
            foreach (var v in sourceList)
            {
                if (!list.ContainsKey(v.PartNumber))
                {
                    Assert.Fail();
                }
            }
        }
    }
}
