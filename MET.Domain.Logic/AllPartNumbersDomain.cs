using METCSV.Common.Exceptions;
using METCSV.Common.ExtensionMethods;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MET.Domain.Logic
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
                var sucess = _allPartNumbers.TryAdd(product.PartNumber, b);
                int i = 0;

                if (_allPartNumbers.ContainsKey(product.PartNumber))
                    continue;

                while (!sucess && (i < 10))
                {
                    sucess = _allPartNumbers.TryAdd(product.PartNumber, b);
                    i++;
                    Thread.Sleep(i * 100);
                }

                if (i >= 10 && !sucess)
                {
                    throw new OperationException($"We could not add item to the concurrent exception after {i} tryouts. Item key: {product.PartNumber}");
                }
            }
        }
    }
}
