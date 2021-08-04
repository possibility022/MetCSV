using METCSV.Common.Exceptions;
using METCSV.Common.ExtensionMethods;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MET.Domain.Logic
{
    public class AllPartNumbersDomain
    {
        private ConcurrentDictionary<string, byte> _allPartNumbers;

        public ConcurrentDictionary<string, byte> GetAllPartNumbers(params IEnumerable<Product>[] lists)
        {
            _allPartNumbers = new ConcurrentDictionary<string, byte>();
            var tasks = new Task[lists.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => GetAllPartNumbers_Logic(lists[i]));
            }

            tasks.StartAll();
            tasks.WaitAll();

            return _allPartNumbers;
        }

        private void GetAllPartNumbers_Logic(IEnumerable<Product> products)
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
