using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ViewModels;
using METCSV.WPF.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
