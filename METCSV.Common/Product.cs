using System;
using System.Linq;

namespace METCSV.Common
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
        public string SymbolSAP { get => _symbolSAP; set { _symbolSAP = value; UpdateSapMenuHashSet(); } }
        public string KodProducenta { get { return _kodProducenta; } }
        public string ModelProduktu { get { return _modelProduktu; } }
        public string OryginalnyKodProducenta
        {
            get { return _oryginalnyKodProducenta; }
            set
            {
                string _value = value;

                if (_value.Contains('#'))
                {
                    int indexOfHash = _value.IndexOf('#');
                    _value = _value.Remove(indexOfHash);
                }

                _oryginalnyKodProducenta = value;
                _modelProduktu = _value;
                _kodProducenta = _value;
                UpdateCodeAndManu();
            }
        }
        public string NazwaProduktu { get; set; }
        public string KodDostawcy { get; set; }
        public string NazwaProducenta { get => _nazwaProducenta; set { _nazwaProducenta = value; UpdateSapMenuHashSet(); UpdateSapMenuHashSet(); } }
        public string NazwaDostawcy { get; set; }
        public int StanMagazynowy { get; set; }
        public bool StatusProduktu { get; set; } = false;
        public double CenaNetto { get; set; }
        public double CenaZakupuNetto { get; set; }
        public string UrlZdjecia { get; set; }
        public string Kategoria { get; set; }
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets the sapnumber ^ manufacturer hash.
        /// </summary>
        /// <value>
        /// The sap manu hash.
        /// </value>
        public int SapManuHash { get => _sapManuHashSet; private set => _sapManuHashSet = value; }
        /// <summary>
        /// Gets the manufacturer code ^ manufacturer name.
        /// </summary>
        /// <value>
        /// The code and manu.
        /// </value>
        public int PartNumber { get => _partNumber; private set => _partNumber = value; }

        string _kodProducenta;
        string _modelProduktu;
        string _oryginalnyKodProducenta;
        private int _sapManuHashSet;
        private string _symbolSAP = string.Empty;
        private string _nazwaProducenta = string.Empty;
        private int _partNumber;

        private void UpdateSapMenuHashSet()
        {
            _sapManuHashSet = SymbolSAP.GetHashCode() ^ NazwaProducenta.GetHashCode();
        }

        private void UpdateCodeAndManu()
        {
            _partNumber = KodProducenta.GetHashCode() ^ NazwaProducenta.GetHashCode();
        }

        public string GetLine() //todo remove
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
                + UrlZdjecia + "\";\""
                + ((Kategoria == "EOL") ? "EOL" : "") + "\"";
        }

        public static string GetHeaders() //todo remove
        {
            return "\"ID\";\"SymbolSAP\";\"KodProducenta\";\"ModelProduktu\";\"KodDostawcy\";\"NazwaProduktu\";\"NazwaProducenta\";\"NazwaDostawcy\";\"StanMagazynowy\";\"StatusProduktu\";\"CenaNetto\";\"CenaZakupuNetto\";\"UrlZdjecia\";\"Kategoria\"";
        }
    }
}
