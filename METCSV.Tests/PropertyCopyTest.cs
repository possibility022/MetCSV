using METCSV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests
{

    class ObjectToCopy
    {
        public string Str { get; set; }
        public int Integer { get; set; }

        public bool Boolean { get; set; }
    }

    [TestClass]
    public class PropertyCopyTest
    {

        ObjectToCopy a;
        ObjectToCopy b;
        ObjectToCopy c;
        ObjectToCopy d;
        ObjectToCopy e;

        [TestInitialize]
        public void TestInitialize()
        {
            a = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 55 };
            b = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 55 };
            c = new ObjectToCopy { Str = "321", Boolean = false, Integer = 55 };
            d = new ObjectToCopy { Str = "ABC", Boolean = true, Integer = 55 };
            e = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 10 };
        }


        [TestMethod]
        public void AllAreEqual()
        {
            var result = PropertyCopy.AnyChanges(a, b);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StrIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(a, c);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BooleanIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(a, d);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IntIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(a, e);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CopyString()
        {
            PropertyCopy.CopyValues(a, c);

            Assert.AreEqual("ABC", c.Str);
        }

        [TestMethod]
        public void CopyBoolean()
        {
            PropertyCopy.CopyValues(a, d);

            Assert.AreEqual(false, d.Boolean);
        }

        [TestMethod]
        public void CopyInt()
        {
            PropertyCopy.CopyValues(a, e);

            Assert.AreEqual(55, e.Integer);
        }
    }
}
