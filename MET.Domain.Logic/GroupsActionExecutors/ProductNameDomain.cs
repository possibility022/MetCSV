using System.Collections.Generic;
using System.Linq;
using METCSV.Common.Formatters;

namespace MET.Domain.Logic
{
    public class ProductNameDomain : IActionExecutor
    {

        /// 1. Nazwa produktu tak jak w pliku CSV ze sklepu met.
        /// 2. Jeżeli produkt jest nowy to wtedy nazwa jak z pliku od dystrybutora. Priorytety:
        /// Lama
        /// TD
        /// AB

        public void ExecuteAction(string partNumber, ICollection<Product> vendorProducts, ICollection<Product> metProducts, IObjectFormatter<object> objectFormatter)
        {
            objectFormatter.WriteLine("Ustawiam nazwe produktu dla " + partNumber);

            string name;

            if (!vendorProducts.Any())
                return;

            if (metProducts.Any())
            {
                objectFormatter.WriteLine("Mamy nazwe produktu ze sklepu met.");

                if (metProducts.Count > 1)
                {
                    objectFormatter.WriteLine("Mamy więcej niż jeden produkt dla tego part numberu na liście CSV z MET. Używam pierwszego na liście");
                }

                name = metProducts.First().NazwaProduktu;
            }
            else
            {
                name = SelectName(vendorProducts);
            }

            foreach(var prod in vendorProducts)
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

        private string SelectName(ICollection<Product> vendorProducts)
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
