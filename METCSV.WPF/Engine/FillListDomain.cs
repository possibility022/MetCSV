using METCSV.Common;
using METCSV.WPF.Converters;
using METCSV.WPF.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace METCSV.WPF.Engine
{
    class FillListDomain
    {

        /// <summary>
        /// This dictionary should contains Sap Manu Hash Set as the key and product as the value.
        /// </summary>
        ConcurrentDictionary<int, IList<Product>> _met;
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        public FillListDomain(IEnumerable<Product> metBag)
        {
            _met = CustomConverters.ConvertToDictionaryOfLists(metBag, p => p.SapManuHash);
        }

        public ConcurrentBag<Product> FillList(ConcurrentBag<Product> products, int? maxThreads = null)
        {
            if (maxThreads != null && maxThreads < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThreads));
            }

            ConcurrentBag<Product> newList = new ConcurrentBag<Product>();
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

        private void FillList_Logic(ConcurrentBag<Product> list, ConcurrentBag<Product> newList)
        {
            Product product = null;

            while (list.TryTake(out product) || list.Count > 0)
            {
                if (product != null)
                {
                    IList<Product> metOutList = null;
                    var taken = _met.TryGetValue(product.SapManuHash, out metOutList);

                    if (taken)
                    {
                        var selected = SelectOneProductAsDataSource(metOutList);
                        SetEOLToNotUsedProducts(metOutList, selected);

                        product.ID = selected.ID;

                        if (string.IsNullOrWhiteSpace(selected.NazwaProduktu) == false)
                        {
                            product.NazwaProduktu = selected.NazwaProduktu;
                        }
                    }
                    else
                    {
                        // return to list to try again later.
                        if (_met.ContainsKey(product.SapManuHash))
                        {
                            list.Add(product);
                            continue;
                        }
                    }

                    newList.Add(product);
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
                }

                p.UrlZdjecia = string.Empty;
            }
        }

    }
}
