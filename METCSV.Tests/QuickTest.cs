using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class QuickTest
    {
        [TestMethod]
        public void TryTakeFromConcurrentDictionaryWillRemoveItem()
        {
            var a = new ConcurrentDictionary<string, string>();
            a.TryAdd("key", "val");

            var taken = a.TryGetValue("key", out string val);

            Assert.IsTrue(taken);

            var taken2 = a.TryGetValue("key", out string val2);
            Assert.IsTrue(taken2);
        }

        [TestMethod]
        public void TryTakeFromConcurrentBagWhenIsEmpty()
        {
            var a = new ConcurrentBag<string>();
            var taken = a.TryTake(out string str);
            Assert.IsFalse(taken);
        }

        [TestMethod]
        public void TryTakeWillOverrideOldValueOnOutValue()
        {
            var str = "someValue";
            Trace.WriteLine($"str: {str}");
            var a = new ConcurrentBag<string>();
            var taken = a.TryTake(out str);
            Trace.WriteLine($"str: {str}");
            Assert.IsFalse(taken);
            Assert.IsNull(str);
        }

        [TestMethod]
        public void TryTakeFromDictionaryWithIntAsKey()
        {
            var dict = new ConcurrentDictionary<int, int>();

            int outValue = -12;

            dict.TryGetValue(1, out outValue);
            Trace.Write(outValue);
        }

        [TestMethod]
        public void TryToAddSameKeyTwice()
        {
            var b = new byte();
            var dict = new ConcurrentDictionary<int, byte>();

            var results = dict.TryAdd(1, b);

            if (!results)
                Assert.Fail();

            for (int i = 0; i < 10; i++)
            {
                var tryAgain = dict.TryAdd(1, b);
                if (tryAgain)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod()
        {
            var requests = new[]
            {
                "priceList_s",
                "priceList_sDamage",
                "param",
                "priceList",
                "vyrobci",
                "strom",
            };

            foreach(var req in requests)
            {
                LamaDownload(req);
            }
        }

        [Ignore] // Enable for real test
        public void LamaDownload(string request)
        {
            var u = File.ReadAllText("C:\\temp\\lamau.txt");
            var p = File.ReadAllText("C:\\temp\\lamap.txt");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.lamaplus.com.pl/partner/export.php");

            var postData = "user=" + u;

            postData += $"&pass=" + p;
            postData += $"&request={request}";

            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            httpWebRequest.ContentLength = data.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                string message = $"Generwoanie licznika miedzy 14:00-16:00 jest niedostępne";
            }

            using (var responseStream = response.GetResponseStream())
            using (var streamWriter = new FileStream($"{request}.content", FileMode.Create))
            {
                byte[] buffer = new byte[2048];
                int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    streamWriter.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                }

                responseStream.Close();
            }
        }
    }
}
