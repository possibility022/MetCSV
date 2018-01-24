using METCSV.WPF.ViewModels;
using METCSV.WPF.Views;
using System.Windows;

namespace METCSV.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainWindowViewModel _mainWindowViewModel;

        MainWindowViewModel MainWindowViewModel { get => _mainWindowViewModel ?? (_mainWindowViewModel = (MainWindowViewModel)DataContext); }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.StartClick();
        }

        private void ShowProfitsWindow(object sender, RoutedEventArgs e)
        {
            ProfitsView view = new ProfitsView();
            view.Show();
        }
    }
}
