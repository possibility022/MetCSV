using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using MET.Data.Models;
using Microsoft.Toolkit.Mvvm.Input;

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
            e.Accepted = ProductSearchFilter(TextFilter, (Product)e.Item);
        }

        public static bool ProductSearchFilter(string filter, Product product)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }


            if (product.PartNumber?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.NazwaProduktu?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.NazwaProducenta?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.NazwaDostawcy?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.UrlZdjecia?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.Kategoria?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.ModelProduktu?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.OryginalnyKodProducenta?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            if (product.CenaZakupuNetto.ToString()?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }
    }

}
