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
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// 
    public partial class SettingsWindow : Window
    {

        SettingsViewModel _model;

        SettingsViewModel Model { get => _model ?? (_model = (SettingsViewModel)DataContext); }



        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Model.Save();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Model.RestoreChanges();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Model.AllSaved())
            {
                var result = MessageBox.Show("Nie wszystkie zmiany zostały zapisane. Czy kontynułować?", "Uwaga", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
    }
}
