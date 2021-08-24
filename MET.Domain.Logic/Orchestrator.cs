using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public class Orchestrator
    {
        public Orchestrator(bool ignoreIdsProblems)
        {
            groupExecutors = new IActionExecutor[]
            {
                new NewProductSetter(),
                new ProductNameDomain(),
                new IdDomain(ignoreDuplicates: ignoreIdsProblems),
                new EndOfLiveDomain(),
                new PriceDomain(),
                new SourceProductSelector(),
                new WarehouseStatusDomain(),
                new ProductStatusDomain(),
            };

            finalProductConstructors = new IFinalProductConstructor[]
            {
                new CodeAtTheVendor(),
                new SapNumberDomain(),
                new EanDomain(),
                new ProductStatusDomain(),
                new CopyPropertiesValues(),

                new OverrideDefaultValuesDomain(), // this one should be last
            };
        }

        public Orchestrator(IAllPartsNumberDomain allPartNumbersDomain, IObjectFormatterConstructor<object> objectFormatter, bool ignoreIdsProblems = false) : this(ignoreIdsProblems)
        {
            this.allPartNumbersDomain = allPartNumbersDomain;
            this.objectFormatter = objectFormatter;
        }

        private readonly IAllPartsNumberDomain allPartNumbersDomain;
        private readonly IObjectFormatterConstructor<object> objectFormatter;
        private ICollection<Product>[] lists;
        private ICollection<Product> metProducts;
        private IReadOnlyCollection<ProductGroup> finalGroups;
        private IReadOnlyCollection<ProductGroup> internalList;

        private readonly IActionExecutor[] groupExecutors;
        private readonly IFinalProductConstructor[] finalProductConstructors;

        public void SetCollections(params ICollection<Product>[] products)
        {
            this.lists = products;
        }

        public void AddMetCollection(ICollection<Product> metProducts)
        {
            if (metProducts.Any(r => r.Provider != Providers.MET))
                throw new Exception("You can add only met products by this method");

            this.metProducts = metProducts;
        }

        public async Task Orchestrate()
        {
            var filter = new ProductFilterDomain(objectFormatter);
            await filter.RemoveProductsWithSpecificCode(lists);
            
            internalList ??= GroupProducts();

            foreach (var groupedProduct in internalList)
            {
                ExecuteActions(groupedProduct);
            }

            var results = Parallel.ForEach(internalList, ExecuteActions);
            finalGroups = internalList;
        }

        public IReadOnlyCollection<ProductGroup> GetGeneratedProductGroups() => finalGroups;

        private void ExecuteActions(ProductGroup productGroup)
        {
            for (int i = 0; i < groupExecutors.Length; i++)
            {
                groupExecutors[i].ExecuteAction(productGroup);
            }

            for (int i = 0; i < finalProductConstructors.Length; i++)
            {
                finalProductConstructors[i].ExecuteAction(productGroup.DataSourceProduct, productGroup.FinalProduct);
            }
        }

        private IReadOnlyList<ProductGroup> GroupProducts()
        {
            var groupedProducts = new ProductGroupFactory(objectFormatter);
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

            return new List<ProductGroup>(groupedProducts.Products.Values);
        }
    }
}
