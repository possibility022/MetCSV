using MET.Domain;
using METCSV.WPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using MET.Data.Models;
using METCSV.Common;

namespace METCSV.WPF.Workflows
{
    public static class ProfitsIO
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
            Profits profits = new Profits(provider);

            if (!File.Exists(path))
            {
                Log.Info($"File {path} could not be found. Using default profits");
                return profits;
            }

            var content = File.ReadAllText(path);
            
            JToken a = JsonConvert.DeserializeObject<JToken>(content); //todo this can be optimized
            
            if (a?.First != null)
            {
                var prof = JsonConvert.DeserializeObject<Dictionary<string, double>>(content);
                profits.SetNewProfits(prof);
            }
            
            return profits;
        }

        public static Profits LoadFromFile(Providers provider)
        {
            return LoadFromFile($"{provider}{App.ProfitsFileExtension}", provider);
        }
    }
}
