using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            Profits profits = new Profits("Lama");

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
            List<EditableDictionaryKey<string, double>> prof = new List<EditableDictionaryKey<string, double>>()
            {
                new EditableDictionaryKey<string, double>("a", 0.1),
                new EditableDictionaryKey<string, double>("b", 0.2),
                new EditableDictionaryKey<string, double>("c", 0.3)
            };

            var json = JsonConvert.SerializeObject(prof);
            File.WriteAllText("unitTest.tmp", json);

            var profits = ProfitsIO.LoadFromFile("unitTest.tmp");

            foreach(var p in profits.Values)
            {
                Trace.WriteLine($"{p.Key} : {p.Value}");
            }

            Assert.AreEqual(3, profits.Values.Count);
        }
    }
}
