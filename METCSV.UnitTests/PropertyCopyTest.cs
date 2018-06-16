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

        ObjectToCopy A = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 55 };
        ObjectToCopy B = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 55 };
        ObjectToCopy C = new ObjectToCopy { Str = "321", Boolean = false, Integer = 55 };
        ObjectToCopy D = new ObjectToCopy { Str = "ABC", Boolean = true, Integer = 55 };
        ObjectToCopy E = new ObjectToCopy { Str = "ABC", Boolean = false, Integer = 10 };


        [TestMethod]
        public void AllAreEqual()
        {
            var result = PropertyCopy.AnyChanges(A, B);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StrIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(A, C);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BooleanIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(A, D);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IntIsDifferent()
        {
            var result = PropertyCopy.AnyChanges(A, E);

            Assert.IsTrue(result);
        }
    }
}
