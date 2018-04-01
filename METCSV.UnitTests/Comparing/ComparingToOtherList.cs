using System;
using System.Collections.Generic;
using System.IO;
using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace METCSV.UnitTests.Comparing
{
    [TestClass]
    public class ComparingToOtherList
    {
        [TestMethod]
        public void CompareNewConcurentWithDebugging()
        {
            var json_old = File.ReadAllText(@"D:\Programowanie\C#2017\tmpExport\concurent_v1.1.json");
            var json_new = File.ReadAllText(@"D:\Programowanie\C#2017\tmpExport\concurent_v2.1.json");

            List<Product> oldList = JsonConvert.DeserializeObject<List<Product>>(json_old);
            List<Product> newList = JsonConvert.DeserializeObject<List<Product>>(json_new);

            var comparer = new ProductComparer();

            oldList.Sort(comparer);
            newList.Sort(comparer);

            var json_old_sorted = JsonConvert.SerializeObject(oldList, Formatting.Indented);
            var json_new_sorted = JsonConvert.SerializeObject(newList, Formatting.Indented);

            File.WriteAllText(@"D:\Programowanie\C#2017\tmpExport\concurentSroted_v1.1.json", json_old_sorted);
            File.WriteAllText(@"D:\Programowanie\C#2017\tmpExport\concurentSroted_v2.1.json", json_new_sorted);

        }
    }
}
