using METCSV.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace METCSV.WPF.Views
{
    /// <summary>
    /// Interaction logic for ProfitsView.xaml
    /// </summary>
    public partial class ProfitsView : Window
    {

        ProfitsViewModel _profitsViewModel;

        ProfitsViewModel ProfitsViewModel { get => _profitsViewModel ?? (_profitsViewModel = (ProfitsViewModel)DataContext); }

        public ProfitsView()
        {
            InitializeComponent();
        }

        private void ButtonReloadProfits_Click(object sender, RoutedEventArgs e)
        {
            ProfitsViewModel.LoadFromFiles();
        }

        private void ButtonSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Jesteś pewien?", "Zapis zastąpi obecne pliki lub je utworzy.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ProfitsViewModel.SaveAllProfits();
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
