using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MET.Domain.Logic
{
    public class PriceErrorDomain
    {
        private readonly ConcurrentDictionary<string, Product> oldProducts = new ConcurrentDictionary<string, Product>();

        private IEnumerable<Product> newProducts;

        public PriceErrorDomain(IEnumerable<Product> oldProducts, IEnumerable<Product> newProducts)
        {
            ToConcurrentDictionary(oldProducts, ref this.oldProducts);
            this.newProducts = newProducts;
        }

        private void SetWarehouseToZero(Product p) => p.StanMagazynowy = 0;

        public void ValidateSingleProduct()
        {
            foreach(var p in newProducts)
            {
                if (!CheckProducts(p))
                    SetWarehouseToZero(p);
            }
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
                    $"SAP: {prod.SymbolSAP} " +
                    $"Nazwa Producent: {prod.NazwaProducenta}");
        }

        /// <summary>
        /// Check if price is ok
        /// </summary>
        /// <returns><c>ture</c> if product is ok, otherwaise false.</returns>
        private bool CheckProducts(Product newProduct)
        {
            if (!oldProducts.ContainsKey(newProduct.SapManuHash))
                return true;

            var oldP = oldProducts[newProduct.SapManuHash];

            if (oldP.CenaZakupuNetto < newProduct.CenaZakupuNetto)
                return true;

            return (oldP.CenaZakupuNetto * 20 / 100) > (oldP.CenaZakupuNetto - newProduct.CenaZakupuNetto);
        }
    }
}
