using System;

namespace MET.Data.Models
{
    [Serializable]
    public class Product
    {
        public Product(Providers provider)
        {
            StanMagazynowy = -1;
            CenaNetto = -1;
            CenaZakupuNetto = -1;
            UrlZdjecia = "";


            // ToDo w przyszłości powinieneś to odkomentować. Będzie to wymagało napisania własnego convertera JSON.
            //if (provider == Providers.None)
            //{
            //    throw new InvalidDataException("Product must have specified provider.");
            //}

            Provider = provider;
        }

        public Providers Provider { get; set; }

        public int? ID { get; set; }

        public string SymbolSAP { get => _symbolSAP; set { _symbolSAP = value; UpdateSapMenuHashSet(); } }

        public string KodProducenta { get; private set; } = string.Empty;

        public string ModelProduktu { get; private set; }

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
                ModelProduktu = _value;
                KodProducenta = _value;
                UpdateCodeAndManu();
            }
        }

        public string NazwaProduktu { get; set; } = string.Empty;

        public string KodDostawcy { get; set; }

        public string NazwaProducenta { get => _nazwaProducenta; set { _nazwaProducenta = value; UpdateSapMenuHashSet(); UpdateCodeAndManu(); } }

        public string NazwaDostawcy { get; set; }
        public string EAN { get; set; }

        public int StanMagazynowy { get; set; }

        public bool StatusProduktu { get; set; } = false;

        public double CenaNetto { get; private set; }

        public void SetCennaNetto(double value)
        {
            CenaNetto = value;
        }

        public double CenaZakupuNetto { get; set; }

        public string UrlZdjecia { get; set; }

        public string Kategoria { get; set; } = string.Empty;

        public bool Hidden { get; set; }



        /// <summary>
        /// Gets the sapnumber ^ manufacturer hash.
        /// </summary>
        /// <value>
        /// The sap manu hash.
        /// </value>
        public string SapManuHash { get; private set; }
        /// <summary>
        /// Gets the manufacturer code ^ manufacturer name.
        /// </summary>
        /// <value>
        /// The code and manu.
        /// </value>
        public string PartNumber { get; private set; }

        string _oryginalnyKodProducenta;
        private string _symbolSAP = string.Empty;
        private string _nazwaProducenta = string.Empty;

        private void UpdateSapMenuHashSet()
        {
            SapManuHash = $"{SymbolSAP}_||_{NazwaProducenta}";
        }

        private void UpdateCodeAndManu()
        {
            PartNumber = $"{KodProducenta.ToUpperInvariant()}_||_{NazwaProducenta.ToUpperInvariant()}";

            //_partNumber = (KodProducenta + NazwaProducenta).GetHashCode(); // OK, Here we have problem when KodProducetna is ABC and NazwaProducenta is XYZ then
            // part number will be the same as product where KodProducenta is AB and NazwaProducenta is AXYZ :(

            //_partNumber = KodProducenta.GetHashCode() ^ NazwaProducenta.GetHashCode(); // This soulution is bad
        }

        public override string ToString()
        {
            return $"{Provider} {OryginalnyKodProducenta} {SymbolSAP}";
        }
    }
}
