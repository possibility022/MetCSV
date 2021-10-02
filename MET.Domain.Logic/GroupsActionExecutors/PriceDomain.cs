using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Data.Models.Profits;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class PriceDomain : IActionExecutor
    {
        // Program musi porównywać ceny od 3 dystrybutorów uwzględniając stan magazynowy powyżej 0. Wyświetlać ma się najniższa cena od dystrybutora, który ma na stanie minimum 1 szt. Produktów.
        // Program musi ignorować produkty u każdego dystrybutora które mają w kodzie producenta ?+dopisek np.. 2981273#AKD?PRX
        // Jeżeli u każdego dystrybutora stan wynosi 0 wtedy wyświetlić najmniejszą cenę

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
                            CalculateProfit(product, defaultProfit);
                        }
                    }

                    return;
                }

                foreach (var product in products)
                {
                    CalculateProfit(product, defaultProfit);
                }
            }
        }

        private static void CalculateProfit(Product product, double profit)
        {
            var newPrice = (product.CenaZakupuNetto * profit) + product.CenaZakupuNetto;
            product.SetCennaNetto(newPrice);
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            CalculatePrice(productGroup.PartNumber, productGroup.VendorProducts);
        }
    }
}
