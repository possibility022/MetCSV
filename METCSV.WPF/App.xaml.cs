using System.Windows;

namespace METCSV.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string[] Providers = new[] { "Lama", "AB", "TechData" };
        public static readonly string ProfitsFileExtension = ".prof";
    }
}
