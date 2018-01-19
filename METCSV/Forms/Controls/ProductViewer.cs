using System.Windows.Forms;

namespace METCSV.Forms.Controls
{
    public partial class ProductViewer : UserControl
    {
        public ProductViewer()
        {
            InitializeComponent();
        }

        public void loadProduct(Product p)
        {
            tbID.Text = p.ID.ToString();
            tbSAP.Text = p.SymbolSAP;
            tbManufacturerCode.Text = p.KodProducenta;
            tbProductModel.Text = p.ModelProduktu;
            tbOrgManufacturerCode.Text = p.OryginalnyKodProducenta;
            tbProductName.Text = p.NazwaProduktu;
            tbProviderCode.Text = p.KodDostawcy;
            tbManufacturerName.Text = p.NazwaProducenta;
            tbProviderName.Text = p.NazwaDostawcy;
            tbWarehouseStatus.Text = p.StanMagazynowy.ToString();
            tbProductStatus.Text = p.StatusProduktu.ToString();
            tbPriceNetto.Text = p.CenaNetto.ToString();
            tbPriceNetto_buying.Text = p.CenaZakupuNetto.ToString();
            tbUrl.Text = p.UrlZdjecia;
            tbCategory.Text = p.Kategoria;
        }

        public void clear()
        {
            foreach (Control tx in this.Controls)
            {
                if (tx is TextBox)
                {
                    TextBox txxx = (TextBox)tx;
                    txxx.Text = "";
                }
            }
        }


        //public string KodDostawcy { get; set; }
        //public string NazwaProducenta { get; set; }
        //public string NazwaDostawcy { get; set; }
        //public int StanMagazynowy { get; set; }
        //public bool StatusProduktu { get; set; }
        //public double CenaNetto { get; set; }
        //public double CenaZakupuNetto { get; set; }
        //public string UrlZdjecia { get; set; }
        //public string Kategoria { get; set; }
    }
}
