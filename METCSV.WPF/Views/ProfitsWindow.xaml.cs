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

        private void ButtonReloadProfits_Click(object sender, RoutedEventArgs e)
        {
            //ProfitsViewModel.LoadFromFiles(); //todo
        }

        private void ButtonSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Jesteś pewien?", "Zapis zastąpi obecne pliki lub je utworzy.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
