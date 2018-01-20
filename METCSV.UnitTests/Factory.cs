using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.UnitTests
{
    class Factory
    {

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();

        public static HashSet<string> GenerateUniqueSapNumbers(int count)
        {
            HashSet<string> sapNumbers = new HashSet<string>();

            while(sapNumbers.Count < count)
            {
                sapNumbers.Add(RandomString(7));
            }

            return sapNumbers;
        }

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
