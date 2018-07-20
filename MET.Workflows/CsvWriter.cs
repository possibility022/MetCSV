using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MET.Workflows
{
    public class CsvWriter
    {
        private const string delimiter = ";";
        private const string quote = "\"";

        private readonly string _headers = $"\"ID\"{delimiter}\"SymbolSAP\"{delimiter}\"KodProducenta\"{delimiter}\"ModelProduktu\"{delimiter}\"KodDostawcy\"{delimiter}\"NazwaProduktu\"{delimiter}\"NazwaProducenta\"{delimiter}\"NazwaDostawcy\"{delimiter}\"StanMagazynowy\"{delimiter}\"StatusProduktu\"{delimiter}\"CenaNetto\"{delimiter}\"CenaZakupuNetto\"{delimiter}\"UrlZdjecia\"{delimiter}\"Kategoria\"";

        public bool ExportProducts(string path, IEnumerable<Product> products)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create), Encoding.GetEncoding("windows-1250")))
                {
                    stream.WriteLine(_headers);
                    
                    foreach (var p in products)
                    {
                        WriteProduct(stream, p);
                    }
                }

                return true;
            }
            catch (IOException ex)
            {
                // todo log;
                return false;
            }
        }

        private void WriteProduct(StreamWriter writer, Product p)
        {
            writer.Write(quote);

            writer.Write(p.ID);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.SymbolSAP);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.OryginalnyKodProducenta);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.OryginalnyKodProducenta);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.KodDostawcy);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.NazwaProducenta);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.NazwaDostawcy);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.StanMagazynowy);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.StatusProduktu);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.CenaNetto);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.CenaZakupuNetto);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.UrlZdjecia);

            writer.Write(quote);
            writer.Write(delimiter);
            writer.Write(quote);

            writer.Write(p.Kategoria);

            writer.Write(quote);

            writer.WriteLine();
        }
    }
}
