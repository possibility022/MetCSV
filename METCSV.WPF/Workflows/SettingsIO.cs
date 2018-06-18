using METCSV.Common;
using METCSV.WPF.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Workflows
{
    public class SettingsIO
    {
        private const int SaltLenght = 93;

        public static void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();

            

        }

        private static void AddSalt(ref StringBuilder sb)
        {
            var salt = RandomValues.RandomString(SaltLenght);
            sb.Append(salt);
        }
    }
}
