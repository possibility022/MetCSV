using System.IO;
using MET.Proxy.Configuration;
using METCSV.Common;
using METCSV.WPF;
using METCSV.WPF.Configuration;
using METCSV.WPF.Workflows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.Workflows
{
    [TestClass]
    public class SettingsIOTests
    {

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists("settings.set"))
                File.Delete("settings.set");
        }

        [TestMethod]
        public void SettingsCannotBeNull()
        {
            // Arrange
            Settings settings = new Settings();

            var type = settings.GetType();
            var properties = type.GetProperties();

            // Act
            foreach(var p in properties)
            {
                var value = p.GetValue(settings);
                if (value == null)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void TestAB()
        {
            // Arrange
            Settings settings = new Settings();
            settings.AbSettings = new AbSettings()
            {
                DeleteOldMessages = true,
                EmailLogin = "EmailLogin",
                EmailPassword = "EmailPassword",
                EmailServerAddress = "ServerAddress",
                EmailServerPort = 222,
                EmailServerUseSSL = true,
                FolderToExtract = "FolderToExtract",
                ZippedFile = "ZipedFile"
            };

            App.Settings = settings;

            // Act
            SettingsIO.SaveSettings();
            SettingsIO.LoadSettings();

            // Assert
            Assert.IsFalse(PropertyCopy.AnyChanges(App.Settings.AbSettings, settings.AbSettings));
        }

        [TestMethod]
        public void TestMet()
        {
            // Arrange
            Settings settings = new Settings();
            settings.MetDownloaderSettings = new MetDownloaderSettings()
            {
                CsvFile = "CSV File",
                Url = "URL!"
            };

            App.Settings = settings;

            // Act
            SettingsIO.SaveSettings();
            SettingsIO.LoadSettings();

            // Assert
            Assert.IsFalse(PropertyCopy.AnyChanges(App.Settings.MetDownloaderSettings, settings.MetDownloaderSettings));
        }


        [TestMethod]
        public void TestTD()
        {
            // Arrange
            Settings settings = new Settings();
            settings.Td = new TechDataSettings()
            {
                CsvMaterials = "Materials",
                CsvPrices = "Prices",
                FolderToExtract = "FolderToExtract",
                FtpAddress = "FtpAddress",
                Login = "Loggiiinnnn",
                Password = "Password",
                Pattern = "Pattern"
            };

            App.Settings = settings;

            // Act
            SettingsIO.SaveSettings();
            SettingsIO.LoadSettings();

            // Assert
            Assert.IsFalse(PropertyCopy.AnyChanges(App.Settings.Td, settings.Td));
        }


        [TestMethod]
        public void TestLama()
        {
            // Arrange
            Settings settings = new Settings();
            settings.LamaSettings = new LamaSettings()
            {
                CsvFile = "CSV",
                Login = "Login",
                Password = "Password",
                Request = "Request",
                Url = "URL",
                XmlFile = "XML"
            };

            App.Settings = settings;

            // Act
            SettingsIO.SaveSettings();
            SettingsIO.LoadSettings();

            // Assert
            Assert.IsFalse(PropertyCopy.AnyChanges(App.Settings.LamaSettings, settings.LamaSettings));
        }

        [TestMethod]
        public void SetNullStringToEmptyString()
        {
            // Arrange
            App.Settings = new Settings();
            App.Settings.MetDownloaderSettings.Url = null;

            SettingsIO.SaveSettings();

            // Act
            SettingsIO.LoadSettings();
            
            // Assert
            Assert.IsNotNull(App.Settings.MetDownloaderSettings.Url);
        }
    }
}
