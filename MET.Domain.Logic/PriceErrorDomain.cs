using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic
{
    public class PriceErrorDomain
    {
        private readonly ConcurrentDictionary<string, Product> oldProducts = new ConcurrentDictionary<string, Product>();

        private IEnumerable<Product> newProducts;
        private readonly IObjectFormatter<object> objectFormatter;

        readonly int maxDifference;

        public PriceErrorDomain(IEnumerable<Product> oldProducts, IEnumerable<Product> newProducts, int maxDifference, IObjectFormatter<object> objectFormatter)
        {
            if (maxDifference < 0 || maxDifference > 100)
                throw new ArgumentException(nameof(maxDifference));

            ToConcurrentDictionary(oldProducts, ref this.oldProducts);
            this.newProducts = newProducts;
            this.objectFormatter = objectFormatter;
            this.maxDifference = maxDifference;
        }

        private void SetWarehouseToZero(Product p)
        {
            p.StanMagazynowy = 0;
        }

        public void ValidateSingleProduct()
        {
            foreach (var p in newProducts)
            {
                if (!CheckProducts(p, out var oldProduct))
                {
                    objectFormatter.WriteLine($"Ustawiam stan magazynowy produktu ponieważ cena uległa znaczącej zmianie. Nowa cena: {p.CenaZakupuNetto}, {oldProduct.CenaZakupuNetto}. Nowy produkt: ");
                    objectFormatter.WriteObject(p);
                    SetWarehouseToZero(p);
                }
            }

            objectFormatter.Flush();
        }

        private void ToConcurrentDictionary(IEnumerable<Product> oldProducts, ref ConcurrentDictionary<string, Product> dict)
        {
            foreach (var prod in oldProducts)
            {
                var results = dict.TryAdd(prod.SapManuHash, prod);
                ThrowExceptionIfFail(prod, results);
            }
        }

        private static void ThrowExceptionIfFail(Product prod, bool results)
        {
            if (!results)
                throw new InvalidOperationException($"Product with the same key exists. " +
                    $"SapManuHash: {prod.SapManuHash} " +
                    $"SAP: {prod.SymbolSap} " +
                    $"Nazwa Producent: {prod.NazwaProducenta}");
        }

        /// <summary>
        /// Check if price is ok
        /// </summary>
        /// <returns><c>ture</c> if product is ok, otherwaise false.</returns>
        private bool CheckProducts(Product newProduct, out Product oldProduct)
        {
            oldProduct = null;

            if (!oldProducts.ContainsKey(newProduct.SapManuHash))
                return true;

            oldProduct = oldProducts[newProduct.SapManuHash];

            if (oldProduct.CenaZakupuNetto < newProduct.CenaZakupuNetto)
                return true;

            return (oldProduct.CenaZakupuNetto * maxDifference / 100) > (oldProduct.CenaZakupuNetto - newProduct.CenaZakupuNetto);
        }
    }
}
