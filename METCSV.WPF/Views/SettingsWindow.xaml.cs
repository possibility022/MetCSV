using METCSV.WPF.ViewModels;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace METCSV.WPF.Views
{

    /// <summary>
    /// Interaction logic for App.SETTINGS.xaml
    /// </summary>
    /// 
    public partial class SettingsWindow : Window
    {

        SettingsViewModel model;

        SettingsViewModel Model { get => model ?? (model = (SettingsViewModel)DataContext); }



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
                var result = MessageBox.Show("Masz niezapisane zmiany. Czy kontynułować?", "Uwaga", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
