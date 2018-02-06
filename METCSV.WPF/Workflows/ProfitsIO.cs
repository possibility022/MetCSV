using METCSV.WPF.Enums;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace METCSV.WPF.Workflows
{
    public class ProfitsIO
    {
        public static string SaveToFile(Profits profit)
        {
            var json = JsonConvert.SerializeObject(profit.Values);

            string file = $"{profit.Provider}{App.ProfitsFileExtension}";

            File.WriteAllText(file, json);

            return file;
        }

        public static Profits LoadFromFile(string path, Providers provider)
        {
            var content = File.ReadAllText(path);

            Profits profits = new Profits(provider);

            var prof = JsonConvert.DeserializeObject<List<EditableDictionaryKey<string, double>>>(content);
            profits.SetNewProfits(prof);

            return profits;
        }

        public static Profits LoadFromFile(Providers provider)
        {
            return LoadFromFile($"{provider}.{App.ProfitsFileExtension}", provider);
        }

        public static string SaveToFile(Providers provider)
        {
            return SaveToFile(provider);
        }
    }
}
