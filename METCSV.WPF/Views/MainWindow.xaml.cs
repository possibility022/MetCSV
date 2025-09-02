using System.Threading.Tasks;
using METCSV.WPF.ViewModels;
using METCSV.WPF.Views;
using Microsoft.Win32;
using System.Windows;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace METCSV.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainWindowViewModel _mainWindowViewModel;

        MainWindowViewModel MainWindowViewModel { get => _mainWindowViewModel; }

        private Task task;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowViewModel = (MainWindowViewModel)DataContext;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowProfitsWindow(object sender, RoutedEventArgs e)
        {

        }

        private void Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".csv";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MainWindowViewModel.Export(dialog.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel.Closing();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.Loaded();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.Stop();
        }
    }
}
