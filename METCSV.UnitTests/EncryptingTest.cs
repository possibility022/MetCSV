using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{
    [TestClass]
    public class EncryptingTest
    {
        [TestMethod]
        public void EncryptingAndDectrypting()
        {
            const string text = "TESTABC123@#!$";
            var encrypted = Encrypting.Encrypt(text);
            var decrypted = Encrypting.Decrypt(encrypted);

            Assert.AreEqual(text, decrypted);
        }
    }
}
