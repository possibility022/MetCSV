﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using METCSV.Common;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;

namespace METCSV.WPF.ProductReaders
{
    class LamaProductReader : IProductReader
    {

        #region IProductReader

        public OperationStatus Status { get; private set; }

        public IEnumerable<Product> GetProducts(string filename, string filename2) => LoadProducts(filename, filename2);

        public EventHandler OnStatusMessage { get; private set; }
        public string ProviderName { get; } = "Lama";

        #endregion
        
        private IEnumerable<Product> LoadProducts(string pathXml, string pathCsv)
        {
            Status = OperationStatus.InProgress;

            if (File.Exists(pathXml) && File.Exists(pathCsv))
            {
                Log("Wczytuję produkty z Lamy");
                var products = ReadLama(pathXml); // wczytanie produktów z XML
                var producents = ReadProducents(pathCsv, Encoding.Default); // wczytanie producentów produktów

                var merged = MergeProductLama(products, producents); // scalenie plików
                
                Log("Produkty z Lamy wczytane");

                Status = OperationStatus.Complete;
                return merged;
            }
            else
            {
                string message = $"Problem z wczytywaniem lamy: Któryś z podanych plików nie istnieje. Czy nie zapomniałeś o rozszerzeniu pliku w nazwie? Szukane pliki 1: {pathXml} 2: {pathCsv}";
                Status = OperationStatus.Faild;
                Log(message);
                throw new FileNotFoundException(message);
            }
        }

        private List<Product> ReadProducents(string pathCsv, Encoding encoding)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() {Delimiter = ";"};

            IEnumerable<string[]> producents = reader.ReadCsv(pathCsv, encoding);

            foreach (var fields in producents)
            {
                if (fields.Length < 16)
                    continue;
                products.Add(new Product
                {
                    SymbolSAP = "LAMA" + fields[4],//nr kat
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
        private IEnumerable<Product> MergeProductLama(List<Product> products, List<Product> producents)
        {
            int count = 0;

            for (int i = 0; i < products.Count; i++)
            {
                try
                {
                    var query = producents.Single(p => p.SymbolSAP == products[i].SymbolSAP);
                    products[i].NazwaProducenta = query.NazwaProducenta;
                }
                catch (InvalidOperationException)
                {
                    count++;
                    products.RemoveAt(i);
                    i--; //  niweluje przesunięcie po usunięciu elementu
                }
            }

            Log($"Number of not found products: {count}");

            return products;
        }

        private void Log(string message)
        {
            OnStatusMessage?.Invoke(this, EventArgs.Empty); //todo change Empty to message
        }


        /// <summary>
        /// Wczytuje wszystkie dostępne dane z podstawowego pliku lamy, bez nazw producentów.
        /// Jak nie ma kodu procudenta albo part number to nie bierze pod uwagę takich produktów
        /// </summary>
        /// <param name="path"> Ściężka do pliku z produktami lamy</param>
        /// <returns> Liste produktów</returns>
        private List<Product> ReadLama(string path)
        {
            StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("ISO-8859-2"));
            var xmlReader = XmlReader.Create(streamReader);

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
                    OryginalnyKodProducenta = HttpUtility.HtmlDecode(product.Element("PN").Value),
                    KodDostawcy = HttpUtility.HtmlDecode(product.Element("KOD").Value),
                    NazwaProduktu = HttpUtility.HtmlDecode(product.Element("NAZEV").Value),
                    NazwaProducenta = null,
                    NazwaDostawcy = ProviderName,
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