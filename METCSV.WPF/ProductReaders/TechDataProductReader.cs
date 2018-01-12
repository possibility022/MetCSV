using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using METCSV.Common;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;

namespace METCSV.WPF.ProductReaders
{
    class TechDataProductReader : IProductReader
    {
        #region IProductReader

        public IEnumerable<Product> GetProducts(string pathProducts, string pathPrices) =>
            ReadProducts(pathProducts, pathPrices);

        public OperationStatus Status { get; private set; }
        public EventHandler OnStatusMessage { get; private set; }
        public string ProviderName { get; } = "TechData";

        #endregion

        private IEnumerable<Product> ReadProducts(string pathProducts, string pathPrices)
        {
            Status = OperationStatus.InProgress;
            if (File.Exists(pathProducts) && File.Exists(pathPrices))
            {
                Log("Wczytuję produkty z TechDaty");
                var prices = ReadPricesFromCsvFile(pathPrices, Encoding.Default);
                var products = ReadProductsFromCsvFile(pathProducts, Encoding.Default);

                var merged = MergePriceTechData(products, prices);

                Log("Produkty z techdaty wczytane");

                Status = OperationStatus.Complete;
                return merged;
            }
            else
            {
                string message = $"Nie znaleziono jednego z plikow. {pathPrices} {pathProducts}";
                Status = OperationStatus.Faild;
                Log(message);
                throw new FileNotFoundException(message);
            }
        }

        private List<Product> ReadPricesFromCsvFile(string filePath, Encoding encoding)
        {
            List<Product> prices = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = ";" };

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);

            foreach (var fields in producents)
            {
                if (fields.Length < 16)
                    continue;
                prices.Add(new Product
                {
                    SymbolSAP = "TechData" + fields[(int)TechDataCsvPricesColumns.SapNo],
                    CenaNetto = -1,
                    CenaZakupuNetto = Double.Parse(fields[(int)TechDataCsvPricesColumns.Cena]),
                });
            }

            return prices;
        }

        private List<Product> ReadProductsFromCsvFile(string filePath, Encoding encoding)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = ";" };

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);

            foreach (string[] fields in producents)
            {
                products.Add(new Product()
                {
                    ID = null,
                    SymbolSAP = "TechData" + fields[(int)TechDataCsvProductsColumns.SapNo], //todo we can move prefix to config. We can do this for all IProductReader's
                    //KodProducenta = fields[(int)TechData.PartNo],
                    //ModelProduktu = fields[(int)TechData.PartNo],
                    OryginalnyKodProducenta = fields[(int)TechDataCsvProductsColumns.PartNo],
                    NazwaProduktu = fields[(int)TechDataCsvProductsColumns.Nazwa],
                    NazwaProducenta = fields[(int)TechDataCsvProductsColumns.Vendor],
                    KodDostawcy = fields[(int)TechDataCsvProductsColumns.SapNo],
                    NazwaDostawcy = ProviderName,
                    StanMagazynowy = Int32.Parse(fields[(int)TechDataCsvProductsColumns.Magazyn]),
                    StatusProduktu = false,
                    CenaNetto = -1,
                    CenaZakupuNetto = -1,
                    UrlZdjecia = null,
                    Kategoria = fields[(int)TechDataCsvProductsColumns.FamilyPr_kod]
                });
            }

            return products;
        }

        /// <summary>
        /// Scala liste produktów z listą ceny TechData-y
        /// </summary>
        /// <param name="products">Lista produktów</param>
        /// <param name="prices">Lista cen</param>
        /// <returns>Zwraca scaloną liste produktów</returns>
        private List<Product> MergePriceTechData(List<Product> products, List<Product> prices)
        {
            foreach (var product in products)
            {
                try
                {
                    var query = prices.Single(p => p.SymbolSAP == product.SymbolSAP);
                    product.CenaNetto = -1;
                    product.CenaZakupuNetto = query.CenaZakupuNetto;
                }
                catch
                {
                    Console.WriteLine(@"No product in prices with provided SapNo: {0}", product.SymbolSAP); //todo move it to looger or debug.log?
                }

            }
            return products;
        }

        private void Log(string message)
        {
            //todo implement
        }

    }
}
