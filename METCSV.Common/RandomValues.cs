﻿using System;
using System.Linq;

namespace METCSV.Common
{
    public static class RandomValues
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
