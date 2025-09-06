using METCSV.WPF.ViewModels;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel.Closing();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.Loaded();
        }
    }
}
