using MET.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MET.Data.Models;

namespace METCSV.UnitTests
{
    class Factory
    {

        static CancellationTokenSource token = new CancellationTokenSource();
        const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();

        public static HashSet<string> GenerateUniqueSapNumbers(int count)
        {
            HashSet<string> sapNumbers = new HashSet<string>();

            while (sapNumbers.Count < count)
            {
                sapNumbers.Add(RandomString(7));
            }

            return sapNumbers;
        }

        private static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static List<Product> GetLamaProducts()
        {
            var content = File.ReadAllText(@"Repository\lamaproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetAbProducts()
        {
            var content = File.ReadAllText(@"Repository\abproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetTdProducts()
        {
            var content = File.ReadAllText(@"Repository\techjDataproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetMetProducts()
        {
            var content = File.ReadAllText(@"Repository\metproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetShortVendorList()
        {
            return new List<Product>()
            {
                new(Providers.None) { SymbolSap = "ABC", NazwaProduktu  = "Produkt",  NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new(Providers.None) { SymbolSap = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
            };
        }
    }
}