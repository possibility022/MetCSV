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

        private Product SelectOneProduct(IReadOnlyCollection<Product> products, string partNumber, IObjectFormatter<object> formatter)
        {
            if (products == null)
                return null;

            if (products.Count == 0)
                return null;

            formatter.WriteLine($"Zaczynam porównywać listę produktów dla PartNumberu [{partNumber}]: ");
            formatter.WriteObject(products);
            

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

            return cheapest;
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
            var cheapest = SelectOneProduct(productGroup.VendorProducts, productGroup.PartNumber, productGroup.ObjectFormatter);
            productGroup.FinalProduct.CenaZakupuNetto = cheapest.CenaZakupuNetto;
            productGroup.FinalProduct.SetCennaNetto(cheapest.CenaNetto);
            productGroup.CheapestProduct = cheapest;
        }
    }
}
