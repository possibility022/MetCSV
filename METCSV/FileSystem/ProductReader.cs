using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using METCSV.Database;

namespace METCSV.FileSystem
{
    class ProductsReader
    {

        public List<Product> lamaFullList = new List<Product>();
        public List<Product> techdataFullList = new List<Product>();
        public List<Product> metFullList = new List<Product>();
        public List<Product> abFullList = new List<Product>();

        private object _lock = new object();

        private Global.Result _lamaLoadResult = Global.Result.readyToStart;
        private Global.Result _techDataLoadResult = Global.Result.readyToStart;
        private Global.Result _metLoadResult = Global.Result.readyToStart;
        private Global.Result _abLoadResult = Global.Result.readyToStart;

        public Global.Result lamaLoadResult
        {
            get
            {
                Global.Result value;
                lock (_lock)
                {
                    value = _lamaLoadResult;
                }
                return value;
            }
            private set
            {
                lock (_lock)
                {
                    _lamaLoadResult = value;
                }
            }
        }
        public Global.Result techDataLoadResult
        {
            get
            {
                Global.Result value;
                lock (_lock)
                {
                    value = _techDataLoadResult;
                }
                return value;
            }
            private set
            {
                lock (_lock)
                {
                    _techDataLoadResult = value;
                }
            }
        }
        public Global.Result metLoadResult
        {
            get
            {
                Global.Result value;
                lock (_lock)
                {
                    value = _metLoadResult;
                }
                return value;
            }
            private set
            {
                lock (_lock)
                {
                    _metLoadResult = value;
                }
            }
        }
        public Global.Result abLoadResult
        {
            get
            {
                Global.Result value;
                lock (_lock)
                {
                    value = _abLoadResult;
                }
                return value;
            }
            private set
            {
                lock (_lock)
                {
                    _abLoadResult = value;
                }
            }
        }


        //Mapowanie 
        enum Lama
        {
            Lama,// Nazwa dostawcy
            KOD, //KOD
            PN, //KodProducenta i ModelProduktu
            NAZEV,//NazwaProduktu
            SKLAD_NUM,//StanMagazynowy
            CENA, //CenaZakupuNetto
            SNIMEK // UrlDoZdjecia
        }
        #region LamaExtendedInfo
        readonly string[] lama = new string[]
{
            "nagłówek1",
            "nagłówek2",
            "nagłówek3",
            "producent",
            "nr kat.",
            "EAN",
            "nazwa",
            "OEM",
            "Twoja cena",
            "Opis",
            "Gwarancja",
            "PHE",
            "Magazyn",
            "nowość",
            "EU",
            "Opakowanie",
            "producent",
            "url obrazek",
            "Magazyn",
            "Magazyn_limit"
};

        public string[] Lama1
        {
            get
            {
                return lama;
            }
        }
        #endregion

        enum AB
        {
            indeks,
            indeks_p,
            nazwa,
            producent,
            magazyn_stan,
            cena_netto,
            cena_brutto,
            cena_brutto_plus_marza,
            kategoria,
            magazyn_ilosc,
            EAN,
            ID_produktu
        }



        enum TechData
        {
            SapNo = 0,
            PartNo,
            Vendor,
            Nazwa,
            Czas,
            Magazyn,
            DataDostawy,
            EC,
            URL,
            Rodzina,
            WagaBrutto,
            EAN,
            FamilyPr_kod, // Kategoria
            FamilyPr_PL,
            KlasaPr_kod,
            KlasaPr_PL,
            PodklasaPr_kod,
            PodklasaPr_PL,
            NazwaEC,
            KIT,
            Wielkogabaryt,
            Sredniogabaryt
        }
        enum TechPrice
        {
            SapNo,
            Cena,
            Waluta,

        }
        enum Met
        {
            ID = 0,
            NazwaProduktu,
            AdresURLzdjecia,
            NazwaProducenta,
            ModelProduktu,
            StatusProduktu,
            Kategoria,
            KodProducenta,
            KodUDostawcy,
            SymbolSAP

        }

        delegate void OperationType(string[] fields, List<Product> products);

        public List<Product> GetABProducts(string csvPath, System.Text.Encoding encoding)
        {
            abLoadResult = Global.Result.inProgress;
            var products = new List<Product>();
            try
            {
                Database.Log.Logging.log_message("Wczytuję produkty z AB");
                if (File.Exists(csvPath))
                {
                    products = ReadCSV(OperationGetProductsAB, csvPath, encoding, 2);
                }
                else
                {
                    throw new FileNotFoundException("Nie znaleziono pliku csv. " + csvPath);
                }
                abFullList = products;
                Database.Log.Logging.log_message("Produkty z AB wczytane");
                abLoadResult = Global.Result.complete;
            }
            catch (Exception ex)
            { Log.Logging.LogException(ex); abLoadResult = Global.Result.faild; }

            return products;
        }

        //Funkcje publiczne: można się bawić
        /// <summary>
        /// Zwraca wszystkie produktu z Lamy,
        /// </summary>
        /// <param name="pathXML">Ścieżka do pliku xml z produktami</param>
        /// <param name="pathCSV">Ścieżka do pliku csv z producentami</param>
        /// <returns>  lista produktów z lamy, jeśli podano prawidłową ścieżkę, w przeciwnym razie null</returns>
        public List<Product> GetLamaProducts(string pathXML, string pathCSV)
        {
            lamaLoadResult = Global.Result.inProgress;
            if (File.Exists(pathXML) && File.Exists(pathCSV))
            {
                Database.Log.Logging.log_message("Wczytuję produkty z Lamy");
                var products = ReadLama(pathXML); // wczytanie produktów z XML
                var producents = ReadCSV(OperationGetProducentsLAMA, pathCSV, System.Text.Encoding.Default); // wczytanie producentów produktów

                var merged = MergeProductLama(products, producents); // scalenie plików

                this.lamaFullList = merged;
                Database.Log.Logging.log_message("Produkty z Lamy wczytane");

                lamaLoadResult = Global.Result.complete;
                return merged;
            }
            else
            {
                string message = "Problem z wczytywaniem lamy: Któryś z podanych plików nie istnieje. Czy nie zapomniałeś o rozszerzeniu pliku w nazwie?";
                lamaLoadResult = Global.Result.faild;
                Database.Log.log(message);
                throw new FileNotFoundException(message);
            }
        }

        /// <summary>
        /// Zwraca wszystkie produkty z TechData
        /// </summary>
        /// <param name="pathProducts"> Ścieżka do pliku csv z produktami</param>
        /// <param name="pathPrices"> Ścieżka do pliku csv z cenami produktów</param>
        /// <returns></returns>
        public List<Product> GetTechDataProducts(string pathProducts, string pathPrices)
        {
            techDataLoadResult = Global.Result.inProgress;
            if (File.Exists(pathProducts) && File.Exists(pathPrices))
            {
                Database.Log.Logging.log_message("Wczytuję produkty z TechDaty");
                var prices = ReadCSV(OperationGetPriceTD, pathPrices, System.Text.Encoding.Default);
                var products = ReadCSV(OperationGetProductsTD, pathProducts, System.Text.Encoding.Default);

                var merged = MergePriceTechData(products, prices);

                this.techdataFullList = merged;
                Database.Log.Logging.log_message("Produkty z techdaty wczytane");

                techDataLoadResult = Global.Result.complete;
                return merged;
            }
            else
            {
                string message = "Nie znaleziono jednego z plikow. " + pathPrices + " " + pathProducts;
                techDataLoadResult = Global.Result.faild;
                Database.Log.log(message);
                throw new FileNotFoundException(message);
            }

        }

        public List<Product> GetMetProducts(string pathProducts)
        {
            metLoadResult = Global.Result.inProgress;
            if (File.Exists(pathProducts))
            {
                Database.Log.Logging.log_message("Wczytuję produkty z Metu");
                var products = ReadCSV(OperationGetProductsMET, pathProducts, System.Text.Encoding.GetEncoding("windows-1250"));
                this.metFullList = products;
                Database.Log.Logging.log_message("Produkty z metu wczytane");
                metLoadResult = Global.Result.complete;
                return products;
            }
            else
            {
                metLoadResult = Global.Result.faild;
                string message = "Problem z wczytywaniem pliku MET: Nie znaleziono pliku - " + pathProducts;
                Database.Log.log(message);
                throw new FileNotFoundException(message);
            }
        }

        /// <summary>
        /// Wyświetla wszystkie produkty
        /// </summary>
        /// <param name="products">Lista produktów do wyświetlenia</param>
        public void DisplayProducts(List<Product> products)
        {
            foreach (var product in products)
            {
                DisplayProduct(product);
            }
        }
        public void DisplayProduct(Product product)
        {
            Console.WriteLine("ID: {0}", product.ID);
            Console.WriteLine("Symbol SAP: {0}", product.SymbolSAP);
            Console.WriteLine("Kod producent: {0}", product.KodProducenta);
            Console.WriteLine("Model produktu: {0}", product.KodProducenta);
            Console.WriteLine("Kod u dostawcy: {0}", product.KodDostawcy);
            Console.WriteLine("Nazwa produktu: {0}", product.NazwaProduktu);
            Console.WriteLine("Nazwa producenta: {0}", product.NazwaProducenta);
            Console.WriteLine("Nazwa dostawcy: {0}", product.NazwaDostawcy);
            Console.WriteLine("Stan magazynowy: {0}", product.StanMagazynowy);
            Console.WriteLine("Status produktu: {0}", product.StatusProduktu);
            Console.WriteLine("Cena netto: {0}", product.CenaNetto);
            Console.WriteLine("Cena zakupu netto: {0}", product.CenaZakupuNetto);
            Console.WriteLine("Url do zdjęcia: {0}", product.UrlZdjecia);
            Console.WriteLine("Kategoria: {0}", product.Kategoria);
            Console.WriteLine("");
        }

        //Funkcje wewntrze: jeśli nic się nie sypię, to nie tykać



        /// <summary>
        /// Wczytuje wszystkie dostępne dane z podstawowego pliku lamy, bez nazw producentów.
        /// Jak nie ma kodu procudenta albo part number to nie bierze pod uwagę takich produktów
        /// </summary>
        /// <param name="path"> Ściężka do pliku z produktami lamy</param>
        /// <returns> Liste produktów</returns>
        private List<Product> ReadLama(string path)
        {
            XDeclaration x = new XDeclaration("1.0", "ISO-8859-2", "");
            System.Xml.XmlReader xmlReader;

            StreamReader streamReader = new StreamReader(path, System.Text.Encoding.GetEncoding("ISO-8859-2"));
            xmlReader = System.Xml.XmlReader.Create(streamReader);

            XDocument.Load(xmlReader);

            List<Product> products = new List<Product>();
            var xml = XDocument.Load(path);


            foreach (var product in xml.Root.Elements())
            {
                var urls = GetUrlsLama(product); // wszystkie Urle danego produktu

                if (product.Element("KOD").Value == "" || product.Element("PN").Value == "")
                {
                    continue;
                }


                products.Add(new Product
                {
                    ID = null,
                    SymbolSAP = "LAMA" + HttpUtility.HtmlDecode(product.Element("KOD").Value),
                    //KodProducenta = HttpUtility.HtmlDecode(product.Element("PN").Value),
                    //ModelProduktu = HttpUtility.HtmlDecode(product.Element("PN").Value),
                    OryginalnyKodProducenta = HttpUtility.HtmlDecode(product.Element("PN").Value),
                    KodDostawcy = HttpUtility.HtmlDecode(product.Element("KOD").Value),
                    NazwaProduktu = HttpUtility.HtmlDecode(product.Element("NAZEV").Value),
                    NazwaProducenta = null,
                    NazwaDostawcy = Lama.Lama.ToString(),
                    StanMagazynowy = Int32.Parse(product.Element("SKLAD_NUM").Value),
                    StatusProduktu = false,
                    CenaNetto = 0,
                    CenaZakupuNetto = Double.Parse(product.Element("CENA").Value),
                    UrlZdjecia = urls.Count > 0 ? urls[0] : null
                });
            }

            return products;
        }

        /// <summary>
        /// Wczytuje kolejno linie z plików CSV. W zależności od 
        /// podanego w parametrze delegata, wykonuje na nich różne operacje
        /// </summary>
        /// <param name="operation"> Odpowiada z typ operacji jaki można zrobić na CSV, w zależności od dostawcy</param>
        /// <param name="path">Ściężka do pliku CSV</param>
        /// <returns> Liste produktów</returns>
        private List<Product> ReadCSV(OperationType operation, string path, System.Text.Encoding encoding, int linePassCount = 1)
        {
            var products = new List<Product>();

            using (TextFieldParser parser = new TextFieldParser(path, encoding))
            {
                parser.SetDelimiters(";");
                parser.TextFieldType = FieldType.Delimited;

                for (int i = 0; i < linePassCount; i++)
                    parser.ReadLine(); // Pomiń pierwszą linie
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    operation(fields, products);
                }
            }
            return products;
        }

        /// <summary>
        /// Wczytuje wszystkie Url-e z xml Lamy i zwraca w pozstaci listy
        /// </summary>
        /// <param name="product"> </param>
        /// <returns>Lista urlów w postaci stringoów albo null jeśli nie ma</returns>
        private List<string> GetUrlsLama(XElement product)
        {
            List<string> listUrls = new List<string>();

            if (product.Element("SNIMEK") != null) // sprawdz czy isnieje url, jeśli tak to zapisz wszystkie Url-e w liście i zwróć 
            {
                var rootURL = product.Elements("SNIMEK");

                foreach (var url in rootURL)
                {
                    listUrls.Add(url.Element("URL").Value);
                }

            }
            return listUrls;
        }

        /// <summary>
        /// Wczytuje produkty z TechData z głównego pliku CSV
        /// </summary>
        /// <param name="fields"> Lista wczytanych pól z pliku CSV</param>
        /// <param name="products"> Lista do której ma dodać nowy produkt</param>
        private void OperationGetProductsTD(string[] fields, List<Product> products)
        {
            products.Add(new Product()
            {
                ID = null,
                SymbolSAP = "TechData" + fields[(int)TechData.SapNo],
                //KodProducenta = fields[(int)TechData.PartNo],
                //ModelProduktu = fields[(int)TechData.PartNo],
                OryginalnyKodProducenta = fields[(int)TechData.PartNo],
                NazwaProduktu = fields[(int)TechData.Nazwa],
                NazwaProducenta = fields[(int)TechData.Vendor],
                KodDostawcy = fields[(int)TechData.SapNo],
                NazwaDostawcy = "TechData",
                StanMagazynowy = Int32.Parse(fields[(int)TechData.Magazyn]),
                StatusProduktu = false,
                CenaNetto = -1,
                CenaZakupuNetto = -1,
                UrlZdjecia = null,
                Kategoria = fields[(int)TechData.FamilyPr_kod]

            });
        }

        private void OperationGetProductsAB(string[] fields, List<Product> products)
        {
            if (fields.Length < 12)
                return;
            products.Add(new Product()
            {
                ID = null,
                SymbolSAP = "AB" + fields[(int)AB.indeks],
                //KodProducenta = fields[(int)AB.indeks_p],
                //ModelProduktu = fields[(int)AB.indeks_p],
                OryginalnyKodProducenta = fields[(int)AB.indeks_p],
                NazwaProduktu = fields[(int)AB.nazwa],
                NazwaProducenta = fields[(int)AB.producent],
                KodDostawcy = fields[(int)AB.ID_produktu],
                NazwaDostawcy = "AB",
                StanMagazynowy = parseABwarehouseStatus(fields[(int)AB.magazyn_ilosc]),
                StatusProduktu = false,
                CenaNetto = -1,
                CenaZakupuNetto = Double.Parse(fields[(int)AB.cena_netto].Replace('.', ',')),
                UrlZdjecia = null,
                Kategoria = HttpUtility.HtmlDecode(fields[(int)AB.kategoria])
            });
        }

        private int parseABwarehouseStatus(string value)
        {
            if (value.Contains("+"))
                return 30;

            if (value == "")
                return 0;

            int result = 0;

            if (Int32.TryParse(value, out result))
                return result;

            return 0;
        }

        /// <summary>
        /// Wczytuje produkty z Met
        /// </summary>
        /// <param name="fields"> Lista wczytanych pól z pliku CSV</param>
        /// <param name="products"> Lista do której ma dodać nowy produkt</param>
        private void OperationGetProductsMET(string[] fields, List<Product> products)
        {

            if (fields[(int)Met.SymbolSAP].StartsWith("MET") == false)
                products.Add(new Product()
                {
                    ID = Int32.Parse(fields[(int)Met.ID]),
                    SymbolSAP = fields[(int)Met.SymbolSAP],
                    //ModelProduktu = fields[(int)Met.ModelProduktu],
                    OryginalnyKodProducenta = fields[(int)Met.ModelProduktu],
                    NazwaProduktu = HttpUtility.HtmlDecode(fields[(int)Met.NazwaProduktu]),
                    NazwaProducenta = fields[(int)Met.NazwaProducenta],
                    KodDostawcy = fields[(int)Met.KodUDostawcy],
                    StatusProduktu = Convert.ToBoolean(Int32.Parse(fields[(int)Met.StatusProduktu])),
                    UrlZdjecia = fields[(int)Met.AdresURLzdjecia],
                    Hidden = fields[(int)Met.Kategoria].StartsWith("_HIDDEN")
                    //Wszystkie kategorie które zaczynają się 
                    //_HIDDEN mają być traktowane jak jedna kategoria.
                    //W pliku MET mogą wystąpić kategorie np. _HIDDEN_techdata
                });
        }

        /// <summary>
        ///  Wczytuje wszystkie ceny produktów z drugiego pliku TechDaty
        /// </summary>
        /// <param name="fields">Lista wczytanych pól z pliku CSV</param>
        /// <param name="products">Lista do której dodaje nowe rekordy</param>
        private void OperationGetPriceTD(string[] fields, List<Product> products)
        {
            products.Add(new Product
            {
                SymbolSAP = "TechData" + fields[(int)TechPrice.SapNo],
                CenaNetto = -1,
                CenaZakupuNetto = Double.Parse(fields[(int)TechPrice.Cena]),
            });
        }

        /// <summary>
        /// Wczytuje nazwe producenta z drugiego pliku Lamy.
        /// </summary>
        /// <param name="fields"> Lista wczytanych pól z pliku CSV</param>
        /// <param name="products">Lista do której ma dodać nowe rekordy</param>
        private void OperationGetProducentsLAMA(string[] fields, List<Product> products)
        {
            if (fields.Length < 16)
                return;
            products.Add(new Product
            {
                SymbolSAP = "LAMA" + fields[4],//nr kat
                NazwaProducenta = fields[16]

            });
        }

        /// <summary>
        /// Scala liste produktów z listą ceny TechData-y
        /// </summary>
        /// <param name="products">Lista produktów</param>
        /// <param name="prices">Lista cen</param>
        /// <returns>SZwraca scaloną liste produktów</returns>
        private List<Product> MergePriceTechData(List<Product> products, List<Product> prices)
        {
            foreach (var product in products)
            {
                try
                {
                    var query = prices.Single(p => p.SymbolSAP == product.SymbolSAP);
                    product.CenaNetto = -1;//query.CenaNetto;
                    product.CenaZakupuNetto = query.CenaZakupuNetto;
                }
                catch (Exception e)
                {
                    Console.WriteLine("No product in prices with provided SapNo: {0}", product.SymbolSAP);
                    Log.Logging.LogException(e);
                }

            }
            return products;
        }

        /// <summary>
        /// Scala listę produktów z listą producentów z Lamy.
        /// Jak nie może znaleźć nazwy producenta po Sap Number, to usuwa dany produkt
        /// </summary>
        /// <param name="products">Lista produktów</param>
        /// <param name="producents">Lista producentów</param>
        /// <returns>Zwraca scaloną listę produktów</returns>
        private List<Product> MergeProductLama(List<Product> products, List<Product> producents)
        {
            int count = 0;
            int startSize = products.Count;

            for (int i = 0; i < products.Count; i++)
            {
                try
                {
                    var query = producents.Single(p => p.SymbolSAP == products[i].SymbolSAP);
                    products[i].NazwaProducenta = query.NazwaProducenta;
                    //product.SymbolSAP += "LAMA";
                }
                catch
                {
                    count++;
                    products.RemoveAt(i);
                    i--; //  niweluje przesunięcie po usunięciu elementu
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Number of not found products: {0}", count);
            return products;
        }

    }
}
