using System.Linq;
using MET.Data.Models;
using MET.Domain;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ViewModels.ProfitsInnerModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Models
{
    [TestClass]
    public class ProfitsTests
    {
        [TestMethod]
        public void New_manufacturers_should_have_default_value()
        {
            // Arrange
            Profits profits = new Profits(Providers.AB);
            string[] allManufactureres = new string[] { "key1", "key2", "key3", "key4", "key5", "key6", "key7", "key8" };

            // Act

            profits.AddManufacturers(allManufactureres);

            // Assert
            Assert.IsTrue(
                profits.Values.All(v => v.Value == profits.DefaultProfit)
            );
        }

        [TestMethod]
        public void Adding_existsting_manufacturer_should_not_override_current_value()
        {
            // Arrange
            Profits profits = new Profits(Providers.AB);
            profits.SetNewProfits(new[] { new EditableDictionaryKey<string, double>("key", 22) });

            // Act
            profits.AddManufacturers(new[] { "key" });

            //Assert
            Assert.AreEqual(22, profits.Values.First().Value);
        }

        [TestMethod]
        public void Values_property_should_contains_all_manufacturers()
        {
            // Arrange
            Profits profits = new Profits(Providers.AB);
            string[] allManufactureres = new string[] { "key1"};

            // Act
            profits.SetNewProfits(new[] { new EditableDictionaryKey<string, double>("key", 22) });
            profits.AddManufacturers(allManufactureres);

            //Assert
            Assert.IsTrue(profits.Values.ContainsKey("key1"));
            Assert.IsTrue(profits.Values.ContainsKey("key"));
        }

        [TestMethod]
        public void Setting_new_values_override_old_values()
        {
            // Arrange
            Profits profits = new Profits(Providers.AB);
            profits.SetNewProfits(new[] { new EditableDictionaryKey<string, double>("key", 22) });

            // Act
            profits.SetNewProfits(new[] { new EditableDictionaryKey<string, double>("key", 33) });

            // Assert
            Assert.AreEqual(33, profits.Values["key"]);
        }

        [TestMethod]
        public void Setting_new_values_will_create_keys()
        {
            // Arrange
            Profits profits = new Profits(Providers.AB);

            // Act
            profits.SetNewProfits(new[] { new EditableDictionaryKey<string, double>("key", 22) });

            //Assert
            Assert.IsTrue(profits.Values.ContainsKey("key"));
        }
    }
}
