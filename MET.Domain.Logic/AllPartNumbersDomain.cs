using METCSV.Common.Exceptions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MET.Domain.Logic
{
    public class AllPartNumbersDomain : IAllPartsNumberDomain
    {
        private readonly ConcurrentDictionary<string, byte> allPartNumbers;
        static byte b = new byte();
        
        public AllPartNumbersDomain()
        {
            allPartNumbers = new ConcurrentDictionary<string, byte>();
        }

        public ConcurrentDictionary<string, byte> GetAllPartNumbers() => new ConcurrentDictionary<string, byte>(allPartNumbers);

        public void AddPartNumbers(params IEnumerable<Product>[] lists)
        {
            foreach (var list in lists)
            {
                foreach (var product in list)
                {
                    AddPartNumber(product.PartNumber);
                }
            }
        }

        public void AddPartNumber(string partNumber)
        {
            AddPartNumber(partNumber, 0);
        }

        private void AddPartNumber(string partNumber, int retry)
        {
            var success = allPartNumbers.TryAdd(partNumber, b);

            if (retry >= 10)
            {
                throw new OperationException($"We could not add item to the concurrent exception after {retry} tryouts. Item key: {partNumber}");
            }

            if (!success)
                AddPartNumber(partNumber, retry++);
        }

    }

    public interface IAllPartsNumberDomain
    {
        ConcurrentDictionary<string, byte> GetAllPartNumbers();
        void AddPartNumber(string partNumber);
    }
}
