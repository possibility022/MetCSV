using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
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
            var name = GetName(productGroup);
            productGroup.FinalProduct.NazwaProduktu = name;
        }

        private string GetName(ProductGroup productGroup)
        {
            var objectFormatter = productGroup.ObjectFormatter;
            objectFormatter.WriteLine("Ustawiam nazwe produktu dla " + productGroup.PartNumber);

            string name;

            if (productGroup.MetProducts.Any())
            {
                objectFormatter.WriteLine("Mamy nazwe produktu ze sklepu met.");

                if (productGroup.MetProducts.Count > 1)
                {
                    objectFormatter.WriteLine("Mamy więcej niż jeden produkt dla tego part numberu na liście CSV z MET. Używam tego z dłuższą nazwą.");
                }

                var longestName = GetProductWithLongestName(productGroup.MetProducts);
                name = longestName.NazwaProduktu;
            }
            else
            {
                name = SelectNameFromVendorList(productGroup.VendorProducts);
            }

            return name;
        }

        private Product GetProductWithLongestName(IEnumerable<Product> products)
        {
            var enumerator = products.GetEnumerator();
            var selected = enumerator.Current;

            while (enumerator.MoveNext())
            {
                if (selected.NazwaProduktu.Length < enumerator.Current.NazwaProduktu.Length)
                {
                    selected = enumerator.Current;
                }
            }

            return selected;
        }

        private static IReadOnlyDictionary<Providers, int> Priorieties = new Dictionary<Providers, int>
        {
            {Providers.Lama, 0 },
            {Providers.TechData, 1 },
            {Providers.AB, 2 },
        };

        private string SelectNameFromVendorList(IReadOnlyCollection<Product> vendorProducts)
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
