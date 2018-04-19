using METCSV.Common;
using METCSV.WPF.ExtensionMethods;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace METCSV.WPF.Engine
{
    static class AllPartNumbersDomain
    {
        public static ConcurrentDictionary<string, byte> GetAllPartNumbers(ConcurrentBag<Product> list1, ConcurrentBag<Product> list2, ConcurrentBag<Product> list3, ConcurrentBag<Product> list4)
        {
            ConcurrentDictionary<string, byte> allPartNumbers = new ConcurrentDictionary<string, byte>();
            Task[] tasks = new Task[4];


            tasks[0] = new Task(() => GetAllPartNumbers_Logic(list1, allPartNumbers));
            tasks[1] = new Task(() => GetAllPartNumbers_Logic(list2, allPartNumbers));
            tasks[2] = new Task(() => GetAllPartNumbers_Logic(list3, allPartNumbers));
            tasks[3] = new Task(() => GetAllPartNumbers_Logic(list4, allPartNumbers));

            tasks.StartAll();
            tasks.WaitAll();

            return allPartNumbers;
        }

        private static void GetAllPartNumbers_Logic(ConcurrentBag<Product> products, ConcurrentDictionary<string, byte> _allPartNumbers)
        {
            byte b = new byte();

            foreach (var product in products)
            {
                _allPartNumbers.TryAdd(product.KodProducenta, b);
            }
        }
    }
}
