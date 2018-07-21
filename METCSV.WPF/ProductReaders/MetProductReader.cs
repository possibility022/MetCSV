using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using MET.Workflows;
using MET.Domain;
using METCSV.WPF.Enums;
using METCSV.Common;

namespace METCSV.WPF.ProductReaders
{
    class MetProductReader : ProductReaderBase
    {
        public override Providers Provider => Providers.MET;

        public MetProductReader(CancellationToken token)
        {
            SetCancellationToken(token);
            ProviderName = "MET";
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
                if (linePassCount > 0)
                {
                    linePassCount--;
                    continue;
                }

                if (fields[(int)MetCsvProductsColums.SymbolSAP].StartsWith("MET") == false)
                    products.Add(new Product(Provider)
                    {
                        ID = Int32.Parse(fields[(int)MetCsvProductsColums.ID]),
                        SymbolSAP = fields[(int)MetCsvProductsColums.SymbolSAP],
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
    }
}
