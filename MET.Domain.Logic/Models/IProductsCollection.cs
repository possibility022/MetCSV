using MET.Data.Models;
using System;
using System.Collections.Generic;

namespace MET.Domain.Logic.Models
{
    public interface IProductsCollection : ICollection<KeyValuePair<Providers, ICollection<Product>>>
    {
    }

    public class ProductLists: List<KeyValuePair<Providers, ICollection<Product>>>, IProductsCollection
    {

        public void AddList(Providers provider, ICollection<Product> products)
        {
            var x = base.Find(r => r.Key == provider);

            if (x.Key == provider)
                throw new InvalidOperationException("Provider is already on the list.");

            base.Add(new KeyValuePair<Providers, ICollection<Product>>(provider, products));
        }

    }
}
