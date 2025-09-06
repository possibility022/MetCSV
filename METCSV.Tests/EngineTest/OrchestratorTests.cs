using System;
using System.Collections.Generic;
using System.Linq;
using MET.Data.Models;
using MET.Domain.Logic;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.Tests.EngineTest
{
    [TestClass]
    public class OrchestratorTests
    {
        private static Orchestrator _orchestrator;

        List<Product> workOnList;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Log.ConfigureNLogForTests();
            _orchestrator = new Orchestrator(new AllPartNumbersDomain(), ZeroOutputFormatter.Instance, true);
            
            var met = Factory.GetMetProducts();
            var lama = Factory.GetLamaProducts();
            var td = Factory.GetTDProducts();
            var ab = Factory.GetABProducts();

            var list = new ProductLists();
            list.AddList(Providers.AB, ab);
            list.AddList(Providers.TechData, td);
            list.AddList(Providers.Lama, lama);

            _orchestrator.AddMetCollection(met);
            _orchestrator.SetCollections(list);

            var t = _orchestrator.Orchestrate();
            t.Wait();
        }

        [TestInitialize]
        public void Initialize()
        {
            FinalListCombineDomain listCombineDomain = new FinalListCombineDomain();
            var finalList = listCombineDomain.CreateFinalList(_orchestrator.GetGeneratedProductGroups());

            workOnList = new List<Product>(finalList);
        }

        //[TestMethod]
        //public void Test()
        //{
        //    var file = @"Repository\metproducts.json";
        //    JArray ja;
            
        //    using (var reader = new JsonTextReader(new StreamReader(file)))
        //    {
        //        ja = (JArray)JArray.ReadFrom(reader);

        //        foreach (var jt in ja)
        //        {
        //            var jo = (JObject)jt;
        //            var val = jt["SymbolSAP"].Value<string>();
        //            var newVal = MetProductReader.DecodeSapSymbol(val);
        //            jt["SymbolSAP"] = newVal;
        //        }
        //    }

        //    File.WriteAllText(file, ja.ToString(Formatting.Indented));
        //}


        // TODO implement
        //[TestMethod]
        //public void ValidateThatProductWithPriceErrorIsNotInWarehouse()
        //{
        //    var product = _workOnList
        //        .Single(r => r.SapManuHash == sapManuHashOfProductsWhichHasPriceError && r.Provider == Providers.AB);

        //    Assert.AreEqual(0, product.StanMagazynowy);
        //}

        [TestMethod]
        public void AllSelectedProductsWillHaveWarehousGreaterThanOne()
        {
            // Assert
            var result = ValidateList(workOnList, ValidateGroupForEmptyWarehouse);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SelectedProductMustBeCheapest()
        {
            // Assert
            var result = ValidateList(workOnList, ValidateGroupForCheapesProduct);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OnlyOneProductCanBeSelected()
        {
            // Assert
            var result = ValidateList(workOnList, ValidateGroupForOnlyOneSelectedGroup);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProductsWithEOLCanNotBeVisible()
        {
            // Assert
            var result = ValidateList(workOnList, ValidateGroupForEOLVisibility);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TechDataProductsWillHaveStatus0()
        {
            // Assert
            var result = ValidateList(workOnList, ValidateForTechDataEOL);
            Assert.IsTrue(result);
        }

        private bool ValidateForTechDataEOL(IEnumerable<Product> products)
        {
            foreach (var p in products)
            {
                if (p.Provider == Providers.TechData && p.OryginalnyKodProducenta.Contains("?TN"))
                {
                    if (p.Kategoria != "EOL_TN" || p.StatusProduktu || p.CenaNetto > 0 || p.CenaZakupuNetto > 0)
                        return false;
                }
            }

            return true;
        }

        private bool ValidateGroupForEOLVisibility(IList<Product> group)
        {
            return group.Where(p => p.Kategoria == EndOfLiveDomain.EndOfLifeCategory).All(p2 => !p2.StatusProduktu);
        }

        private bool ValidateGroupForEmptyWarehouse(IList<Product> group)
        {
            foreach (var p in group)
            {
                if (p.StatusProduktu && p.StanMagazynowy < 1)
                    return false;
            }

            return true;
        }

        private bool ValidateGroupForCheapesProduct(IList<Product> group)
        {
            var selected = group.FirstOrDefault(p => p.StatusProduktu);

            if (selected != null)
            {
                foreach (var p in group)
                {
                    if (!ReferenceEquals(p, selected))
                    {
                        if (selected.CenaNetto > p.CenaNetto)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool ValidateGroupForOnlyOneSelectedGroup(IList<Product> group)
        {
            return group.Count(p => p.StatusProduktu) <= 1;
        }

        private bool ValidateList(IEnumerable<Product> products, Func<IList<Product>, bool> validatingFunction)
        {
            var dict = CreateGrouppedDict(products);

            foreach (var list in dict)
            {
                if (!validatingFunction(list.Value))
                    return false;
            }

            return true;
        }

        private Dictionary<string, IList<Product>> CreateGrouppedDict(IEnumerable<Product> products)
        {
            var dict = new Dictionary<string, IList<Product>>();

            foreach (var p in products)
            {
                if (p.Provider == Providers.MET)
                    continue;

                if (dict.ContainsKey(p.PartNumber))
                {
                    dict[p.PartNumber].Add(p);
                }
                else
                {
                    dict.Add(p.PartNumber, new List<Product>() { p });
                }
            }

            return dict;
        }
    }
}
