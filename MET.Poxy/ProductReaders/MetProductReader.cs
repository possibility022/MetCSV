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
        public override Providers Provider => Providers.MET;

        Dictionary<int, double> _productIdToPrice;
        private static bool _encodingInitialized;

        public MetProductReader(CancellationToken token) : base(token)
        {
            ProviderName = "MET";
            InitializeEncoding();
        }

        private static void InitializeEncoding()
        {
            if (!_encodingInitialized)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                _encodingInitialized = true;
            }
        }

        public override IList<Product> GetProducts(string path, string pathToPrices)
        {
            GetMetProductPrices(pathToPrices);
            return GetMetProducts(path);
        }

        public IList<Product> GetMetProducts(string pathProducts)
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

        private Dictionary<int, double> GetMetProductPrices(string pathToPrices)
        {
            _productIdToPrice = new();
            CsvReader reader = new CsvReader() { Delimiter = ";" };
            IEnumerable<string[]> productsWithPrices = reader.ReadCsv(pathToPrices, Encoding.GetEncoding("windows-1250")).Skip(1);

            foreach (var fields in productsWithPrices)
            {
                ThrowIfCanceled();

                var id = int.Parse(fields[(int)MetCsvProductWithPriceColums.ID_produktu]);
                var cenaNetto = double.Parse(fields[(int)MetCsvProductWithPriceColums.Cena_Netto]);

                _productIdToPrice.Add(id, cenaNetto);
            }

            return _productIdToPrice;
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
                    ID = Int32.Parse(fields[(int)MetCsvProductsColums.ID]),
                    SymbolSAP = DecodeSapSymbol(fields[(int)MetCsvProductsColums.SymbolSAP], out originalSource),
                    //ModelProduktu = fields[(int)Met.ModelProduktu],
                    OryginalnyKodProducenta = fields[(int)MetCsvProductsColums.ModelProduktu],
                    NazwaProduktu = HttpUtility.HtmlDecode(fields[(int)MetCsvProductsColums.NazwaProduktu]),
                    NazwaProducenta = fields[(int)MetCsvProductsColums.NazwaProducenta],
                    KodDostawcy = fields[(int)MetCsvProductsColums.KodUDostawcy],
                    StatusProduktu = Convert.ToBoolean(Int32.Parse(fields[(int)MetCsvProductsColums.StatusProduktu])),
                    UrlZdjecia = fields[(int)MetCsvProductsColums.AdresURLzdjecia],
                    Hidden = fields[(int)MetCsvProductsColums.Kategoria].StartsWith("_HIDDEN"),
                    Kategoria = fields[(int)MetCsvProductsColums.Kategoria]
                    //Wszystkie kategorie które zaczynają się 
                    //_HIDDEN mają być traktowane jak jedna kategoria.
                    //W pliku MET mogą wystąpić kategorie np. _HIDDEN_techdata
                };

                p.OriginalSource = originalSource;

                if (p.ID != null)
                    if (_productIdToPrice.TryGetValue(p.ID.Value, out var v))
                        p.SetCennaNetto(v);

                products.Add(p);
            }

            return products;
        }

        private string DecodeSapSymbol(string sourceValue, out Providers provider)
        {
            if (sourceValue.StartsWith("AB"))
            {
                provider = Providers.AB;
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
