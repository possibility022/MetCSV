using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using MET.Domain;
using MET.Proxy.Configuration;
using MET.Workflows;
using METCSV.Common;

namespace MET.Proxy.ProductReaders
{
    public class LamaProductReader : ProductReaderBase
    {
        public override Providers Provider => Providers.Lama;

        private readonly string FileEncoding;

        private readonly string CsvDelimiter;

        public LamaProductReader(LamaDownloaderSettings settings, CancellationToken token) : base(token)
        {
            ProviderName = "Lama";
            SapPrefix = settings.SAPPrefix;
            CsvDelimiter = settings.CsvDelimiter;
            FileEncoding = settings.CsvFileEncoding;
        }

        public override IList<Product> GetProducts(string filename, string filename2) => LoadProducts(filename, filename2);

        private IList<Product> LoadProducts(string pathXml, string pathCsv)
        {
            Status = OperationStatus.InProgress;

            // plik csv musi byc dostarczony przez uzytkownika.

            if (File.Exists(pathXml) && File.Exists(pathCsv))
            {
                LogInfo("Wczytuję produkty z Lamy");
                var products = ReadLama(pathXml); // wczytanie produktów z XML
                var producents = ReadProducents(pathCsv, Encoding.Default); // wczytanie producentów produktów

                var merged = MergeProductLama(products, producents); // scalenie plików

                LogInfo("Produkty z Lamy wczytane");

                Status = OperationStatus.Complete;
                return merged;
            }
            else
            {
                string message = $"Problem z wczytywaniem lamy: Któryś z podanych plików nie istnieje. Czy nie zapomniałeś o rozszerzeniu pliku w nazwie? Szukane pliki 1: \"{pathXml}\" 2: \"{pathCsv}\". Pamiętaj, że plik CSV musi być dostarczony przez użytkownika.";
                Status = OperationStatus.Faild;
                LogInfo(message);
                throw new FileNotFoundException(message);
            }
        }

        private List<Product> ReadProducents(string pathCsv, Encoding encoding, int linePassCount = 1)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = CsvDelimiter };

            IEnumerable<string[]> producents = reader.ReadCsv(pathCsv, encoding);

            foreach (var fields in producents)
            {
                ThrowIfCanceled();

                if (linePassCount > 0)
                {
                    linePassCount--;
                    continue;
                }

                if (fields.Length < 16)
                    continue;

                products.Add(new Product(Provider)
                {
                    SymbolSAP = SapPrefix + fields[4],//nr kat
                    NazwaProducenta = fields[16]
                });
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
        private IList<Product> MergeProductLama(List<Product> products, List<Product> producents)
        {
            int count = 0;

            var producentsDict = new Dictionary<string, Product>();

            foreach (var p in producents)
            {
                ThrowIfCanceled();

                if (producentsDict.ContainsKey(p.SymbolSAP))
                {
                    LogError($"Met producents file contains two the same sap numbers. Check that out. SAP : {p.SymbolSAP}");
                }
                else
                {
                    producentsDict.Add(p.SymbolSAP, p);
                }
            }

            for (int i = 0; i < products.Count; i++)
            {
                ThrowIfCanceled();

                if (producentsDict.ContainsKey(products[i].SymbolSAP))
                {
                    products[i].NazwaProducenta = producentsDict[products[i].SymbolSAP].NazwaProducenta;
                }
                else
                {
                    count++;
                    products.RemoveAt(i);
                    i--; //  niweluje przesunięcie po usunięciu elementu
                }
            }

            LogInfo($"Number of not found products: {count}. You can download new CSV file to decrease this number.");

            return products;
        }


        /// <summary>
        /// Wczytuje wszystkie dostępne dane z podstawowego pliku lamy, bez nazw producentów.
        /// Jak nie ma kodu procudenta albo part number to nie bierze pod uwagę takich produktów
        /// </summary>
        /// <param name="path"> Ściężka do pliku z produktami lamy</param>
        /// <returns> Liste produktów</returns>
        private List<Product> ReadLama(string path)
        {
            StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding(FileEncoding));
            var xmlReader = XmlReader.Create(streamReader);

            XDocument.Load(xmlReader);

            List<Product> products = new List<Product>();
            var xml = XDocument.Load(path);


            foreach (var product in xml.Root.Elements())
            {
                ThrowIfCanceled();

                var urls = GetUrlsLama(product); // wszystkie Urle danego produktu

                if (product.Element("KOD").Value == "" || product.Element("PN").Value == "")
                {
                    continue;
                }

                products.Add(new Product(Provider)
                {
                    ID = null,
                    SymbolSAP = SapPrefix + HttpUtility.HtmlDecode(product.Element("KOD").Value),
                    OryginalnyKodProducenta = HttpUtility.HtmlDecode(product.Element("PN").Value),
                    KodDostawcy = HttpUtility.HtmlDecode(product.Element("KOD").Value),
                    NazwaProduktu = HttpUtility.HtmlDecode(product.Element("NAZEV").Value),
                    NazwaProducenta = string.Empty,
                    NazwaDostawcy = ProviderName,
                    StanMagazynowy = Int32.Parse(product.Element("SKLAD_NUM").Value),
                    StatusProduktu = false,
                    CenaZakupuNetto = Double.Parse(product.Element("CENA").Value),
                    UrlZdjecia = urls.Count > 0 ? urls[0] : null
                });
            }

            return products;
        }

        /// <summary>
        /// Wczytuje wszystkie Url-e z xml Lamy i zwraca w pozstaci listy
        /// </summary>
        /// <param name="product"> </param>
        /// <returns>Lista urlów w postaci stringoów albo pusta lista.</returns>
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
    }
}
