using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MET.Data.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Enums;
using MET.Workflows;
using METCSV.Common;

namespace MET.Proxy.ProductReaders
{
    public class TechDataProductReader : ProductReaderBase
    {
        public override Providers Provider => Providers.TechData;

        private readonly string csvDelimiter;

        public TechDataProductReader(ITechDataReaderSettings settings, CancellationToken token) : base(token)
        {
            ProviderName = "TechData";
            csvDelimiter = settings.CsvDelimiter;
        }

        public override IList<Product> GetProducts(string pathProducts, string pathPrices) =>
            ReadProducts(pathProducts, pathPrices);

        private IList<Product> ReadProducts(string pathProducts, string pathPrices)
        {
            Status = OperationStatus.InProgress;
            if (File.Exists(pathProducts) && File.Exists(pathPrices))
            {
                LogInfo("Wczytuję produkty z TechDaty");
                var prices = ReadPricesFromCsvFile(pathPrices, Encoding.Default);
                var products = ReadProductsFromCsvFile(pathProducts, Encoding.GetEncoding("windows-1250"));

                var merged = MergePriceTechData(products, prices);

                LogInfo("Produkty z techdaty wczytane");

                Status = OperationStatus.Complete;
                return merged;
            }
            else
            {
                string message = $"Nie znaleziono jednego z plikow. {pathPrices} {pathProducts}";
                Status = OperationStatus.Faild;
                LogInfo(message);
                throw new FileNotFoundException(message);
            }
        }

        private IList<Product> ReadPricesFromCsvFile(string filePath, Encoding encoding, int linePassCount = 1)
        {
            List<Product> prices = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = csvDelimiter };

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);

            foreach (var fields in producents)
            {
                ThrowIfCanceled();

                if (linePassCount > 0)
                {
                    linePassCount--;
                    continue;
                }

                prices.Add(new Product(Provider)
                {
                    SymbolSAP = fields[(int)TechDataCsvPricesColumns.SapNo],
                    CenaZakupuNetto = Double.Parse(fields[(int)TechDataCsvPricesColumns.Cena]),
                });
            }

            return prices;
        }

        private IList<Product> ReadProductsFromCsvFile(string filePath, Encoding encoding, int linePassCount = 1)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = csvDelimiter };

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);

            foreach (string[] fields in producents)
            {
                ThrowIfCanceled();

                if (linePassCount > 0)
                {
                    linePassCount--;
                    continue;
                }

                products.Add(new Product(Provider)
                {
                    ID = null,
                    SymbolSAP = fields[(int)TechDataCsvProductsColumns.SapNo],
                    //KodProducenta = fields[(int)TechData.PartNo],
                    //ModelProduktu = fields[(int)TechData.PartNo],
                    OryginalnyKodProducenta = fields[(int)TechDataCsvProductsColumns.PartNo],
                    NazwaProduktu = fields[(int)TechDataCsvProductsColumns.Nazwa],
                    NazwaProducenta = fields[(int)TechDataCsvProductsColumns.Vendor],
                    KodDostawcy = fields[(int)TechDataCsvProductsColumns.SapNo],
                    NazwaDostawcy = ProviderName,
                    StanMagazynowy = Int32.Parse(fields[(int)TechDataCsvProductsColumns.Magazyn]),
                    StatusProduktu = false,
                    CenaZakupuNetto = -1,
                    UrlZdjecia = null,
                    Kategoria = fields[(int)TechDataCsvProductsColumns.FamilyPr_kod],
                    EAN = fields[(int)TechDataCsvProductsColumns.EAN],
                });

                if (products[products.Count - 1].NazwaProduktu.Contains("ienny Uchwyt do  Monitora"))
                    Console.WriteLine("TOON");
            }

            return products;
        }

        /// <summary>
        /// Scala liste produktów z listą ceny TechData-y
        /// </summary>
        /// <param name="products">Lista produktów</param>
        /// <param name="prices">Lista cen</param>
        /// <returns>Zwraca scaloną liste produktów</returns>
        private IList<Product> MergePriceTechData(IList<Product> products, IList<Product> prices)
        {

            // we cannot use SapManuHash here
            var pricesDict = new Dictionary<string, Product>();

            foreach (var price in prices)
            {
                if (!pricesDict.ContainsKey(price.SymbolSAP))
                    pricesDict.Add(price.SymbolSAP, price);
                else
                    LogError($"Two same keys in TechData prices. {price.SymbolSAP}. Open the file and check that value.");
            }

            foreach (var product in products)
            {
                if (pricesDict.ContainsKey(product.SymbolSAP))
                {
                    product.CenaZakupuNetto = pricesDict[product.SymbolSAP].CenaZakupuNetto;
                }
                else
                {
                    LogError($"No product in prices with provided SapNo: {product.SymbolSAP}");
                }
            }
            return products;
        }

    }
}
