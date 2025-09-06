using System.Collections.Concurrent;
using System.Collections.Generic;
using MET.Data.Models;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public class ProductGroupFactory
    {
        private readonly IObjectFormatterConstructor<object> constructor;

        public IReadOnlyDictionary<string, ProductGroup> Products => products;

        private readonly ConcurrentDictionary<string, ProductGroup> products;
        
        public ProductGroupFactory(IObjectFormatterConstructor<object> constructor)
        {
            products = new ConcurrentDictionary<string, ProductGroup>();
            this.constructor = constructor;
        }

        public void AddProduct(Product product)
        {
            AddToVendorCollection(product);
        }

        private void AddToVendorCollection(Product product)
        {
            products.AddOrUpdate(
                product.PartNumber,
                str =>
                {
                    var productGroup = new ProductGroup(product.PartNumber, this.constructor.GetNewInstance());
                    productGroup.AddVendorProduct(product);
                    return productGroup;
                },
                (key, oldValue) =>
                {
                    oldValue.AddVendorProduct(product);
                    return oldValue;
                });
        }

        private void AddToMetCollection(Product product)
        {
            products.AddOrUpdate(
                product.PartNumber,
                str =>
                {
                    var productGroup = new ProductGroup(product.PartNumber, this.constructor.GetNewInstance());
                    productGroup.AddMetProduct(product);
                    return productGroup;
                },
                (key, oldValue) =>
                {
                    oldValue.AddMetProduct(product);
                    return oldValue;
                });
        }


        public void AddMetProduct(Product metProduct)
        {
            AddToMetCollection(metProduct);
        }
    }
}
