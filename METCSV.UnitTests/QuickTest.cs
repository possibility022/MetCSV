using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class QuickTest
    {
        [TestMethod]
        public void TryTakeFromConcurrentDictionaryWillRemoveItem()
        {
            var a = new ConcurrentDictionary<string, string>();
            a.TryAdd("key", "val");

            var taken = a.TryGetValue("key", out string val);

            Assert.IsTrue(taken);

            var taken2 = a.TryGetValue("key", out string val2);
            Assert.IsTrue(taken2);
        }

        [TestMethod]
        public void TryTakeFromConcurrentBagWhenIsEmpty()
        {
            var a = new ConcurrentBag<string>();
            var taken = a.TryTake(out string str);
            Assert.IsFalse(taken);
        }

        [TestMethod]
        public void TryTakeWillOverrideOldValueOnOutValue()
        {
            var str = "someValue";
            Trace.WriteLine($"str: {str}");
            var a = new ConcurrentBag<string>();
            var taken = a.TryTake(out str);
            Trace.WriteLine($"str: {str}");
            Assert.IsFalse(taken);
            Assert.IsNull(str);
        }

        [TestMethod]
        public void TryTakeFromDictionaryWithIntAsKey()
        {
            var dict = new ConcurrentDictionary<int, int>();

            int outValue = -12;

            dict.TryGetValue(1, out outValue);
            Trace.Write(outValue);
        }
    }
}
