using System.Windows;
using METCSV.WPF.Interfaces;

namespace METCSV.WPF.Views
{
    /// <summary>
    /// Interaction logic for PartNumberSearchWindow.xaml
    /// </summary>
    public partial class PartNumberSearchWindow : Window, IClosable
    {
        public PartNumberSearchWindow()
        {
            InitializeComponent();
        }
    }
}
