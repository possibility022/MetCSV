using System;
using System.Diagnostics;
using System.Globalization;
using MET.Data.Models;
using MET.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class ProductsTests
    {

        [TestMethod]
        public void CultureInfoTest()
        {
            var emailMetadata = "Tue, 24 Jul 2018 07:31:26";

            var d = DateTime.ParseExact(emailMetadata, "r", CultureInfo.CreateSpecificCulture("en-US"));

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var dateTime = DateTime.Now;

            var cu = CultureInfo.CreateSpecificCulture("en-US");
            Console.WriteLine(DateTime.Now.ToString(cu));

            foreach (var cultureInfo in cultures)
            {
                Console.WriteLine(cultureInfo.DateTimeFormat.FullDateTimePattern);
                Console.WriteLine(cultureInfo);
                Console.WriteLine(dateTime.ToString("r", cultureInfo));
                Console.WriteLine();
            }

            

        }

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
