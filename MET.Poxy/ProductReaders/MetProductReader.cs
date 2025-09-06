using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using MET.Data.Models;
using MET.Proxy.Enums;
using MET.Workflows;
using METCSV.Common;

namespace MET.Proxy.ProductReaders
{
    public class MetProductReader : ProductReaderBase
    {
        public override Providers Provider => Providers.Met;

        Dictionary<int, double> productIdToPrice;
        private static bool encodingInitialized;

        public MetProductReader(CancellationToken token) : base(token)
        {
            ProviderName = "MET";
            InitializeEncoding();
        }

        private static void InitializeEncoding()
        {
            if (!encodingInitialized)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                encodingInitialized = true;
            }
        }

        public override IList<Product> GetProducts(string path, string pathToPrices)
        {
            GetMetProductPrices(pathToPrices);
            return GetMetProducts(path);
        }

        private IList<Product> GetMetProducts(string pathProducts)
        {
            Status = OperationStatus.InProgress;

            if (File.Exists(pathProducts))
            {
                LogInfo("Wczytuję produkty z Metu");
                var products = GetMetProducts(pathProducts, Encoding.GetEncoding("windows-1250"));
                LogInfo("Produkty z metu wczytane");
                Status = OperationStatus.Complete;
                return products;
            }
            else
            {
                Status = OperationStatus.Faild;
                string message = $"Problem z wczytywaniem pliku MET: Nie znaleziono pliku - {pathProducts}";
                LogInfo(message);
                throw new FileNotFoundException(message);
            }
        }

        private void GetMetProductPrices(string pathToPrices)
        {
            productIdToPrice = new();
            CsvReader reader = new CsvReader() { Delimiter = ";" };
            IEnumerable<string[]> productsWithPrices = reader.ReadCsv(pathToPrices, Encoding.GetEncoding("windows-1250")).Skip(1);

            foreach (var fields in productsWithPrices)
            {
                ThrowIfCanceled();

                var id = int.Parse(fields[(int)MetCsvProductWithPriceColums.IdProduktu]);
                var cenaNetto = double.Parse(fields[(int)MetCsvProductWithPriceColums.CenaNetto]);

                productIdToPrice.Add(id, cenaNetto);
            }
        }

        private IList<Product> GetMetProducts(string path, Encoding encoding, int linePassCount = 1)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = ";" };

            IEnumerable<string[]> productsFromFile = reader.ReadCsv(path, encoding).Skip(linePassCount);

            foreach (var fields in productsFromFile)
            {
                ThrowIfCanceled();

                Providers originalSource;

                var p = new Product(Provider)
                {
                    Id = Int32.Parse(fields[(int)MetCsvProductsColums.Id]),
                    SymbolSap = DecodeSapSymbol(fields[(int)MetCsvProductsColums.SymbolSap], out originalSource),
                    //ModelProduktu = fields[(int)Met.ModelProduktu],
                    OryginalnyKodProducenta = fields[(int)MetCsvProductsColums.ModelProduktu],
                    NazwaProduktu = HttpUtility.HtmlDecode(fields[(int)MetCsvProductsColums.NazwaProduktu]),
                    NazwaProducenta = fields[(int)MetCsvProductsColums.NazwaProducenta],
                    KodDostawcy = fields[(int)MetCsvProductsColums.KodUDostawcy],
                    StatusProduktu = Convert.ToBoolean(Int32.Parse(fields[(int)MetCsvProductsColums.StatusProduktu])),
                    UrlZdjecia = fields[(int)MetCsvProductsColums.AdresUrLzdjecia],
                    Hidden = fields[(int)MetCsvProductsColums.Kategoria].StartsWith("_HIDDEN"),
                    Kategoria = fields[(int)MetCsvProductsColums.Kategoria]
                    //Wszystkie kategorie które zaczynają się 
                    //_HIDDEN mają być traktowane jak jedna kategoria.
                    //W pliku MET mogą wystąpić kategorie np. _HIDDEN_techdata
                };

                p.OriginalSource = originalSource;

                if (p.Id != null)
                    if (productIdToPrice.TryGetValue(p.Id.Value, out var v))
                        p.SetCennaNetto(v);

                products.Add(p);
            }

            return products;
        }

        private string DecodeSapSymbol(string sourceValue, out Providers provider)
        {
            if (sourceValue.StartsWith("AB"))
            {
                provider = Providers.Ab;
                return sourceValue.Remove(0, "AB".Length);
            }

            if (sourceValue.StartsWith("LAMA"))
            {
                provider = Providers.Lama;
                return sourceValue.Remove(0, "LAMA".Length);
            }

            if (sourceValue.StartsWith("TechData"))
            {
                provider = Providers.TechData;
                return sourceValue.Remove(0, "TechData".Length);
            }

            if (sourceValue.StartsWith("SAP_"))
            {
                provider = Providers.None;
                return sourceValue.Remove(0, "SAP_".Length);
            }

            provider = Providers.None;
            return sourceValue;
        }
    }
}
