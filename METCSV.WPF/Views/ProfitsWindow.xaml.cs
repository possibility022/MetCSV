using METCSV.WPF.ViewModels;
using System.Windows;

namespace METCSV.WPF.Views
{
    /// <summary>
    /// Interaction logic for ProfitsView.xaml
    /// </summary>
    public partial class ProfitsWindow : Window
    {

        ProfitsViewModel _profitsViewModel;

        ProfitsViewModel ProfitsViewModel { get => _profitsViewModel ?? (_profitsViewModel = (ProfitsViewModel)DataContext); }

        public ProfitsWindow()
        {
            InitializeComponent();
        }

        private void ButtonSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Jesteś pewien?", "Zapis zastąpi obecne pliki lub je utworzy.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ProfitsViewModel.Save = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ProfitsViewModel.Save = false;
            Close();
        }
    }
}
