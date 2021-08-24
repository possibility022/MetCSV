using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MET.Data.Models;
using MET.Domain;
using METCSV.WPF;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.Workflows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace METCSV.UnitTests.Workflows
{
    [TestClass]
    public class ProfitsIOTests
    {
        [TestMethod]
        public void Serialize()
        {
            //Assert
            Profits profits = new Profits(Providers.AB);

            List<EditableDictionaryKey<string, double>> prof = new List<EditableDictionaryKey<string, double>>()
            {
                new EditableDictionaryKey<string, double>("a", 0.1),
                new EditableDictionaryKey<string, double>("b", 0.2),
                new EditableDictionaryKey<string, double>("c", 0.3)
            };

            profits.SetNewProfits(prof);

            var file = ProfitsIO.SaveToFile(profits);

            if (File.Exists(file))
            {
                var contetn = File.ReadAllText(file);
                Trace.WriteLine(contetn);

                File.Delete(file);
                Assert.IsFalse(File.Exists(file));
            }
            else
            {
                Assert.Fail("File does not exists");
            }
        }

        [TestMethod]
        public void Deserialize()
        {
            Dictionary<string, double> prof = new Dictionary<string, double>()
            {
                {"a", 0.2 },
                { "b", 0.2 },
                { "c", 0.3 }
            };

            var json = JsonConvert.SerializeObject(prof);
            File.WriteAllText($"{Providers.AB}{App.ProfitsFileExtension}", json);

            var profits = ProfitsIO.LoadFromFile(Providers.AB);

            foreach (var p in profits.Values)
            {
                Trace.WriteLine($"{p.Key} : {p.Value}");
            }

            Assert.AreEqual(3, profits.Values.Count);
        }


        [TestMethod]
        public void DeserializeDictionary()
        {
            //Assert
            Profits profits = new Profits(Providers.AB);

            Dictionary<string, double> prof = new Dictionary<string, double>()
            {
                ["a"] = 0.1,
                ["b"] = 0.2,
                ["c"] = 0.3
            };

            profits.SetNewProfits(prof);

            var file = ProfitsIO.SaveToFile(profits);

            if (File.Exists(file))
            {
                var contetn = File.ReadAllText(file);
                Trace.WriteLine(contetn);

                File.Delete(file);
                Assert.IsFalse(File.Exists(file));
            }
            else
            {
                Assert.Fail("File does not exists");
            }
        }

        [TestMethod]
        public void SerializingEmptyCollectionAndDeserializingEmptyCollection()
        {
            // Arrange
            var profits = Factory.GetProfits(Providers.AB);

            // Act
            ProfitsIO.SaveToFile(profits);
            profits = ProfitsIO.LoadFromFile(Providers.AB);

            // Assert
            Assert.IsNotNull(profits.Values);

        }
    }
}
