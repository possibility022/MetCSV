using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MET.Domain.Logic.Models
{
    public class GroupedProducts
    {
        private readonly ConcurrentDictionary<string, IList<Product>> allProducts;

        private readonly ConcurrentDictionary<string, IList<Product>> metProducts;

        public GroupedProducts()
        {
            metProducts = new ConcurrentDictionary<string, IList<Product>>();
            allProducts = new ConcurrentDictionary<string, IList<Product>>();
        }

        public void AddProduct(Product products)
        {
            AddToCollection(products, allProducts);
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                AddToCollection(product, this.allProducts);
            }
        }

        private static void AddToCollection(Product product, ConcurrentDictionary<string, IList<Product>> collection)
        {
            collection.AddOrUpdate(
                product.PartNumber,
                new List<Product>() { product },
                (key, oldValue) => { oldValue.Add(product); return oldValue; });
        }

        public void AddMetProduct(Product metProduct)
        {
            if (metProduct.Provider != Providers.MET)
                throw new Exception("You can add only met products here.");

            AddToCollection(metProduct, metProducts);
        }

        public void ApplyActionPerGroup(Action<IList<Product>> action)
        {
            // todo pararrel action
        }
    }
}
