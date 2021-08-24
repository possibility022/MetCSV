using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using MET.Data.Models;
using MET.Domain;
using MET.Proxy.Enums;
using MET.Workflows;
using METCSV.Common;

namespace MET.Proxy.ProductReaders
{
    public class MetProductReader : ProductReaderBase
    {
        public override Providers Provider => Providers.MET;

        private static bool EncodingInitialized = false;

        public MetProductReader(CancellationToken token) : base(token)
        {
            ProviderName = "MET";
            InitializeEncoding();
        }

        private static void InitializeEncoding()
        {
            if (!EncodingInitialized)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                EncodingInitialized = true;
            }
        }

        public override IList<Product> GetProducts(string path, string thisPathIsIgnored) => GetMetProducts(path);

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

        private IList<Product> GetMetProducts(string path, Encoding encoding, int linePassCount = 1)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = ";" };

            IEnumerable<string[]> producents = reader.ReadCsv(path, encoding);

            foreach (var fields in producents)
            {
                ThrowIfCanceled();

                if (linePassCount > 0)
                {
                    linePassCount--;
                    continue;
                }
                
                products.Add(new Product(Provider)
                {
                    ID = Int32.Parse(fields[(int)MetCsvProductsColums.ID]),
                    SymbolSAP = DecodeSapSymbol(fields[(int)MetCsvProductsColums.SymbolSAP]),
                    //ModelProduktu = fields[(int)Met.ModelProduktu],
                    OryginalnyKodProducenta = fields[(int)MetCsvProductsColums.ModelProduktu],
                    NazwaProduktu = HttpUtility.HtmlDecode(fields[(int)MetCsvProductsColums.NazwaProduktu]),
                    NazwaProducenta = fields[(int)MetCsvProductsColums.NazwaProducenta],
                    KodDostawcy = fields[(int)MetCsvProductsColums.KodUDostawcy],
                    StatusProduktu = Convert.ToBoolean(Int32.Parse(fields[(int)MetCsvProductsColums.StatusProduktu])),
                    UrlZdjecia = fields[(int)MetCsvProductsColums.AdresURLzdjecia],
                    Hidden = fields[(int)MetCsvProductsColums.Kategoria].StartsWith("_HIDDEN")
                    //Wszystkie kategorie które zaczynają się 
                    //_HIDDEN mają być traktowane jak jedna kategoria.
                    //W pliku MET mogą wystąpić kategorie np. _HIDDEN_techdata
                });
            }

            return products;
        }

        private string DecodeSapSymbol(string sourceValue)
        {
            if (sourceValue.StartsWith("AB"))
                return sourceValue.Remove(0, "AB".Length);

            if (sourceValue.StartsWith("LAMA"))
                return sourceValue.Remove(0, "LAMA".Length);

            if (sourceValue.StartsWith("TechData"))
                return sourceValue.Remove(0, "TechData".Length);

            return sourceValue;
        }
    }
}
