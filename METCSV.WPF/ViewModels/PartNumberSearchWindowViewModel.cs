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
        private readonly ObservableCollection<Product> products;

        public ICollectionView CollectionView
        {
            get => collectionView;
            private set => SetProperty(ref collectionView, value);
        }

        public void SetProducts(IEnumerable<Product> products)
        {
            this.products.Clear();

            foreach (var product in products)
            {
                this.products.Add(product);
            }

            collectionViewSource.Source = products;
            CollectionView = collectionViewSource.View;
        }
    }
}
