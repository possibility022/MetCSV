using METCSV.WPF.Enums;
using METCSV.WPF.Models;
using METCSV.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.UnitTests
{
    class Factory
    {

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

        public static Profits GetProfits(Providers provider = Providers.AB)
        {
            return new Profits(provider);
        }

        public static ProfitsViewModel GetProfitsViewModel()
        {
            var model = new ProfitsViewModel();
            model.AddManufacturers(GetManufacturers());
            return model;
        }
    }
}