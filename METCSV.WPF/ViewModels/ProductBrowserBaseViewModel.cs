using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MET.Data.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    public abstract class ProductBrowserBaseViewModel : BindableBase
    {

        protected ProductBrowserBaseViewModel()
        {
            collectionViewSource = new CollectionViewSource();
            products = new List<Product>();
            collectionViewSource.Source = products;
            collectionViewSource.Filter += CollectionViewSourceOnFilter;
            RefreshFilterCommand = new RelayCommand(() => this.collectionView.Refresh());
            IsGridReadOnly = true;
        }

        private readonly CollectionViewSource collectionViewSource;
        private string textFilter;
        private ICollectionView collectionView;
        private readonly List<Product> products;
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
