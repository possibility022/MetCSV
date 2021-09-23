using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using MET.Data.Models;
using MET.Domain.Logic.Models;
using METCSV.WPF.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    class BrowseAllProductsGroupsViewModel : BindableBase
    {

        public BrowseAllProductsGroupsViewModel() : base()
        {
            collectionViewSource = new CollectionViewSource();
            products = new List<ProductGroup>();
            collectionViewSource.Source = products;
            collectionViewSource.Filter += CollectionViewSourceOnFilter;
            RefreshFilterCommand = new RelayCommand(() => this.collectionView.Refresh());
            IsGridReadOnly = true;
        }

        private void CollectionViewSourceOnFilter(object sender, FilterEventArgs e)
        {
            var productGroup = (ProductGroup)e.Item;
            var results = ProductBrowserBaseViewModel.ProductSearchFilter(TextFilter, productGroup.FinalProduct);
            e.Accepted = results;
        }

        private readonly CollectionViewSource collectionViewSource;
        private string textFilter;
        private ICollectionView collectionView;
        private readonly List<ProductGroup> products;
        private bool isGridReadOnly;

        public ICommand RefreshFilterCommand { get; }

        public string TextFilter
        {
            get => textFilter;
            set => SetProperty(ref textFilter, value);
        }

        public ICollectionView CollectionView
        {
            get => collectionView;
            private set => SetProperty(ref collectionView, value);
        }

        public bool IsGridReadOnly
        {
            get => isGridReadOnly;
            set => SetProperty(ref isGridReadOnly, value);
        }

        public void AddProducts(IEnumerable<ProductGroup> products)
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
