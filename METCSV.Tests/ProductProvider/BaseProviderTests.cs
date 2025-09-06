using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MET.Data.Models;
using MET.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.ProductProvider
{
    [TestClass]
    public class BaseProductProviderTests
    {

        BaseProviderImplementation provider;
        CancellationTokenSource token;
        List<Product> products;

        const string TestFileName = "TestFilePrefix_23.03.2019.bin";

        [TestInitialize]
        public void TestInit()
        {
            token = new CancellationTokenSource();
            provider = new BaseProviderImplementation(token.Token);
            products = Factory.GetAbProducts();

            if (Directory.Exists(provider.Archive))
                Directory.Delete(provider.Archive, true);
        }

        private void CopyTestFileToArchive()
        {
            Directory.CreateDirectory(provider.Archive);
            var file = Path.Combine(provider.Archive, TestFileName);

            File.Copy(Path.Combine("Repository", "FilesWithDifferentCreationDate", TestFileName), file, true);

            DateTime yesterday;

            if (DateTime.Now.Day == 1)
            {
                if (DateTime.Now.Month == 1)
                    yesterday = new DateTime(DateTime.Now.Year, 12, 1);
                else
                    yesterday = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            }
            else
                yesterday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);

            File.SetLastWriteTime(file, yesterday);
        }

        [TestMethod]
        [Ignore]
        public void SerializeAndDeserialize()
        {
            // Act
            provider.SaveAsOldProducts(products);

            // Assert
            var file = $"{provider.Archive}\\{provider.FilePrefix}_{DateTime.Now.Date.ToString("d")}.bin";
            Assert.IsTrue(File.Exists(file));
        }

        [TestMethod]
        public void Deserialize()
        {
            // Arrange
            CopyTestFileToArchive();

            // Act
            var loadedList = provider.LoadOldProducts();

            // Assert
            Assert.AreEqual(products.Count, loadedList.Count);
        }

        [TestMethod]
        public void ReturnNullWhenFolderDoesNotExists()
        {
            // Arrange
            if (Directory.Exists(provider.Archive))
                Directory.Delete(provider.Archive, true);

            // Act
            var loadedList = provider.LoadOldProducts();

            // Assert
            Assert.IsNull(loadedList);

        }
    }
}
