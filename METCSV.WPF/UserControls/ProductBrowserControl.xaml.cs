using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using MET.Data.Models;
using UserControl = System.Windows.Controls.UserControl;

namespace METCSV.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ProductBrowser.xaml
    /// </summary>
    public partial class ProductBrowser : UserControl
    {

        public static readonly DependencyProperty ProductsProperty =
            DependencyProperty.Register("Products", typeof(ObservableCollection<Product>),
                typeof(ProductBrowser), new FrameworkPropertyMetadata(new ObservableCollection<Product>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(StatusChanged)));

        public ObservableCollection<Product> Products
        {
            get { return (ObservableCollection<Product>)GetValue(ProductsProperty); }
            set { SetValue(ProductsProperty, value); }
        }

        public CollectionViewSource CollectionViewSource { get; set; } = new CollectionViewSource();

        static void StatusChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ((ProductBrowser)property).ChangeControl((ObservableCollection<Product>)args.NewValue);
        }

        public void ChangeControl(ObservableCollection<Product> products)
        {
            CollectionViewSource.Source = products;
        }

        public ProductBrowser()
        {
            InitializeComponent();
        }
    }
}
