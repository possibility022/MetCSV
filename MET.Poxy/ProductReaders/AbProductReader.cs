using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using MET.Domain;
using MET.Proxy.Configuration;
using MET.Proxy.Enums;
using MET.Workflows;
using METCSV.Common;

namespace MET.Proxy.ProductReaders
{
    public class AbProductReader : ProductReaderBase 
    {
        public override Providers Provider => Providers.AB;

        private readonly string CsvFileEncoding;
        private readonly string CsvDelimiter;

        public AbProductReader(AbDownloaderSettings settings, CancellationToken token) : base(token)
        {
            ProviderName = "AB";
            SapPrefix = settings.SAPPrefix;
            CsvFileEncoding = settings.CsvFileEncoding;
            CsvDelimiter = settings.CsvDelimiter;
        }

        public override IList<Product> GetProducts(string filename, string filename2) =>
            GetProducts(filename, null);

        public IList<Product> GetProducts(string csvPath, Encoding encoding)
        {
            ThrowIfCanceled();

            Status = OperationStatus.InProgress;

            if (encoding == null)
            {
                encoding = Encoding.GetEncoding(CsvFileEncoding);
            }

            IList<Product> products = new List<Product>();
            try
            {
                LogInfo("Wczytuję produkty z AB");

                if (!File.Exists(csvPath))
                {
                    LogError($"Nie znaleziono pliku csv: {csvPath}");
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
            CsvReader reader = new CsvReader() { Delimiter = CsvDelimiter };

            IEnumerable<string[]> producents = reader.ReadCsv(filePath, encoding);
            
            foreach (var fields in producents)
            {
                ThrowIfCanceled();

                if (passLinesCount > 0 || fields.Length < 12)
                {
                    passLinesCount--;
                    continue;
                }

                products.Add(new Product(Provider)
                {
                    ID = null,
                    SymbolSAP = fields[(int)AbCsvProductsColumns.indeks],
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
                    Kategoria = HttpUtility.HtmlDecode(fields[(int)AbCsvProductsColumns.kategoria]),
                    EAN = fields[(int)AbCsvProductsColumns.EAN],
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
