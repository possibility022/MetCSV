using METCSV.Common.Converters;
using METCSV.Common.ExtensionMethods;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class FillListDomain
    {

        /// <summary>
        /// This dictionary should contains Sap Manu Hash Set as the key and product as the value.
        /// </summary>
        ConcurrentDictionary<int, IList<Product>> _met;
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        IObjectFormatterConstructor<object> _objectFormatter;

        public FillListDomain(IEnumerable<Product> metBag, IObjectFormatterConstructor<object> objectFormatter)
        {
            _met = CustomConverters.ConvertToDictionaryOfLists(metBag, p => p.SapManuHash);
            _objectFormatter = objectFormatter;
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

        private void FillList_Logic(ConcurrentBag<Product> source, ConcurrentBag<Product> target)
        {
            Product product = null;

            while (source.TryTake(out product) || source.Count > 0)
            {
                if (product != null)
                {
                    var sb = new StringBuilder();// sadawd //todo refactor BasicJsonFormatter and interface



                    IList<Product> metOutList = null;
                    var taken = _met.TryGetValue(product.SapManuHash, out metOutList);

                    if (taken)
                    {
                        var selected = SelectOneProductAsDataSource(metOutList);
                        SetEOLToNotUsedProducts(metOutList, selected);

                        if (string.IsNullOrWhiteSpace(selected.UrlZdjecia) == false)
                        {
                            // TO JEST Tak że jeśli zdjęcie już jest to ustawiamy puste. Jeśli nie ma to zostawiamy to od dostawcy.
                            product.UrlZdjecia = string.Empty;
                        }

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
                            source.Add(product);
                            continue;
                        }
                    }

                    target.Add(product);
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

                // Copying to local variable. It will allow to use [ref]
                Product v = p;

                if (!ReferenceEquals(p, selectedProduct))
                {
                    EndOfLiveDomain.SetEndOfLife(ref v);
                }
                else
                {
                    p.UrlZdjecia = string.Empty;
                }
            }
        }

    }
}
