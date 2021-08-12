using System;
using System.Collections.Generic;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic.Models
{
    public class ProductGroup
    {
        public string PartNumber { get; }
        private readonly List<Product> vendorProducts = new List<Product>(3);
        private readonly List<Product> metProducts = new List<Product>(1);

        public IReadOnlyList<Product> VendorProducts => vendorProducts;
        public IReadOnlyList<Product> MetProducts => metProducts;

        public IObjectFormatter<object> ObjectFormatter { get; private set; }


        public ProductGroup(string partNumber, IObjectFormatter<object> objectFormatter)
        {
            this.PartNumber = partNumber;
            ObjectFormatter = objectFormatter;
        }

        public void AddVendorProduct(Product product)
        {
            if (product.Provider == Providers.MET)
                throw new InvalidOperationException("Cannot add product from MET provider to vendor list.");

            vendorProducts.Add(product);
        }

        public void AddMetProduct(Product product)
        {
            if (product.Provider != Providers.MET)
                throw new InvalidOperationException($"Cannot add product to MET list with given provider: {product.Provider}");

            metProducts.Add(product);
        }
    }
}