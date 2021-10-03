using MET.Domain;
using METCSV.WPF.Models;
using METCSV.WPF.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MET.Data.Models;
using METCSV.WPF.ViewModels.ProfitsInnerModels;

namespace METCSV.UnitTests
{
    class Factory
    {

        static CancellationTokenSource token = new CancellationTokenSource();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static ManufacturersCollection GetManufacturers(Providers providers = Providers.AB)
        {
            return new ManufacturersCollection(providers, new HashSet<string>() { "ABC", "CDE" });
        }

        public static CategoryCollection GetCategories(Providers providers = Providers.AB)
        {
            return new CategoryCollection(providers, new HashSet<string>() { "ABC", "CDE" });
        }

        public static Profits GetProfits(Providers provider = Providers.AB)
        {
            return new Profits(provider);
        }

        public static ProfitsViewModel GetProfitsViewModel()
        {
            var model = new ProfitsViewModel();
            model.AddCategories(GetCategories());
            return model;
        }

        public static List<Product> GetLamaProducts()
        {
            var content = File.ReadAllText(@"Repository\lamaproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetABProducts()
        {
            var content = File.ReadAllText(@"Repository\abproducts.json");
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        public static List<Product> GetTDProducts()
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
                new Product(Providers.None) { SymbolSAP = "ABC", NazwaProduktu  = "Produkt",  NazwaProducenta = "Producent", OryginalnyKodProducenta = "A" },
                new Product(Providers.None) { SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "_" }
            };
        }

        public static List<Product> GetShortMetList()
        {
            return new List<Product> {
                new Product(Providers.MET) {SymbolSAP = "ABC1", NazwaProduktu  = "Produkt1", NazwaProducenta = "Producent1", OryginalnyKodProducenta = "A"},
                new Product(Providers.MET) {SymbolSAP = "ABC2", NazwaProduktu  = "Produkt2",  NazwaProducenta = "Producent2", OryginalnyKodProducenta = "B"},
                new Product(Providers.MET) {SymbolSAP = "ABC3", NazwaProduktu  = "Produkt3",  NazwaProducenta = "Producent3", OryginalnyKodProducenta = "C"}
            };
        }
    }
}