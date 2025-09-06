using System.Collections.Generic;
using MET.Data.Models;
using MET.Domain.Logic.Models;

namespace METCSV.Tests.EngineTest.Extensions;

public static class ProductGroupExtensions
{
    public static void AddVendorProducts(this ProductGroup productGroup, IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            productGroup.AddVendorProduct(product);
        }
    }

    public static void AddMetProducts(this ProductGroup productGroup, IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            productGroup.AddMetProduct(product);
        }
    }
}