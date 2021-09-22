using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MET.Data.Models;
using METCSV.WPF.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    public class PartNumberSearchWindowViewModel : BindableBase
    {

        public PartNumberSearchWindowViewModel()
        {
            collectionViewSource = new CollectionViewSource();
            products = new List<Product>();
            collectionViewSource.Source = products;
            collectionViewSource.Filter += CollectionViewSourceOnFilter;
            RefreshFilterCommand = new RelayCommand(() => this.collectionView.Refresh());

            SelectAndCloseCommand = new RelayCommand<IClosable>((window) =>
            {
                ExposeSelected();
                window.Close();
            });

            CloseCommand = new RelayCommand<IClosable>((window => window.Close()));
        }


        //private readonly ObservableCollection<Product> products;
        private readonly CollectionViewSource collectionViewSource;
        private ICollectionView collectionView;
        private readonly List<Product> products;
        private string textFilter;

        public IRelayCommand<IClosable> SelectAndCloseCommand { get; }
        public ICommand RefreshFilterCommand { get; }
        public IRelayCommand<IClosable> CloseCommand { get; }

        public ICollectionView CollectionView
        {
            get => collectionView;
            private set => SetProperty(ref collectionView, value);
        }

        public string TextFilter
        {
            get => textFilter;
            set => SetProperty(ref textFilter, value);
        }

        public void ClearProducts()
        {
            products.Clear();
        }

        public Product SelectedItem { get; private set; }

        private void ExposeSelected()
        {
            SelectedItem = collectionView.CurrentItem as Product;
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                this.products.Add(product);
            }

            collectionViewSource.Source = products;
            CollectionView = collectionViewSource.View;
        }

        private void CollectionViewSourceOnFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(TextFilter))
            {
                e.Accepted = true;
                return;
            }

            if (e.Item is Product product)
            {
                if (product.PartNumber?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.NazwaProduktu?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.NazwaDostawcy?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.UrlZdjecia?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.Kategoria?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.ModelProduktu?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.OryginalnyKodProducenta?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }

                if (product.CenaZakupuNetto.ToString()?.Contains(TextFilter) == true)
                {
                    e.Accepted = true;
                    return;
                }
            }

            e.Accepted = false;
        }

    }
}
