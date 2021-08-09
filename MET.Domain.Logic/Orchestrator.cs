using System;
using System.Collections.Generic;
using System.Linq;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public class Orchestrator
    {
        private IAllPartsNumberDomain allPartNumbersDomain;
        private IObjectFormatterConstructor<object> objectFormatter;
        private ICollection<Product>[] lists;
        private ICollection<Product> metProducts;

        public Orchestrator(IAllPartsNumberDomain allPartNumbersDomain, IObjectFormatterConstructor<object> objectFormatter)
        {
            this.allPartNumbersDomain = allPartNumbersDomain;
            this.objectFormatter = objectFormatter;
        }

        public void LoadCollections(params ICollection<Product>[] products)
        {
            this.lists = products;
        }

        public void AddMetCollection(ICollection<Product> metProducts)
        {
            if (metProducts.Any(r => r.Provider != Providers.MET))
                throw new Exception("You can add only met products by this method");

            this.metProducts = metProducts;
        }

        public void Orchestrate()
        {
            var groupedProducts = new GroupedProducts();
            foreach (var list in lists)
            {
                foreach (var product in list)
                {
                    groupedProducts.AddProduct(product);
                    allPartNumbersDomain.AddPartNumber(product.PartNumber);
                }
            }

            foreach (var metProduct in metProducts)
            {
                groupedProducts.AddMetProduct(metProduct);
                allPartNumbersDomain.AddPartNumber(metProduct.PartNumber);
            }
        }
    }
}
