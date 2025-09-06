using System;

namespace MET.Data.Models
{
    [Serializable]
    public class Product : ICheapestProductDomain
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

        public int? Id { get; set; }

        public string SymbolSap { get => symbolSap; set { symbolSap = value; UpdateSapMenuHashSet(); } }

        public string KodProducenta { get; private set; } = string.Empty;

        public string ModelProduktu { get; private set; }

        public Providers OriginalSource { get; set; }

        public string OryginalnyKodProducenta
        {
            get { return oryginalnyKodProducenta; }
            set
            {
                string v = value;

                if (v.Contains('#'))
                {
                    int indexOfHash = v.IndexOf('#');
                    v = v.Remove(indexOfHash);
                }

                oryginalnyKodProducenta = v;
                ModelProduktu = v;
                KodProducenta = v;
                UpdateCodeAndManu();
            }
        }

        public string NazwaProduktu { get; set; } = string.Empty;

        public string KodDostawcy { get; set; }

        public string NazwaProducenta { get => nazwaProducenta; set { nazwaProducenta = value; UpdateSapMenuHashSet(); UpdateCodeAndManu(); } }

        public string NazwaDostawcy { get; set; }
        public string Ean { get; set; }

        public int StanMagazynowy { get; set; }

        public bool StatusProduktu { get; set; } = false;

        public double CenaNetto { get; set; }

        public void SetCennaNetto(double value)
        {
            CenaNetto = value;
        }

        public double CenaZakupuNetto { get; set; }

        public string UrlZdjecia { get; set; }

        public string Kategoria { get; set; } = string.Empty;

        public bool Hidden { get; set; }

        public bool? EndOfLive { get; set; } = null;



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

        string oryginalnyKodProducenta;
        private string symbolSap = string.Empty;
        private string nazwaProducenta = string.Empty;

        private void UpdateSapMenuHashSet()
        {
            SapManuHash = $"{SymbolSap}_||_{NazwaProducenta}";
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
            return $"{Provider} {OryginalnyKodProducenta} {SymbolSap}";
        }
    }

    public interface ICheapestProductDomain
    {
        double CenaNetto { get; }

        int StanMagazynowy { get; }
    }
}
