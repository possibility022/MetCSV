using METCSV.Common;
using METCSV.WPF.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace METCSV.WPF.Workflows
{
    public class SettingsIo
    {
        private const int SaltLenght = 93;
        private const string SettingsFile = "settings.set";

        public static void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(JsonConvert.SerializeObject(App.Settings));
            AddSalt(ref sb);

            var encrypted = Encrypting.Encrypt(sb.ToString());
            File.WriteAllText(SettingsFile, encrypted);
        }

        public static void LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                StringBuilder sb = new StringBuilder();

                var content = File.ReadAllText(SettingsFile);

                content = Encrypting.Decrypt(content);

                sb.Append(content);
                sb = sb.Remove(content.Length - SaltLenght, SaltLenght);

                var settings = JsonConvert.DeserializeObject<Settings>(sb.ToString());
                SetEmptyIfNull(settings);
                App.Settings = settings;
            }
            else
            {
                App.Settings = new Settings();
            }
        }

        private static void AddSalt(ref StringBuilder sb)
        {
            var salt = RandomValues.RandomString(SaltLenght);
            sb.Append(salt);
        }

        private static void SetEmptyIfNull(Settings settings)
        {

            var settingsType = settings.GetType();
            var settingsProp = settingsType.GetProperties();

            foreach(var pset in settingsProp)
            {
                var val = pset.GetValue(settings);
                if (val != null)
                {
                    var t = val.GetType();
                    var tProp = t.GetProperties();

                    foreach (var p in tProp)
                    {
                        if (p.PropertyType == typeof(string) && p.GetValue(val) == null)
                        {
                            p.SetValue(val, string.Empty);
                        }
                    }
                }
            }
        }
    }
}
