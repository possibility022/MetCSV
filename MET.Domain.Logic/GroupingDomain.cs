using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class GroupingDomain
    {
        ConcurrentDictionary<string, IList<Product>> _products;

        public ConcurrentDictionary<string, IList<Product>> CombineIntoGroups(params IEnumerable<Product>[] lists)
        {
            _products = new ConcurrentDictionary<string, IList<Product>>();

            foreach (var enumerable in lists)
            {
                Parallel.ForEach(enumerable, AddToCollection);
            }

            var tmp = _products;
            _products = null;

            return tmp;
        }

        private void AddToCollection(Product product)
        {
            _products.AddOrUpdate(
                product.PartNumber,
                new List<Product>() { product },
                (key, oldValue) => { oldValue.Add(product); return oldValue; });

        }
    }
}
