using System.Diagnostics;
using MET.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class ProductsTests
    {
        [TestMethod]
        public void PrintHashSet()
        {
            Product p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC"
            };

            Trace.WriteLine($"Nazwa Producenta: {p.NazwaProducenta}");
            Trace.WriteLine($"KodProducenta: {p.OryginalnyKodProducenta}");

            Trace.WriteLine(p.PartNumber);

            p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC",
                OryginalnyKodProducenta = "ABC"
            };

            Trace.WriteLine($"Nazwa Producenta: {p.NazwaProducenta}");
            Trace.WriteLine($"KodProducenta: {p.OryginalnyKodProducenta}");

            Trace.WriteLine(p.PartNumber);

            p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC2",
                OryginalnyKodProducenta = "ABC2"
            };

            Trace.WriteLine($"Nazwa Producenta: {p.NazwaProducenta}");
            Trace.WriteLine($"KodProducenta: {p.OryginalnyKodProducenta}");

            Trace.WriteLine(p.PartNumber);
        }

        [TestMethod]
        public void HavewSameManufacturersNamesAndSamePartNumber()
        {
            Product p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC"
            };

            var partNumber1 = p.PartNumber;

            p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC"
            };

            var partNumber2 = p.PartNumber;

            Assert.AreEqual(partNumber1, partNumber2);
        }


        [TestMethod]
        public void HaveSameValues()
        {
            Product p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC",
                OryginalnyKodProducenta = "XYZ"
            };

            var partNumber1 = p.PartNumber;

            p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC",
                OryginalnyKodProducenta = "XYZ"
            };

            var partNumber2 = p.PartNumber;

            Assert.AreEqual(partNumber1, partNumber2);
        }

        [TestMethod]
        public void HaveDifferentPartNumbers()
        {
            Product p = new Product(Providers.AB)
            {
                NazwaProducenta = "AB",
                OryginalnyKodProducenta = "CXYZ"
            };

            var partNumber1 = p.PartNumber;

            p = new Product(Providers.AB)
            {
                NazwaProducenta = "ABC",
                OryginalnyKodProducenta = "XYZ"
            };

            var partNumber2 = p.PartNumber;

            Assert.AreNotEqual(partNumber1, partNumber2);
        }

        
    }
}
