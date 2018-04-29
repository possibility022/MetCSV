using METCSV.Common;
using METCSV.WPF.Converters;
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

        ConcurrentDictionary<int, byte> _allPartNumbers = new ConcurrentDictionary<int, byte>();
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        ConcurrentBag<Product> _metBag;

        ConcurrentBag<Product> _lamaProducts;
        ConcurrentBag<Product> _techDataProducts;
        ConcurrentBag<Product> _abProducts;

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

            // STEP 1
            RemoveHiddenProducts();

            // STEP 2
            _allPartNumbers = AllPartNumbersDomain.GetAllPartNumbers(_metBag, _lamaProducts, _techDataProducts, _abProducts);

            // STEP 3
            var fillList = new FillListDomain(_metBag);
            _lamaProducts = fillList.FillList(_lamaProducts);
            _abProducts = fillList.FillList(_abProducts);
            _techDataProducts = fillList.FillList(_techDataProducts);

            // STEP4
            var setEndOfLive = new EndOfLiveDomain(_metBag, _lamaProducts, _abProducts, _techDataProducts);
            setEndOfLive.SetEndOfLife();

            CompareAll();
            //SolveConflicts();
            _finalList = CombineList();
        }

        private void RemoveHiddenProducts()
        {
            var hiddenEngine = new HiddenProductsDomain();

            _hiddenMetProducts = hiddenEngine.CreateListOfHiddenProducts(_metBag);

            _metBag = hiddenEngine.RemoveHiddenProducts(_metBag);
            _lamaProducts = hiddenEngine.RemoveHiddenProducts(_lamaProducts);
            _techDataProducts = hiddenEngine.RemoveHiddenProducts(_techDataProducts);
            _abProducts = hiddenEngine.RemoveHiddenProducts(_abProducts);
        }

        
        #region Compare

        private void CompareAll()
        {
            Task[] tasks = new Task[Environment.ProcessorCount];
            ConcurrentBag<string> allPartNumbers = new ConcurrentBag<string>(_allPartNumbers.Keys);

            for (int i = 0; i < tasks.Length; i++)
            {
                //tasks[i] = new Task(() => Compare(allPartNumbers, _lamaFilled, _techDataFilled, _abFilled));
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