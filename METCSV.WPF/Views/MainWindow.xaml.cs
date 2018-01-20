using METCSV.WPF.ViewModels;
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
    }
}
