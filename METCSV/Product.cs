using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV
{
    [Serializable]
    public class Product
    {
        public Product()
        {
            StanMagazynowy = -1;
            CenaNetto = -1;
            CenaZakupuNetto = -1;
            UrlZdjecia = "";
        }

        public Nullable<int> ID { get; set; }
        public string SymbolSAP { get; set; }
        public string KodProducenta { get { return _kodProducenta; } }
        public string ModelProduktu { get { return _modelProduktu; } }
        public string OryginalnyKodProducenta {
            get { return _oryginalnyKodProducenta; }
            set {
                string _value = value;

                if (_value.Contains('#'))
                {
                    int indexOfHash = _value.IndexOf('#');
                    _value = _value.Remove(indexOfHash);
                }

                _oryginalnyKodProducenta = value;
                _modelProduktu = _value;
                _kodProducenta = _value;
            } }
        public string NazwaProduktu { get; set; }        
        public string KodDostawcy { get; set; }
        public string NazwaProducenta { get; set; }
        public string NazwaDostawcy { get; set; }
        public int StanMagazynowy { get; set; }
        public bool StatusProduktu { get; set; }
        public double CenaNetto { get; set; }
        public double CenaZakupuNetto { get; set; }
        public string UrlZdjecia { get; set; }
        public string Kategoria { get; set; }
        public bool Hidden { get; set; }

        string _kodProducenta;
        string _modelProduktu;
        string _oryginalnyKodProducenta;

        public string getLine()
        {
            return "\"" + ID + "\";\""
                + SymbolSAP + "\";\""
                + OryginalnyKodProducenta + "\";\""
                + OryginalnyKodProducenta + "\";\""
                + KodDostawcy + "\";\""
                + NazwaProduktu + "\";\""
                + NazwaProducenta + "\";\""
                + NazwaDostawcy + "\";\""
                + StanMagazynowy + "\";\""
                + ((StatusProduktu) ? "1" : "0") + "\";\""
                + CenaNetto + "\";\""
                + CenaZakupuNetto + "\";\""
                + ((Kategoria == "EOL") ? "EOL" : "") + "\"";
        }

        public string getHeader()
        {
            return "\"ID\";\"SymbolSAP\";\"KodProducenta\";\"ModelProduktu\";\"KodDostawcy\";\"NazwaProduktu\";\"NazwaProducenta\";\"NazwaDostawcy\";\"StanMagazynowy\";\"StatusProduktu\";\"CenaNetto\";\"CenaZakupuNetto\";\"Kategoria\"";
        }
    }
}
