using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using MET.Data.Models;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    public class PartNumberSearchWindowViewModel : BindableBase
    {

        public PartNumberSearchWindowViewModel()
        {
            collectionViewSource = new CollectionViewSource();
            products = new ObservableCollection<Product>();
            collectionViewSource.Source = products;
        }

        //private readonly ObservableCollection<Product> products;
        private readonly CollectionViewSource collectionViewSource;
        private ICollectionView collectionView;
        private ObservableCollection<Product> products;

        public ObservableCollection<Product> ProductsABC
        {
            get => products;
            set => SetProperty(ref products, value);
        }

        public ICollectionView CollectionView
        {
            get => collectionView;
            private set => SetProperty(ref collectionView, value);
        }

        public void SetProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                this.products.Add(product);
            }

            collectionViewSource.Source = products;
            CollectionView = collectionViewSource.View;
        }
    }
}
