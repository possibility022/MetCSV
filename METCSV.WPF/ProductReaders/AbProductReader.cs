using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using METCSV.Common;
using METCSV.WPF.Enums;

namespace METCSV.WPF.ProductReaders
{
    class AbProductReader : ProductReaderBase 
    {
        public override Providers Provider => Providers.AB;

        public AbProductReader(CancellationToken token)
        {
            SetCancellationToken(token);
            ProviderName = "AB";
        }

        public override IList<Product> GetProducts(string filename, string filename2) =>
            GetProducts(filename, null);

        public IList<Product> GetProducts(string csvPath, Encoding encoding)
        {
            Status = OperationStatus.InProgress;

            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("windows-1250"); //todo move it to config somehow
            }

            IList<Product> products = new List<Product>();
            try
            {
                LogInfo("Wczytuję produkty z AB");

                if (!File.Exists(csvPath))
                {
                    throw new FileNotFoundException("Nie znaleziono pliku csv. " + csvPath);     //todo remove throwing
                }

                products = ReadProductsFromCsvFile(csvPath, encoding, 2);

                LogInfo("Produkty z AB wczytane");
                Status = OperationStatus.Complete;
            }
            catch (Exception ex)
            {
                LogInfo($"AB exception during collectiong products. {ex.Message}");
                Status = OperationStatus.Faild;
            }

            return products;
        }

        private IList<Product> ReadProductsFromCsvFile(string filePath, Encoding encoding, int passLinesCount = 2)
        {
            List<Product> products = new List<Product>();
            CsvReader reader = new CsvReader() { Delimiter = ";" }; // todo to config

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);
            
            foreach (var fields in producents)
            {
                if (passLinesCount > 0 || fields.Length < 12)
                {
                    passLinesCount--;
                    continue;
                }

                products.Add(new Product(Provider)
                {
                    ID = null,
                    SymbolSAP = "AB" + fields[(int)AbCsvProductsColumns.indeks],
                    //KodProducenta = fields[(int)AB.indeks_p],
                    //ModelProduktu = fields[(int)AB.indeks_p],
                    OryginalnyKodProducenta = fields[(int)AbCsvProductsColumns.indeks_p],
                    NazwaProduktu = fields[(int)AbCsvProductsColumns.nazwa],
                    NazwaProducenta = fields[(int)AbCsvProductsColumns.producent],
                    KodDostawcy = fields[(int)AbCsvProductsColumns.ID_produktu],
                    NazwaDostawcy = ProviderName,
                    StanMagazynowy = ParseABwarehouseStatus(fields[(int)AbCsvProductsColumns.magazyn_ilosc]),
                    StatusProduktu = false,
                    CenaZakupuNetto = Double.Parse(fields[(int)AbCsvProductsColumns.cena_netto].Replace('.', ',')),
                    UrlZdjecia = null,
                    Kategoria = HttpUtility.HtmlDecode(fields[(int)AbCsvProductsColumns.kategoria])
                });
            }

            return products;
        }

        private int ParseABwarehouseStatus(string value)
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

        
    }
}
