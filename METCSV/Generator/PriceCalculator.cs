using System;
using System.Collections.Generic;
using System.Linq;

namespace METCSV.Generator
{
    class PriceCalculator
    {
        private double staticProfit = 1.1;

        Dictionary<string, double> profits =
            new Dictionary<string, double>();

        public PriceCalculator() { }

        public List<string> getProviders()
        { return profits.Keys.ToList(); }

        public Dictionary<string, double> getProfits()
        { return profits; }

        public List<string> loadProviders(List<Product> products, double staticProfit)
        {
            this.staticProfit = staticProfit;
               
            foreach(Product p in products)
            {
                addProvider(p.NazwaProducenta);
            }

            return profits.Keys.ToList();
        }

        private void addProvider(string provider)
        {
            if (provider != null)
                if (profits.ContainsKey(provider) == false)
                    profits.Add(provider, staticProfit);
        }

        public void setProfit(string provider, double value)
        {
            if (profits.ContainsKey(provider) == false)
            {
                throw new ArgumentException("Ta lista produktów nie posiada tej kategori.");
            }

            profits[provider] = value;
        }

        public void setProfits(Dictionary<string, double> profits)
        {
            this.profits = profits;
        }

        public string[] SaveProfitsToFileString()
        {
            string[] lines = new string[profits.Keys.Count];
            for(int i = 0; i < profits.Keys.Count; i++)
            {
                string key = profits.Keys.ToList()[i];
                lines[i] = key + "|" + profits[key].ToString();
            }

            return lines;
        }

        public void setProfit(string provider, string value)
        {
            double parsed_value = 0.0;
            value.Replace('.', ',');
            parsed_value = double.Parse(value);
            setProfit(provider, parsed_value);
        }

        public void LoadProfitsFrom_ListViewItemCollection(System.Windows.Forms.ListView.ListViewItemCollection collection)
        {
            foreach(System.Windows.Forms.ListViewItem item in collection)
            {
                setProfit(item.SubItems[0].Text, item.SubItems[1].Text);
            }
        }

        public void CountPrice(List<Product> products)
        {
            foreach(Product p in products)
            {
                p.CenaNetto = p.CenaZakupuNetto * profits[p.NazwaProducenta];
            }
        }
    }
}
