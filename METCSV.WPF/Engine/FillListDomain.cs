using METCSV.Common;
using METCSV.WPF.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Engine
{
    class FillListDomain
    {

        /// <summary>
        /// This dictionary should contains Sap Number as the key and product as the value.
        /// </summary>
        ConcurrentDictionary<string, IList<Product>> _met;
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        public FillListDomain(ConcurrentDictionary<string, IList<Product>> metBag)
        {
            _met = metBag;
        }

        public ConcurrentDictionary<string, Product> GetPartNumbersConfilcts()
        {
            return _partNumbersConfilcts;
        }

        public ConcurrentDictionary<string, Product> FillList(ConcurrentBag<Product> products, int? maxThreads = null)
        {
            if (maxThreads != null && maxThreads < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThreads));
            }

            ConcurrentDictionary<string, Product> newList = new ConcurrentDictionary<string, Product>();
            int threads = maxThreads ?? Environment.ProcessorCount;
            Task[] tasks = new Task[threads];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => FillList_Logic(products, newList));
            }

            tasks.StartAll();
            tasks.WaitAll();

            return newList;
        }

        private void FillList_Logic(ConcurrentBag<Product> list, ConcurrentDictionary<string, Product> newList)
        {
            Product product = null;

            while (list.TryTake(out product) && list.Count > 0)
            {
                //var products = _met.Where(m => m.Key == product.SymbolSAP).ToList(); //todo can be optimized

                IList<Product> metOutList = null;
                var taken = _met.TryGetValue(product.SymbolSAP, out metOutList);

                if (taken)
                {
                    var selected = SelectOneProductAsDataSource(metOutList);
                    SetEOLToNotUsedProducts(metOutList, selected);

                    if (string.IsNullOrWhiteSpace(selected.UrlZdjecia) == false)
                    {
                        selected.UrlZdjecia = string.Empty;
                    }

                    product.ID = selected.ID;

                    if (string.IsNullOrWhiteSpace(selected.NazwaProduktu) == false)
                    {
                        product.NazwaProduktu = selected.NazwaProduktu;
                    }
                }
                else
                {
                    //log it
                }
            }
        }

        /// <summary>
        /// Selects the one product as data source.
        /// </summary>
        /// <param name="metOutList">The products.</param>
        /// <returns></returns>
        private Product SelectOneProductAsDataSource(IEnumerable<Product> metOutList)
        {
            foreach (var p in metOutList)
            {
                if (p.UrlZdjecia.Length > 0)
                {
                    return p;
                }
            }

            return metOutList.First();
        }

        private void SetEOLToNotUsedProducts(IEnumerable<Product> products, Product selectedProduct)
        {
            foreach (var p in products)
            {
                if (object.ReferenceEquals(p, selectedProduct) == false)
                {
                    p.Kategoria = "EOL";
                    p.UrlZdjecia = "";
                }
            }
        }

    }
}
