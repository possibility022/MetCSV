using METCSV.WPF.ExtensionMethods;
using METCSV.WPF.ProductProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Engine
{
    class ProductMerger
    {
        private List<Product> _hiddenProducts;

        List<Product> _finalList;

        HashSet<string> _allPartNumbers = new HashSet<string>();
        HashSet<string> _partNumbersConfilcts = new HashSet<string>();

        List<Product> _metProducts;
        List<Product> _lamaProducts;
        List<Product> _techDataProducts;
        List<Product> _abProducts;

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(List<Product> met, List<Product> lama, List<Product> td, List<Product> ab)
        {
            _metProducts = met;
            _lamaProducts = lama;
            _techDataProducts = td;
            _abProducts = ab;
        }

        public void Generate()
        {
            _finalList = new List<Product>();

            RemoveHiddenProducts();

            GetAllPartNumbers(_metProducts);
            GetAllPartNumbers(_lamaProducts);
            GetAllPartNumbers(_techDataProducts);
            GetAllPartNumbers(_abProducts);
            
            FillLists();
            SetEndOfLife();
            CompareAll();
            SolveConflicts();
            _finalList = CombineList();
        }

        private void RemoveHiddenProducts()
        {
            CreateListOfHiddenProducts();

            Task[] tasks = new Task[4];

            tasks[0] = new Task(() => RemoveHiddenProducts(_lamaProducts));
            tasks[1] = new Task(() => RemoveHiddenProducts(_techDataProducts));
            tasks[2] = new Task(() => RemoveHiddenProducts(_abProducts));
            tasks[3] = new Task(() => RemoveHiddenProducts(_metProducts));

            tasks.StartAll();

            Task.WaitAll(tasks);
        }

        private void RemoveHiddenProducts(List<Product> products)
        {
            //Database.Log.log("Usuwam ukryte produkty"); ToDo write to log
            int countAtBegining = products.Count;

            if (ReferenceEquals(products, _metProducts))
            {
                for (int i = 0; i < _hiddenProducts.Count; i++)
                    products.Remove(_hiddenProducts[i]);
            }

            else

                for (int i = 0; i < products.Count; i++)
                {
                    if (_hiddenProducts.Any(p => p.SymbolSAP == products[i].SymbolSAP))
                    {
                        products.RemoveAt(i);
                        i--;
                    }
                }
            //Database.Log.log("Usunięto " + (countAtBegining - products.Count).ToString()); //todo LOG
        }

        private void CreateListOfHiddenProducts()
        {
            foreach (var p in _metProducts)
            {
                if (p.Hidden)
                {
                    _hiddenProducts.Add(p);
                }
            }
        }

        private void GetAllPartNumbers(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                _allPartNumbers.Add(product.KodProducenta);
            }
        }

        private void FillLists()
        {
            Task[] tasks = new Task[3];

            tasks[0] = new Task(() => FillList(_lamaProducts));
            tasks[1] = new Task(() => FillList(_techDataProducts));
            tasks[2] = new Task(() => FillList(_abProducts));
            tasks.StartAll();
        }

        private void FillList(List<Product> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                List<Product> products = _metProducts.Where(p => p.SymbolSAP == list[i].SymbolSAP).ToList(); //todo can be optimized

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
                        list[i].UrlZdjecia = ""; // TO JEST Tak że jeśli zdjęcie już jest to ustawiamy puste. Jeśli nie ma to zostawiamy to od dostawcy.

                    list[i].ID = products[workon].ID;

                    if (products[0].NazwaProduktu != "")
                        list[i].NazwaProduktu = products[workon].NazwaProduktu;
                }
            }
        }

        private void SetEndOfLife()
        {
            Task[] tasks = new Task[4];

            tasks[0] = new Task(() => SetEndOfLife_part(
                0,
                _metProducts.Count / 4));
            tasks[1] = new Task(() => SetEndOfLife_part(
                (_metProducts.Count / 4) + 1,
                (_metProducts.Count / 4) * 2));
            tasks[2] = new Task(() => SetEndOfLife_part(
                ((_metProducts.Count / 4) * 2) + 1,
                ((_metProducts.Count / 4) * 3)));
            tasks[3] = new Task(() => SetEndOfLife_part(
                (_metProducts.Count / 4) * 3 + 1,
                _metProducts.Count));

            tasks.StartAll();

            Task.WaitAll(tasks);
        }

        private void SetEndOfLife_part(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if ((_lamaProducts.Where(p => p.SymbolSAP == _metProducts[i].SymbolSAP).Count() == 0)
                    && (_techDataProducts.Where(p => p.SymbolSAP == _metProducts[i].SymbolSAP).Count() == 0)
                    && (_abProducts.Where(p => p.SymbolSAP == _metProducts[i].SymbolSAP).Count() == 0)

                    &&

                    (_lamaProducts.Where(p => p.KodProducenta == _metProducts[i].KodProducenta).Count() == 0)
                    && (_techDataProducts.Where(p => p.KodProducenta == _metProducts[i].KodProducenta).Count() == 0)
                    && (_abProducts.Where(p => p.KodProducenta == _metProducts[i].KodProducenta).Count() == 0)
                    )
                {
                    _metProducts[i].Kategoria = "EOL";
                }
            }
        }

        private void CompareAll()
        {
            //compareFragment(0, partNumbers.Count);
            List<string> allPartNumbers = _allPartNumbers.ToList();

            Task[] tasks = new Task[4];

            tasks[0] = new Task(() => CompareFragment(0, allPartNumbers.Count / 4, allPartNumbers));
            tasks[1] = new Task(() => CompareFragment((allPartNumbers.Count / 4) + 1, (allPartNumbers.Count / 4) * 2, allPartNumbers));
            tasks[2] = new Task(() => CompareFragment(((allPartNumbers.Count / 4) * 2) + 1, (allPartNumbers.Count / 4) * 3, allPartNumbers));
            tasks[3] = new Task(() => CompareFragment(((allPartNumbers.Count / 4) * 3) + 1, allPartNumbers.Count, allPartNumbers));

            tasks.StartAll();

            Task.WaitAll(tasks);
        }

        private void CompareFragment(int start, int end, List<string> allPartNumbers)
        {
            //Database.Log.Logging.log_message("Porównywanie fragmentu listy: " + start.ToString() + " " + end.ToString()); //TODO log it
            for (int i = start; i < end; i++)
            {
                var productLama = _lamaProducts.Where(p => p.KodProducenta == allPartNumbers[i]);
                var productTechData = _techDataProducts.Where(p => p.KodProducenta == allPartNumbers[i]);
                var productAB = _abProducts.Where(p => p.KodProducenta == allPartNumbers[i]);

                if (productTechData.Count() <= 1 && productLama.Count() <= 1 && productAB.Count() <= 1)
                {
                    List<Product> tmpList = new List<Product>();
                    tmpList.AddRange(productLama);
                    tmpList.AddRange(productTechData);
                    tmpList.AddRange(productAB);
                    SelectOneProduct(tmpList);
                }
                else
                {
                    _partNumbersConfilcts.Add(allPartNumbers[i]);
                }
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

        private void SolveConflicts()
        {
            foreach (string partNumber in _partNumbersConfilcts)
            {
                var productLama = _lamaProducts.Where(p => p.KodProducenta == partNumber);
                var productTechData = _techDataProducts.Where(p => p.KodProducenta == partNumber);
                var productAB = _abProducts.Where(p => p.KodProducenta == partNumber);

                List<Product> list = new List<Product>();
                list.AddRange(productLama.ToList());
                list.AddRange(productTechData.ToList());
                list.AddRange(productAB.ToList());

                SolveConflictsUsingSapNumber(list);

                if (list.Count < 1)
                    continue;

                throw new NotImplementedException();

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
            }
        }

        private void SolveConflictsUsingSapNumber(List<Product> list)
        {
            List<string> sapNumbers = new List<string>();

            foreach (Product p in list)
                if (sapNumbers.Contains(p.SymbolSAP) == false)
                    sapNumbers.Add(p.SymbolSAP);

            foreach (string sap in sapNumbers)
            {
                List<Product> selected = list.Where(p => p.SymbolSAP == sap).ToList();
                SelectOneProduct(selected);
            }

            list.Clear(); // TODO debug. Jeśli czyści to forma do pokazywania konfliktów nigdy nie zostanie pokazana.
        }

        private List<Product> CombineList()
        {
            List<Product> combinedList = new List<Product>();
            combinedList.AddRange(_lamaProducts);
            combinedList.AddRange(_techDataProducts);
            combinedList.AddRange(_abProducts);

            var endOfLife = _metProducts.Where(p => p.Kategoria == "EOL");
            combinedList.AddRange(endOfLife);

            return combinedList;
        }
    }
}