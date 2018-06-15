using METCSV.WPF.ViewModels;
using METCSV.WPF.Views;
using Microsoft.Win32;
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            MainWindowViewModel.StartClickAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void ShowProfitsWindow(object sender, RoutedEventArgs e)
        {

        }

        private void Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                MainWindowViewModel.Export(dialog.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var w = new SettingsWindow();
            w.Show();
        }
    }
}
