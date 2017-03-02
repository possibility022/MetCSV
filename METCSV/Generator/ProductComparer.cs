using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace METCSV.Generator
{
    class ProductComparer
    {
        List<ProductGroup> groups;

        List<Product> lamaProducts;
        List<Product> techDataProducts;
        List<Product> METProducts;
        List<Product> abProducts;

        List<Product> FinalList;
        List<Product> listOfHiddenProducts = null;
        List<Product> notAvailable; // Lista produktów których nie ma u głównego dostawcy ale są u innego dostawcy.
                                    // Inaczej: Produkt jest w metowym pliku od dostawcy AB. Ale w nowym pliku AB go nie ma.
                                    // Jest natomiast w tech dacie. Na tej liscie powinien pojawić się ten produkt z AB.
        List<string> partNumbers;
        List<int> partNumbersConfilcts = new List<int>();

        public ProductComparer(List<Product> lamaProducts, List<Product> techDataProducts, List<Product> metProducts, List<Product> abProducts)
        {
            this.lamaProducts = lamaProducts;
            this.techDataProducts = techDataProducts;
            this.METProducts = metProducts;
            this.abProducts = abProducts;
        }

        public void Generate()
        {
            FinalList = new List<Product>();

            removeHiddenProducts();

            AllPartNumberList allPartNumbers = new AllPartNumberList(lamaProducts, techDataProducts, abProducts);

            System.Threading.Thread generateAllPartsNumber = new System.Threading.Thread(() => allPartNumbers.getPartNumbers());
            generateAllPartsNumber.Start();
            //partNumbers = allPartNumbers.getPartNumbers();

            //Przepisuje obrazek, id, nazwe produktu z pliku MET.
            System.Threading.Thread fillLamaList_thread = new System.Threading.Thread(() => fillList(lamaProducts));
            fillLamaList_thread.Start();

            System.Threading.Thread fillTechData_thread = new System.Threading.Thread(() => fillList(techDataProducts));
            fillTechData_thread.Start();

            System.Threading.Thread fillAB_thread = new Thread(() => fillList(abProducts));
            fillAB_thread.Start();

            while (generateAllPartsNumber.IsAlive || fillTechData_thread.IsAlive || fillLamaList_thread.IsAlive || fillAB_thread.IsAlive)
                System.Threading.Thread.Sleep(1000);

            partNumbers = allPartNumbers.getPartNumbers();

            setEndOfLife(); //Przypisujemy End Of Life

            compareAll();

            solveConflicts();

            foreach (int s in partNumbersConfilcts)
                Database.Log.log("Problem With This PartNumber: " + s.ToString());
        }

        public void compareAll()
        {
            //compareFragment(0, partNumbers.Count);

            Thread threadOne = new Thread(() => compareFragment(0, partNumbers.Count / 4));

            Thread threadTwo = new Thread(() => compareFragment((partNumbers.Count / 4) + 1, (partNumbers.Count / 4) * 2));

            Thread threadThree = new Thread(() => compareFragment(((partNumbers.Count / 4) * 2) + 1, (partNumbers.Count / 4) * 3));

            Thread ThreadFour = new Thread(() => compareFragment(((partNumbers.Count / 4) * 3) + 1, partNumbers.Count));

            ThreadController threadController = new ThreadController();
            threadController.addThread(threadOne);
            threadController.addThread(threadTwo);
            threadController.addThread(threadThree);
            threadController.addThread(ThreadFour);

            threadController.startControll();
        }

        public void compareFragment(int start, int end)
        {
            Database.Log.Logging.log_message("Porównywanie fragmentu listy: " + start.ToString() + " " + end.ToString());
            for (int i = start; i < end; i++)
            {
                var productLama = lamaProducts.Where(p => p.KodProducenta == partNumbers[i]);
                var productTechData = techDataProducts.Where(p => p.KodProducenta == partNumbers[i]);
                var productAB = abProducts.Where(p => p.KodProducenta == partNumbers[i]);

                if (productTechData.Count() <= 1 && productLama.Count() <= 1 && productAB.Count() <= 1)
                {
                    List<Product> tmpList = new List<Product>();
                    tmpList.AddRange(productLama);
                    tmpList.AddRange(productTechData);
                    tmpList.AddRange(productAB);
                    selectOneProduct(tmpList);
                }else
                {
                    partNumbersConfilcts.Add(i);
                }
            }
        }

        public void selectOneProduct(List<Product> products)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            removeEmptyWarehouse(products);
            int cheapest = findCheapestProduct(products);

            products[cheapest].StatusProduktu = true;
        }

        private void removeHiddenProducts(List<Product> products)
        {
            Database.Log.log("Usuwam ukryte produkty");
            int countAtBegining = products.Count;

            if (listOfHiddenProducts == null)
                listOfHiddenProducts = METProducts.Where(p => p.Hidden).ToList();

            if (Object.ReferenceEquals(products, METProducts))
            {
                for (int i = 0; i < listOfHiddenProducts.Count; i++)
                    products.Remove(listOfHiddenProducts[i]);
            }

            else

            for (int i = 0; i < products.Count; i++)
            {
                    if (listOfHiddenProducts.Any(p => p.SymbolSAP == products[i].SymbolSAP))
                    {
                        products.RemoveAt(i);
                        i--;
                    }
            }
            Database.Log.log("Usunięto " + (countAtBegining - products.Count).ToString());
        }

        private void removeHiddenProducts()
        {
            Thread t1 = new Thread(() => removeHiddenProducts(lamaProducts));
            Thread t2 = new Thread(() => removeHiddenProducts(techDataProducts));
            Thread t3 = new Thread(() => removeHiddenProducts(abProducts));
            Thread t4 = new Thread(() => removeHiddenProducts(METProducts));

            ThreadController controller = new ThreadController();
            controller.addThread(t1);
            controller.addThread(t2);
            controller.addThread(t3);
            controller.addThread(t4);

            controller.startControll();
        }

        private int findCheapestProduct(List<Product> products)
        {
            int cheapest = 0;
            for (int i = 1; i < products.Count; i++)
            {
                if (products[i].CenaZakupuNetto < products[cheapest].CenaZakupuNetto)
                    cheapest = i;
            }

            return cheapest;
        }

        private void removeEmptyWarehouse(List<Product> products)
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

        private void solveConflicts()
        {
            foreach (int i in partNumbersConfilcts)
            {
                var productLama = lamaProducts.Where(p => p.KodProducenta == partNumbers[i]);
                var productTechData = techDataProducts.Where(p => p.KodProducenta == partNumbers[i]);
                var productAB = abProducts.Where(p => p.KodProducenta == partNumbers[i]);

                List<Product> list = new List<Product>();
                list.AddRange(productLama.ToList());
                list.AddRange(productTechData.ToList());
                list.AddRange(productAB.ToList());

                solveConflictsUsingSapNumber(list);

                if (list.Count < 1)
                    continue;

                Forms.GroupController gc = new Forms.GroupController();
                gc.LoadGroups(FileSystem.Exporter.importGroups(partNumbers[i]), list);
                if (gc.allSolved == false)
                    gc.ShowDialog();
                FileSystem.Exporter.exportGroups(partNumbers[i], gc.getGroups());

                List<ProductGroup> groups =  gc.getGroups();

                for (int a = 0; a < groups.Count; a++)
                {
                    selectOneProduct(groups[a].getList());
                }
            }
        }

        private void solveConflictsUsingSapNumber(List<Product> list)
        {
            List<string> sapNumbers = new List<string>();

            foreach (Product p in list)
                if (sapNumbers.Contains(p.SymbolSAP) == false)
                    sapNumbers.Add(p.SymbolSAP);

            foreach(string sap in sapNumbers)
            {
                List<Product> selected = list.Where(p => p.SymbolSAP == sap).ToList();
                selectOneProduct(selected);

                //for (int i = 0; i < list.Count; i++)
                //{
                //    list.Remove(selected[i]);
                //    i--;
                //}
            }

            list.Clear();
        }

        private Product findProductByProviderNumber(string what, ref List<Product> source)
        {
            return source.Single(p => p.KodProducenta == what);
        }

        private void fillList(List<Product> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                List<Product> products = METProducts.Where(p => p.SymbolSAP == list[i].SymbolSAP).ToList();

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
                        list[i].UrlZdjecia = products[workon].UrlZdjecia;

                    list[i].ID = products[workon].ID;

                    if (products[0].NazwaProduktu != "")
                        list[i].NazwaProduktu = products[workon].NazwaProduktu;
                }
            }
        }

        private void setEndOfLife()
        {
            notAvailable = new List<Product>();

            Thread threadOne = new Thread(() => setEndOfLife_part(
                0,
                METProducts.Count / 4));
            Thread threadTwo = new Thread(() => setEndOfLife_part(
                (METProducts.Count / 4) + 1,
                (METProducts.Count / 4) * 2));
            Thread threadThree = new Thread(() => setEndOfLife_part(
                ((METProducts.Count / 4) * 2) + 1,
                ((METProducts.Count / 4) * 3)));
            Thread threadFour = new Thread(() => setEndOfLife_part(
                (METProducts.Count / 4) * 3 + 1,
                METProducts.Count));

            ThreadController threadController = new ThreadController();
            threadController.addThread(threadOne);
            threadController.addThread(threadTwo);
            threadController.addThread(threadThree);
            threadController.addThread(threadFour);

            threadController.startControll();
        }

        private void setEndOfLife_part(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if ((lamaProducts.Where(p => p.SymbolSAP == METProducts[i].SymbolSAP).Count() == 0)
                    && (techDataProducts.Where(p => p.SymbolSAP == METProducts[i].SymbolSAP).Count() == 0)
                    && (abProducts.Where(p => p.SymbolSAP == METProducts[i].SymbolSAP).Count() == 0) 
                    
                    &&

                    (lamaProducts.Where(p => p.KodProducenta == METProducts[i].KodProducenta).Count() == 0)
                    && (techDataProducts.Where(p => p.KodProducenta == METProducts[i].KodProducenta).Count() == 0)
                    && (abProducts.Where(p => p.KodProducenta == METProducts[i].KodProducenta).Count() == 0)
                    )
                {
                    METProducts[i].Kategoria = "EOL";
                }
                else
                {
                    try
                    {
                        notAvailable.Add(METProducts[i]);
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }

        public List<Product> combineLists()
        {
            List<Product> combinedList = new List<Product>();
            combinedList.AddRange(lamaProducts);
            combinedList.AddRange(techDataProducts);
            combinedList.AddRange(abProducts);

            var endOfLife = METProducts.Where(p => p.Kategoria == "EOL");
            combinedList.AddRange(endOfLife);

            return combinedList;
        }

        public void SetPricesByProducent(List<Product> products, string producent, double price)
        {
            var query = products.Where(p => p.NazwaProducenta == producent);
            foreach (var product in query)
            {
                product.CenaNetto += (product.CenaNetto * price * 0.01); // Ustawienie marży
            }
        }
        public void SetPricesByCategory(List<Product> products, string category, double price)
        {
            var query = products.Where(p => p.Kategoria == category);
            foreach (var product in query)
            {
                product.CenaNetto += (product.CenaNetto * price * 0.01);
            }
        }
        public void SetPricesBySupplier(List<Product> products, string supplier, double price)
        {
            var query = products.Where(p => p.NazwaDostawcy == supplier);
            foreach (var product in query)
            {
                product.CenaNetto += (product.CenaNetto * price * 0.01);
            }
        }
    }
}
