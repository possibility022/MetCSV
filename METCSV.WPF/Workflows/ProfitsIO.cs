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

            string file = $"{profit.Provider}.prof";

            File.WriteAllText(file, json);

            return file;
        }

        public static Profits LoadFromFile(string path)
        {
            var content = File.ReadAllText(path);

            Profits profits = new Profits(Path.GetFileNameWithoutExtension(path));

            var prof = JsonConvert.DeserializeObject<List<EditableDictionaryKey<string, double>>>(content);
            profits.SetNewProfits(prof);

            return profits;
        }
    }
}
