using METCSV.Common;
using METCSV.WPF.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace METCSV.WPF.Engine
{
    class ProductMerger
    {
        private ConcurrentDictionary<string, Product> _hiddenMetProducts;

        List<Product> _finalList;

        ConcurrentDictionary<string, byte> _allPartNumbers = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        ConcurrentBag<Product> _metBag;

        ConcurrentBag<Product> _lamaProducts;
        ConcurrentBag<Product> _techDataProducts;
        ConcurrentBag<Product> _abProducts;

        ConcurrentDictionary<string, Product> _lamaFilled;
        ConcurrentDictionary<string, Product> _techDataFilled;
        ConcurrentDictionary<string, Product> _abFilled;

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(IEnumerable<Product> met, IEnumerable<Product> lama, IEnumerable<Product> td, IEnumerable<Product> ab)
        {
            _metBag = new ConcurrentBag<Product>(met);
            _lamaProducts = new ConcurrentBag<Product>(lama);
            _techDataProducts = new ConcurrentBag<Product>(td);
            _abProducts = new ConcurrentBag<Product>(ab);
        }

        public void Generate()
        {
            _finalList = new List<Product>();
            _hiddenMetProducts = new ConcurrentDictionary<string, Product>();

            RemoveHiddenProducts();

            _allPartNumbers = AllPartNumbersDomain.GetAllPartNumbers(_metBag, _lamaProducts, _techDataProducts, _abProducts);

            FillLists();
            SetEndOfLife();
            CompareAll();
            //SolveConflicts();
            _finalList = CombineList();
        }

        #region RemovingHiddenProducts

        private void RemoveHiddenProducts()
        {
            var hiddenEngine = new HiddenProductsDomain();

            _hiddenMetProducts = hiddenEngine.CreateListOfHiddenProducts(_metBag);

            _metBag = hiddenEngine.RemoveHiddenProducts(_metBag);
            _lamaProducts = hiddenEngine.RemoveHiddenProducts(_lamaProducts);
            _techDataProducts = hiddenEngine.RemoveHiddenProducts(_techDataProducts);
            _abProducts = hiddenEngine.RemoveHiddenProducts(_abProducts);
        }

        #endregion
        

        #region FillList

        private void FillLists()
        {
            _lamaFilled = FillList(_lamaProducts);
            _techDataFilled = FillList(_techDataProducts);
            _abFilled = FillList(_abProducts);
        }

        private ConcurrentDictionary<string, Product> FillList(ConcurrentBag<Product> products)
        {
            ConcurrentDictionary<string, Product> newList = new ConcurrentDictionary<string, Product>();
            Task[] tasks = new Task[Environment.ProcessorCount];

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
                List<Product> products = _metBag.Where(m => m.SymbolSAP == product.SymbolSAP).ToList(); //todo can be optimized

                int workon = 0;
                if (products.Count >= 2)        //To jest tak że produkty w pliku METCSV się powtarzają. I wybierany jest ten gdzie jest URL
                {
                    for (int metProductIndex = 0; metProductIndex < products.Count; metProductIndex++)
                    {
                        if (products[metProductIndex].UrlZdjecia.Length > 0)
                        {
                            workon = metProductIndex;

                            for (int setEOL = 0; setEOL < products.Count; setEOL++)
                            {
                                if (setEOL == workon)
                                    continue;

                                products[setEOL].Kategoria = "EOL";
                                products[setEOL].UrlZdjecia = "";
                            }

                            break;
                        }
                    }
                    //throw new Exception("Znaleziono dwa takie same produkty w pliku MET od tego samego dostawcy.");
                }

                if (products.Count == 1)
                {
                    workon = 0;
                }

                if (products.Count > 0)
                {
                    if (products[workon].UrlZdjecia.Length > 0)
                        //list[i].UrlZdjecia = products[workon].UrlZdjecia;
                        product.UrlZdjecia = ""; // TO JEST Tak że jeśli zdjęcie już jest to ustawiamy puste. Jeśli nie ma to zostawiamy to od dostawcy.

                    product.ID = products[workon].ID;

                    if (products[0].NazwaProduktu != "")
                        product.NazwaProduktu = products[workon].NazwaProduktu;
                }

                var success = newList.TryAdd(product.KodProducenta, product);
                if (success == false)
                {
                    _partNumbersConfilcts.TryAdd(product.SymbolSAP, product);
                }

                product = null;
            }
        }

        #endregion

        #region EndOfLife

        private void SetEndOfLife()
        {

            Task<Tuple<HashSet<string>, HashSet<string>>>[] tasks = new Task<Tuple<HashSet<string>, HashSet<string>>>[3];

            tasks[0] = new Task<Tuple<HashSet<string>, HashSet<string>>>(() => CreateSapHashset(_lamaFilled));
            tasks[1] = new Task<Tuple<HashSet<string>, HashSet<string>>>(() => CreateSapHashset(_abFilled));
            tasks[2] = new Task<Tuple<HashSet<string>, HashSet<string>>>(() => CreateSapHashset(_techDataFilled));

            tasks.StartAll();
            tasks.WaitAll();

            Tuple<HashSet<string>, HashSet<string>> lamaPair = tasks[0].Result;
            Tuple<HashSet<string>, HashSet<string>> abPair = tasks[1].Result;
            Tuple<HashSet<string>, HashSet<string>> tdPair = tasks[2].Result;

            HashSet<string> lamaSAP = lamaPair.Item1;
            HashSet<string> abSAP = abPair.Item1;
            HashSet<string> tdSAP = tdPair.Item1;

            HashSet<string> lamaKodProducenta = lamaPair.Item2;
            HashSet<string> abKodProducenta = abPair.Item2;
            HashSet<string> tdKodProducenta = tdPair.Item2;

            int i = 0;

            foreach (var prod in _metBag)
            {

                if (lamaSAP.Contains(prod.SymbolSAP) == false
                    && tdSAP.Contains(prod.SymbolSAP) == false
                    && abSAP.Contains(prod.SymbolSAP) == false
                    && lamaKodProducenta.Contains(prod.KodProducenta) == false
                    && tdKodProducenta.Contains(prod.KodProducenta) == false
                    && abKodProducenta.Contains(prod.KodProducenta) == false)
                {
                    prod.Kategoria = "EOL"; //todo move to config
                    i++;
                }
                else
                {
                    Debug.WriteLine("Not EOL");
                }
            }
        }

        private Tuple<HashSet<string>, HashSet<string>> CreateSapHashset(ConcurrentDictionary<string, Product> products)
        {
            var sapNumbers = new HashSet<string>();
            var kodProducents = new HashSet<string>();

            foreach (var p in products.Values)
            {
                sapNumbers.Add(p.SymbolSAP);
                kodProducents.Add(p.KodProducenta);
            }

            return new Tuple<HashSet<string>, HashSet<string>>(sapNumbers, kodProducents);
        }


        #endregion

        #region Compare

        private void CompareAll()
        {
            Task[] tasks = new Task[Environment.ProcessorCount];
            ConcurrentBag<string> allPartNumbers = new ConcurrentBag<string>(_allPartNumbers.Keys);

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => Compare(allPartNumbers, _lamaFilled, _techDataFilled, _abFilled));
            }

            tasks.StartAll();
            tasks.WaitAll();
        }

        private void Compare(
            ConcurrentBag<string> allPartNumbers,
            ConcurrentDictionary<string, Product> lamaProducts,
            ConcurrentDictionary<string, Product> tdProducts,
            ConcurrentDictionary<string, Product> abProducts
            )
        {
            string kodProducenta = null;

            while (allPartNumbers.TryTake(out kodProducenta) || allPartNumbers.Count > 0)
            {
                List<Product> products = new List<Product>();

                Product p = null;

                if (lamaProducts.TryGetValue(kodProducenta, out p))
                {
                    products.Add(p);
                }

                if (abProducts.TryGetValue(kodProducenta, out p))
                {
                    products.Add(p);
                }

                if (tdProducts.TryGetValue(kodProducenta, out p))
                {
                    products.Add(p);
                }

                SelectOneProduct(products);
            }
        }

        private void SelectOneProduct(List<Product> products)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            RemoveEmptyWarehouse(products);
            int cheapest = FindCheapestProduct(products);

            if (products[cheapest].ID != null)
                products[cheapest].StatusProduktu = true;
        }

        #endregion

        private void RemoveEmptyWarehouse(List<Product> products)
        {
            int empty = 0;
            for (int i = 0; i < products.Count; i++)
                if (products[i].StanMagazynowy <= 0)
                    empty++;

            if (empty == products.Count)
                return;
            else
                for (int i = 0; i < products.Count; i++)
                    if (products[i].StanMagazynowy <= 0)
                    {
                        products.RemoveAt(i);
                        i--;
                    }
        }

        private int FindCheapestProduct(List<Product> products)
        {
            int cheapest = 0;
            for (int i = 1; i < products.Count; i++)
            {
                if (products[i].CenaZakupuNetto < products[cheapest].CenaZakupuNetto)
                    cheapest = i;
            }

            return cheapest;
        }

        //private void SolveConflicts()
        //{
        //foreach (string partNumber in _partNumbersConfilcts)
        //{
        //    var productLama = _lamaProducts.Where(p => p.KodProducenta == partNumber);
        //    var productTechData = _techDataProducts.Where(p => p.KodProducenta == partNumber);
        //    var productAB = _abProducts.Where(p => p.KodProducenta == partNumber);

        //    List<Product> list = new List<Product>();
        //    list.AddRange(productLama.ToList());
        //    list.AddRange(productTechData.ToList());
        //    list.AddRange(productAB.ToList());

        //    SolveConflictsUsingSapNumber(list);

        //    if (list.Count < 1)
        //        continue;

        //throw new NotImplementedException();

        //Forms.GroupController gc = new Forms.GroupController();
        //gc.LoadGroups(FileSystem.Exporter.importGroups(partNumbers[partNumber]), list);
        //if (gc.allSolved == false)
        //    gc.ShowDialog();
        //FileSystem.Exporter.exportGroups(partNumbers[partNumber], gc.getGroups());

        //List<ProductGroup> groups = gc.getGroups();

        //for (int a = 0; a < groups.Count; a++)
        //{
        //    selectOneProduct(groups[a].getList());
        //}
        //  }
        //}

        //private void SolveConflictsUsingSapNumber(List<Product> list)
        //{
        //    List<string> sapNumbers = new List<string>();

        //    foreach (Product p in list)
        //        if (sapNumbers.Contains(p.SymbolSAP) == false)
        //            sapNumbers.Add(p.SymbolSAP);

        //    foreach (string sap in sapNumbers)
        //    {
        //        List<Product> selected = list.Where(p => p.SymbolSAP == sap).ToList();
        //        SelectOneProduct(selected);
        //    }

        //    list.Clear(); // TODO debug. Jeśli czyści to forma do pokazywania konfliktów nigdy nie zostanie pokazana.
        //}

        private List<Product> CombineList()
        {
            List<Product> combinedList = new List<Product>();
            combinedList.AddRange(_lamaProducts);
            combinedList.AddRange(_techDataProducts);
            combinedList.AddRange(_abProducts);

            var endOfLife = _metBag.Where(p => p.Kategoria == "EOL");
            combinedList.AddRange(endOfLife);

            return combinedList;
        }
    }
}