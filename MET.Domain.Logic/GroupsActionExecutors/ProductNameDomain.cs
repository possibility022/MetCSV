using System.Collections.Generic;
using System.Linq;
using MET.Domain.Logic.Models;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class ProductNameDomain : IActionExecutor
    {

        /// 1. Nazwa produktu tak jak w pliku CSV ze sklepu met.
        /// 2. Jeżeli produkt jest nowy to wtedy nazwa jak z pliku od dystrybutora. Priorytety:
        /// Lama
        /// TD
        /// AB

        public void ExecuteAction(ProductGroup productGroup)
        {
            var objectFormatter = productGroup.ObjectFormatter;
            objectFormatter.WriteLine("Ustawiam nazwe produktu dla " + productGroup.PartNumber);

            string name;

            if (!productGroup.VendorProducts.Any())
                return;

            if (productGroup.MetProducts.Any())
            {
                objectFormatter.WriteLine("Mamy nazwe produktu ze sklepu met.");

                if (productGroup.MetProducts.Count > 1)
                {
                    objectFormatter.WriteLine("Mamy więcej niż jeden produkt dla tego part numberu na liście CSV z MET. Używam pierwszego na liście");
                }

                name = productGroup.MetProducts.First().NazwaProduktu;
            }
            else
            {
                name = SelectName(productGroup.VendorProducts);
            }

            foreach(var prod in productGroup.VendorProducts)
            {
                prod.NazwaProduktu = name;
            }
        }

        private static IReadOnlyDictionary<Providers, int> Priorieties = new Dictionary<Providers, int>
        {
            {Providers.Lama, 0 },
            {Providers.TechData, 1 },
            {Providers.AB, 2 },
        };

        private string SelectName(IReadOnlyCollection<Product> vendorProducts)
        {
            var first = vendorProducts.First();
            if (first != null)
            {
                foreach (var prod in vendorProducts)
                {
                    if (ReferenceEquals(first, prod))
                        continue;

                    if (Priorieties[prod.Provider] < Priorieties[first.Provider])
                    {
                        first = prod;
                    }
                }
            }

            return first.NazwaProduktu;
        }
    }
}
