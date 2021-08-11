using System.Collections.Generic;
using System.Linq;
using MET.Domain.Logic.Comparers;
using MET.Domain.Logic.Models;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class PriceDomain : IActionExecutor
    {
        // Program musi porównywać ceny od 3 dystrybutorów uwzględniając stan magazynowy powyżej 0. Wyświetlać ma się najniższa cena od dystrybutora, który ma na stanie minimum 1 szt. Produktów.
        // Program musi ignorować produkty u każdego dystrybutora które mają w kodzie producenta ?+dopisek np.. 2981273#AKD?PRX
        // Jeżeli u każdego dystrybutora stan wynosi 0 wtedy wyświetlić najmniejszą cenę

        readonly ProductByProductPrice netPriceComparer = new ProductByProductPrice();

        private void SelectOneProduct(IReadOnlyCollection<Product> products, string partNumber, IObjectFormatter<object> formatter)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            formatter.WriteLine($"Zaczynam porównywać listę produktów dla PartNumberu [{partNumber}]: ");
            formatter.WriteObject(products);


            // Todo move it to new domain
            //RemoveEmptyWarehouse(products, formatter);

            var availableProducts = products.Where(r => r.StanMagazynowy > 0).ToList();
            var includeAll = !availableProducts.Any();

            var workOnThisList = includeAll ? products : availableProducts;

            if (!includeAll)
            {
                formatter.WriteLine($"Wszystkie produkty dla {partNumber} są niedostępne");
            }

            formatter.WriteLine("Wybieram najtańszy produkt z listy:");
            formatter.WriteObject(workOnThisList);

            var cheapest = FindCheapestProduct(workOnThisList, includeAll);

            formatter.WriteLine($"Najtańszy produkt dla PartNumberu [{partNumber}] to:");
            formatter.WriteObject(cheapest);

            if (cheapest.ID != null)
                cheapest.StatusProduktu = true;

            formatter.Flush();
        }

        private void RemoveEmptyWarehouse(IList<Product> products, IObjectFormatter<object> formatter)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].StanMagazynowy <= 0)
                {
                    formatter.WriteLine("Stan magazynowy produktu jest pusty, usuwam go z listy: ");
                    formatter.WriteObject(products[i]);

                    products.RemoveAt(i);
                    i--;
                }
            }
        }

        private Product FindCheapestProduct(IReadOnlyCollection<Product> products, bool includeAll)
        {
            var cheapest = products.First(product => ProductFilter(product, includeAll));

            foreach (var product in products)
            {
                if (!ProductFilter(product, includeAll))
                    continue;

                var result = netPriceComparer.Compare(product, cheapest);

                if (result == -1)
                {
                    cheapest = product;
                }
            }

            return cheapest;
        }

        private static bool ProductFilter(Product p, bool includeNotAvailable)
        {
            if (includeNotAvailable)
                return true;

            return p.StanMagazynowy > 0;
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            SelectOneProduct(productGroup.VendorProducts, productGroup.PartNumber, productGroup.ObjectFormatter);
        }
    }
}
