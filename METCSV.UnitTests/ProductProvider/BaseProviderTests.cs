using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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

        const string testFileName = "TestFilePrefix_23.03.2019.bin";

        [TestInitialize]
        public void TestInit()
        {
            token = new CancellationTokenSource();
            provider = new BaseProviderImplementation(token.Token);
            products = Factory.GetABProducts();

            if (Directory.Exists(provider.Archive))
                Directory.Delete(provider.Archive, true);
        }

        private void CopyTestFileToArchive()
        {
            Directory.CreateDirectory(provider.Archive);
            File.Copy(Path.Combine("Repository", "FilesWithDifferentCreationDate", testFileName), Path.Combine(provider.Archive, testFileName), true);
        }

        [TestMethod]
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
    }
}
