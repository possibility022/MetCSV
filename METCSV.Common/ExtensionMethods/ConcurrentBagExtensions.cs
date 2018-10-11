using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Common.ExtensionMethods
{
    public static class ConcurrentBagExtensions<T>
    {
        public static bool TryXTimesToAdd(this ConcurrentBag<T> collection, int retry = 10)
        {
            int i = 0;
            collection.a
        }
    }
}
