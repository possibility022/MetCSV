using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Data.Models.Profits;
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

        private IReadOnlyDictionary<string, List<CategoryProfit>> categoryProfits;
        private IReadOnlyDictionary<string, CustomProfit> customProfits;

        private double defaultProfit = 0.1;

        public PriceDomain()
        {

        }

        public void SetProfits(
            IReadOnlyCollection<CategoryProfit> category,
            IReadOnlyCollection<CustomProfit> custom)
        {
            var profits = new Dictionary<string, List<CategoryProfit>>();
            foreach (var categoryProfit in category)
            {
                if (profits.ContainsKey(categoryProfit.Category))
                {
                    profits[categoryProfit.Category].Add(categoryProfit);
                }
                else
                {
                    profits[categoryProfit.Category] = new List<CategoryProfit>() { categoryProfit };
                }
            }

            this.categoryProfits = profits;
            this.customProfits = custom.ToDictionary(r => r.PartNumber);
        }

        public void SetDefaultProfit(double value)
        {
            defaultProfit = value;
        }

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

        private void CalculatePrice(string partNumber, IReadOnlyCollection<Product> products)
        {
            if (products.Any())
            {
                if (customProfits != null)
                {
                    var containsKey = customProfits.ContainsKey(partNumber);

                    if (containsKey)
                    {
                        var profit = customProfits[partNumber].Profit;

                        foreach (var product in products)
                        {
                            CalculateProfit(product, profit);
                        }

                        return;
                    }
                }

                if (categoryProfits != null)
                {
                    foreach (var product in products)
                    {
                        if (categoryProfits.ContainsKey(product.Kategoria))
                        {
                            var profits = categoryProfits[product.Kategoria];
                            var profit = profits.FirstOrDefault(r => r.Provider == product.Provider);

                            if (profit != null)
                            {
                                CalculateProfit(product, profit.Profit);
                            }
                            else
                            {
                                CalculateProfit(product, defaultProfit);
                            }
                        }
                        else
                        {
                            product.SetCennaNetto(product.CenaZakupuNetto * defaultProfit);
                        }
                    }

                    return;
                }

                foreach (var product in products)
                {
                    product.SetCennaNetto(product.CenaZakupuNetto * defaultProfit);
                }
            }
        }

        private static void CalculateProfit(Product product, double profit)
        {
            product.SetCennaNetto((product.CenaZakupuNetto * profit) + product.CenaZakupuNetto);
        }

        private static bool ProductFilter(Product p, bool includeNotAvailable)
        {
            if (includeNotAvailable)
                return true;

            return p.StanMagazynowy > 0;
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            CalculatePrice(productGroup.PartNumber, productGroup.VendorProducts);

            var cheapest = SelectOneProduct(productGroup.VendorProducts, productGroup.PartNumber, productGroup.ObjectFormatter);
            if (cheapest == null)
                return;

            productGroup.FinalProduct.CenaZakupuNetto = cheapest.CenaZakupuNetto;
            productGroup.FinalProduct.SetCennaNetto(cheapest.CenaNetto);
            productGroup.CheapestProduct = cheapest;
        }
    }
}
