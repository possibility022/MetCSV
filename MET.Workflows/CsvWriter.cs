using MET.Domain;
using METCSV.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MET.Data.Models;

namespace MET.Workflows
{
    public class CsvWriter
    {
        private const string delimiter = ";";
        private const string quote = "\"";

        private const string IdHeader = "ID";
        private const string SapHeader = "SymbolSAP";
        private const string OryginalnyKodProducentaHeader = "OryginalnyKodProducenta";
        private const string KodDostawcyHeader = "KodDostawcy";
        private const string NazwaProducentaHeader = "NazwaProducenta";
        private const string NazwaDostawcyHeader = "NazwaDostawcy";
        private const string StanMagazynowyHeader = "StanMagazynowy";
        private const string StatusProduktuHeader = "StatusProduktu";
        private const string CenaNettoHeader = "CenaNetto";
        private const string CzasRealizacjiZamowieniaHeader = "CzasRealizacjiZamowienia";
        private const string CzasGwarancjiHeader = "CzasGwarancji";
        private const string CenaZakupuNettoHeader = "CenaZakupuNetto";
        private const string UrlZdjeciaHeader = "UrlZdjecia";
        private const string KategoriaHeader = "Kategoria";
        private const string NazwaProduktuHeader = "NazwaProduktu";
        private const string KodProducentaHeader = "KodProducenta";
        private const string ScieskaKategoriiHeader = "SciezkaKategorii";
        private const string ModelProduktuHeader = "ModelProduktu";
        private const string KodKreskowyHEader = "KodKreskowy";

        readonly IReadOnlyCollection<string> valuesOrder = new List<string>
            {
                IdHeader,
                NazwaProduktuHeader,
                CenaNettoHeader,
                CenaZakupuNettoHeader,
                StanMagazynowyHeader,
                KodDostawcyHeader,
                SapHeader,
                ModelProduktuHeader,
                StatusProduktuHeader,
                ScieskaKategoriiHeader,

                KodProducentaHeader,
                NazwaProducentaHeader,
                NazwaDostawcyHeader,

                KodKreskowyHEader,
            };


        public bool ExportProducts(string path, IEnumerable<Product> products)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();

            try
            {
                using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create), Encoding.GetEncoding("windows-1250")))
                {
                    WriteHeader(stream, valuesOrder);

                    stream.WriteLine();

                    foreach (var p in products)
                    {
                        AssingToDict(p, ref columns);

                        WriteProduct(stream, columns, valuesOrder);
                    }
                }

                return true;
            }
            catch (IOException ex)
            {
                Log.Error(ex, "Nie udało się zapisać pliku wyjściowego.");
                return false;
            }
        }

        private void AssingToDict(Product p, ref Dictionary<string, string> columns)
        {
            columns[IdHeader] = p.ID.ToString();
            columns[SapHeader] = p.SymbolSAP;
            columns[OryginalnyKodProducentaHeader] = p.OryginalnyKodProducenta;
            columns[KodProducentaHeader] = p.KodProducenta;
            columns[ModelProduktuHeader] = p.ModelProduktu;
            columns[KodDostawcyHeader] = p.KodDostawcy;
            columns[NazwaProduktuHeader] = p.NazwaProduktu;
            columns[NazwaProducentaHeader] = p.NazwaProducenta;
            columns[NazwaDostawcyHeader] = p.NazwaDostawcy;
            columns[StanMagazynowyHeader] = p.StanMagazynowy.ToString();
            columns[StatusProduktuHeader] = p.StatusProduktu ? "1" : "0";
            columns[CenaNettoHeader] = p.CenaNetto.ToString();
            columns[CzasGwarancjiHeader] = "1";
            columns[CzasRealizacjiZamowieniaHeader] = "2";
            columns[ScieskaKategoriiHeader] = string.Empty;
            columns[CenaZakupuNettoHeader] = p.CenaZakupuNetto.ToString();
            columns[UrlZdjeciaHeader] = p.UrlZdjecia;
            columns[KategoriaHeader] = p.Kategoria;
            columns[KodKreskowyHEader] = p.EAN;
        }

        private void WriteProduct(StreamWriter writer, Dictionary<string, string> p, IReadOnlyCollection<string> headers)
        {
            if (headers.Count == 0)
                throw new InvalidOperationException("List of headers should have at least one element.");

            var enumerator = headers.GetEnumerator();

            enumerator.MoveNext();
            Write(writer, p[enumerator.Current]);

            for (int i = 0; i < headers.Count - 1; i++)
            {
                enumerator.MoveNext();
                writer.Write(delimiter);
                Write(writer, p[enumerator.Current]);
            }

            writer.WriteLine();
        }

        private void WriteHeader(StreamWriter writer, IReadOnlyCollection<string> keys)
        {
            if (keys.Count == 0)
                throw new InvalidOperationException("List of headers should have at least one element.");

            var enumerator = keys.GetEnumerator();

            enumerator.MoveNext();
            Write(writer, enumerator.Current);

            for (int i = 0; i < keys.Count - 1; i++)
            {
                enumerator.MoveNext();
                writer.Write(delimiter);
                Write(writer, enumerator.Current);
            }
        }

        private void Write(StreamWriter writer, string value)
        {
            writer.Write(quote);
            writer.Write(value);
            writer.Write(quote);
        }
    }
}
