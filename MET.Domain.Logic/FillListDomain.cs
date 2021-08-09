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
        ConcurrentDictionary<string, IList<Product>> _met;
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

            var newList = new ConcurrentBag<Product>();
            int threads = maxThreads ?? Environment.ProcessorCount;
            var tasks = new Task[threads];

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

            var formatter = _objectFormatter.GetNewInstance();

            while (source.TryTake(out product) || source.Count > 0)
            {
                if (product != null)
                {
                    IList<Product> metOutList = null;
                    var taken = _met.TryGetValue(product.SapManuHash, out metOutList);

                    if (taken)
                    {
                        formatter.WriteLine($"Wypełniam dane produktu: [{product.SymbolSAP}] i jego dane na podstawie pliku met.");
                        formatter.WriteLine("Produkt przed zmianami: ");
                        formatter.WriteObject(product);

                        var selected = SelectOneProductAsDataSource(metOutList, formatter);
                        SetEOLToNotUsedProducts(metOutList, selected, formatter);
                        
                        if (string.IsNullOrWhiteSpace(selected.UrlZdjecia) == false)
                        {
                            // TO JEST Tak że jeśli zdjęcie już jest to ustawiamy puste. Jeśli nie ma to zostawiamy to od dostawcy.
                            formatter.WriteLine($"Ustawiam url zdjecia na Empty dla produktu: [{selected.SymbolSAP}], aby uniknąć dodawania jeszcze raz tego samego zdjęcia do bazy danych.");
                            product.UrlZdjecia = string.Empty;
                            selected.UrlZdjecia = string.Empty;
                        }

                        product.ID = selected.ID;

                        if (string.IsNullOrWhiteSpace(selected.NazwaProduktu) == false)
                        {
                            formatter.WriteLine("Przepisuję nazwę z produktu źródłowego pobranego z MET, ponieważ nazwa jest dostępna.");
                            product.NazwaProduktu = selected.NazwaProduktu;
                        }

                        formatter.WriteLine("Produkt po zmianach:");
                        formatter.WriteObject(product);
                        formatter.Flush();

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
        private Product SelectOneProductAsDataSource(ICollection<Product> metOutList, IStringFormatter formatter)
        {
            foreach (var p in metOutList)
            {
                if (p.UrlZdjecia.Length > 0)
                {
                    formatter.WriteLine($"Wybieram produkt: [{p.SymbolSAP}], posiada on URL.");
                    return p;
                }
            }
            
            var product = metOutList.First();

            if (metOutList.Count > 1)
            {
                formatter.WriteLine("Jest wiele produktów o tym samym Symbolu SAP na liście.");
            }

            formatter.WriteLine($"Wybieram produkt: [{product.SymbolSAP}], jest pierwszy na liscie.");

            return product;
        }

        private void SetEOLToNotUsedProducts(IEnumerable<Product> products, Product selectedProduct, IStringFormatter formatter)
        {
            foreach (var p in products)
            {

                // Copying to local variable. It will allow to use [ref]
                var v = p;

                if (!ReferenceEquals(p, selectedProduct))
                {
                    formatter.WriteLine($"Ustawiam kategorę EOL na produkcie: [{v.SymbolSAP}], nie został on wybrany.");
                    EndOfLiveDomain.SetEndOfLife(v);
                }
            }
        }

    }
}
