using System;
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
        private IReadOnlyDictionary<string, ManufacturerProfit> manufacturerProfits;

        private double defaultProfit = 0.1;

        public PriceDomain()
        {

        }

        public void SetProfits(IReadOnlyCollection<CategoryProfit> category,
            IReadOnlyCollection<CustomProfit> custom,
            IReadOnlyCollection<ManufacturerProfit> manufacturers)
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
            this.manufacturerProfits = manufacturers.ToDictionary(r => r.Manufacturer);
        }

        public void SetDefaultProfit(double value)
        {
            defaultProfit = value;
        }

        private void CalculatePrice(string partNumber, IReadOnlyCollection<Product> inputProducts)
        {
            if (!inputProducts.Any())
                return;

            List<Product> products = new List<Product>(inputProducts);
            
            var customProfitsSet = CalculateCustomProfits(partNumber, products);

            if (customProfitsSet)
                return;

            CalculateCategoryProfits(products);
            CalculateManufacturersProfits(products);
            CalculateDefaultProfits(products);
        }

        private void CalculateDefaultProfits(IList<Product> products)
        {
            for (var i = 0; i < products.Count; i++)
            {
                var product = products[i];
                CalculateProfit(product, defaultProfit);
            }
        }

        private bool CalculateCustomProfits(string partNumber, IList<Product> products)
        {
            if (customProfits != null)
            {
                var containsKey = customProfits.ContainsKey(partNumber);

                if (containsKey)
                {
                    var profit = customProfits[partNumber].Profit;

                    for (var i = 0; i < products.Count; i++)
                    {
                        var product = products[i];
                        CalculateProfit(product, profit);
                    }

                    return true;
                }
            }

            return false;
        }

        private void CalculateManufacturersProfits(IList<Product> products)
        {
            if (manufacturerProfits != null)
            {
                for (var i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    if (manufacturerProfits.ContainsKey(product.NazwaProducenta))
                    {
                        var profit = manufacturerProfits[product.NazwaProducenta];
                        CalculateProfit(product, profit.Profit);
                        products.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void CalculateCategoryProfits(IList<Product> products)
        {
            if (categoryProfits != null)
            {
                for (var i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    if (categoryProfits.ContainsKey(product.Kategoria))
                    {
                        var profits = categoryProfits[product.Kategoria];
                        var profit = profits.FirstOrDefault(r => r.Provider == product.Provider);

                        if (profit != null)
                        {
                            CalculateProfit(product, profit.Profit);
                            products.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        private static void CalculateProfit(Product product, double profit)
        {
            var newPrice = (product.CenaZakupuNetto * profit) + product.CenaZakupuNetto;
            newPrice = Math.Round(newPrice, 2);
            product.SetCennaNetto(newPrice);

        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            CalculatePrice(productGroup.PartNumber, productGroup.VendorProducts);
        }
    }
}
