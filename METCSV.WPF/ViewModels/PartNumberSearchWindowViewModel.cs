using MET.Data.Models;
using METCSV.WPF.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;

namespace METCSV.WPF.ViewModels
{
    public class PartNumberSearchWindowViewModel : ProductBrowserBaseViewModel
    {

        public PartNumberSearchWindowViewModel()
        {
            SelectAndCloseCommand = new RelayCommand<IClosable>((window) =>
            {
                ExposeSelected();
                window.Close();
            });

            CloseCommand = new RelayCommand<IClosable>((window => window.Close()));
        }


        //private readonly ObservableCollection<Product> products;

        public IRelayCommand<IClosable> SelectAndCloseCommand { get; }
        public IRelayCommand<IClosable> CloseCommand { get; }
        
        public Product SelectedItem { get; private set; }

        private void ExposeSelected()
        {
            SelectedItem = CollectionView.CurrentItem as Product;
        }
        
    }
}
