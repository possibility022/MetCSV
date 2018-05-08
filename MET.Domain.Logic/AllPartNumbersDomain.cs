using METCSV.Common;
using METCSV.Common.ExtensionMethods;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace METCSV.Domain.Logic
{
    public static class AllPartNumbersDomain
    {
        public static ConcurrentDictionary<int, byte> GetAllPartNumbers(ConcurrentBag<Product> list1, ConcurrentBag<Product> list2, ConcurrentBag<Product> list3, ConcurrentBag<Product> list4)
        {
            ConcurrentDictionary<int, byte> allPartNumbers = new ConcurrentDictionary<int, byte>();
            Task[] tasks = new Task[4];


            tasks[0] = new Task(() => GetAllPartNumbers_Logic(list1, allPartNumbers));
            tasks[1] = new Task(() => GetAllPartNumbers_Logic(list2, allPartNumbers));
            tasks[2] = new Task(() => GetAllPartNumbers_Logic(list3, allPartNumbers));
            tasks[3] = new Task(() => GetAllPartNumbers_Logic(list4, allPartNumbers));

            tasks.StartAll();
            tasks.WaitAll();

            return allPartNumbers;
        }

        private static void GetAllPartNumbers_Logic(ConcurrentBag<Product> products, ConcurrentDictionary<int, byte> _allPartNumbers)
        {
            byte b = new byte();

            foreach (var product in products)
            {
                _allPartNumbers.TryAdd(product.PartNumber, b);
            }
        }
    }
}
