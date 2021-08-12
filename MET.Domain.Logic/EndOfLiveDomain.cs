using System.Linq;
using MET.Domain.Logic.GroupsActionExecutors;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic
{
    public class EndOfLiveDomain : IActionExecutor
    {
        public const string EndOfLifeCategory = "EOL";
        public const string EndOfLifeProductNamePrefix = "EOL - wycofany z oferty - ";

        public static void SetEndOfLife(Product p)
        {
            p.Kategoria = EndOfLifeCategory;
            p.StatusProduktu = false;
            p.SetCennaNetto(0);
            p.CenaZakupuNetto = 0;
            p.StanMagazynowy = 0;
        }

        private static void AddPrefixToProductName(Product p)
        {
            if (p.NazwaProduktu != null && !p.NazwaProduktu.StartsWith(EndOfLifeProductNamePrefix))
                p.NazwaProduktu = $"{EndOfLifeProductNamePrefix}{p.NazwaProduktu}";
        }

        public void ExecuteAction(ProductGroup productGroup)
        {
            var formatter = productGroup.ObjectFormatter;
            formatter.WriteLine("Sprawdzam czy ustawić status EOL.");

            if (!productGroup.VendorProducts.Any())
            {
                foreach (var product in productGroup.MetProducts)
                {
                    formatter.WriteLine($"Brakuje produktów u dostawcow. Ustawiam EOL dla {product}: ");
                    formatter.WriteObject(product);
                    SetEndOfLife(product);
                    AddPrefixToProductName(product);
                }

            }
        }
    }
}
