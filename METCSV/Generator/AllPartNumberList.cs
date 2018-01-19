using System.Collections.Generic;

namespace METCSV.Generator
{
    class AllPartNumberList
    {
        private List<string> partNumbers;

        List<Product> lama;
        List<Product> techData;
        List<Product> METproducts;
        List<Product> ab;

        bool generated = false;

        public AllPartNumberList(ref List<Product> lama, ref List<Product> techData, ref List<Product> ab, ref List<Product> METproducts)
        {
            partNumbers = new List<string>();

            this.lama = lama;
            this.techData = techData;
            this.METproducts = METproducts;
            this.ab = ab;
        }

        public AllPartNumberList(List<Product> lama, List<Product> techData, List<Product> ab)
        {
            partNumbers = new List<string>();

            this.lama = lama;
            this.techData = techData;
            this.ab = ab;
        }

        private void buildPartNumberList()
        {
            Database.Log.Logging.log_message("Budowanie listy part numberów z lamy");
            foreach (Product p in lama)
                addIfDoesNotExists(p.KodProducenta, ref partNumbers);

            Database.Log.Logging.log_message("Budowanie listy part numberów z techdaty");
            foreach (Product p in techData)
                addIfDoesNotExists(p.KodProducenta, ref partNumbers);

            Database.Log.Logging.log_message("Budowanie listy part numberów z ab");
            foreach (Product p in ab)
                addIfDoesNotExists(p.KodProducenta, ref partNumbers);

            Database.Log.Logging.log_message("Budowanie listy part numberów z metu");
            if (METproducts != null)
                foreach (Product p in METproducts)
                    addIfDoesNotExists(p.KodProducenta, ref partNumbers);

            generated = true;
        }

        private void addIfDoesNotExists(string what, ref List<string> targetList)
        {
            if (targetList.Contains(what) == false)
                targetList.Add(what);
        }

        /// <summary>
        /// Zwraca listę kodów producenta. Każdy element w liście jest unikalny (nie powtarza się).
        /// </summary>
        /// <returns></returns>
        public List<string> getPartNumbers()
        {
            if (this.generated == false)
            {
                buildPartNumberList();
            }
            return partNumbers;
        }
    }
}
