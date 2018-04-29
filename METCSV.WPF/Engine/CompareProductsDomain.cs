using System.Collections.Concurrent;
using System.Collections.Generic;

namespace METCSV.WPF.Engine
{
    class CompareProductsDomain
    {

        ConcurrentBag<int> _allPartNumbers;

        public CompareProductsDomain(ISet<int> allManuCodeAndManuList)
        {
            _allPartNumbers = new ConcurrentBag<int>(allManuCodeAndManuList);
        }
    }
}
