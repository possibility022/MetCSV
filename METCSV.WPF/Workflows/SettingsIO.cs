using METCSV.Common;
using METCSV.WPF.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace METCSV.WPF.Workflows
{
    public class SettingsIO
    {
        private const int SaltLenght = 93;
        private const string SettingsFile = "settings.set";

        public static void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(JsonConvert.SerializeObject(App.Settings));
            AddSalt(ref sb);

            Encrypting.Encrypt(sb.ToString());
            File.WriteAllText(SettingsFile, sb.ToString());
        }

        public static void LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                StringBuilder sb = new StringBuilder();

                var content = File.ReadAllText(SettingsFile);

                sb.Append(content);
                sb = sb.Remove(content.Length - SaltLenght, SaltLenght);

                App.Settings = JsonConvert.DeserializeObject<Settings>(sb.ToString());
            }

        }

        private static void AddSalt(ref StringBuilder sb)
        {
            var salt = RandomValues.RandomString(SaltLenght);
            sb.Append(salt);
        }
    }
}
