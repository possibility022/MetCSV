using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class PerformanceTests
    {
        [Ignore]
        [TestMethod]
        public void ListVsHashSets()
        {
            HashSet<string> generatedNumbers = Factory.GenerateUniqueSapNumbers(40000);

            HashSet<string> hashSetCollection = new HashSet<string>(generatedNumbers);
            List<string> listCollection = new List<string>(generatedNumbers);

            string[] lookFor = new string[1000];

            int i = 0;
            int index = 0;

            foreach(var val in generatedNumbers)
            {
                if (i < 1000)
                {
                    lookFor[index] = val;
                    index++;
                    if (index == lookFor.Length)
                    {
                        break;
                    }
                }
                i++;
            }

            long total = 0;
            var times = CollectionContains(hashSetCollection, lookFor, 500, out total);

            Trace.WriteLine($"Hashset Total(ms) : {total}");

            times = CollectionContains(listCollection, lookFor, 500, out total);
            
            Trace.WriteLine($"List Total(ms) : {total}");

            long totalHashSetLinq = CollectionContainsLinqMethod(hashSetCollection, lookFor, 500);
            long totalListLinq = CollectionContainsLinqMethod(listCollection, lookFor, 500);

            Trace.WriteLine($"Total Hashset LINQ: {totalHashSetLinq}");
            Trace.WriteLine($"Total List LINQ: {totalListLinq}");
        }

        private List<long> CollectionContains(ICollection<string> source, IEnumerable<string> lookForThisValues, int iterations, out long total)
        {
            List<long> times = new List<long>();
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var val in lookForThisValues)
                {
                    bool contains = source.Contains(val);
                }

                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
            }

            totalStopwatch.Stop();
            total = totalStopwatch.ElapsedMilliseconds;
            return times;
        }

        private long CollectionContainsLinqMethod(ICollection<string> source, IEnumerable<string> lookForThisValues, int iterations)
        {
            List<long> times = new List<long>();
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                
                foreach (var val in lookForThisValues)
                {
                    bool contains = source.Any(p => p == val);
                }
                
            }

            totalStopwatch.Stop();
            return totalStopwatch.ElapsedMilliseconds;
        }
    }
}
