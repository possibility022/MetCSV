using System.Collections.Generic;
using System.Windows.Forms;

namespace METCSV
{
    public partial class TestingForm : Form
    {
        public enum Provider
        {
            AB,
            Lama,
            TechData
        }

        public TestingForm()
        {
            InitializeComponent();
        }

        public void loadCategories(Dictionary<string, double> profits, Provider provider)
        {
            switch (provider)
            {
                case Provider.AB:
                    FillList(profits, this.setProfits_AB);
                    break;
                case Provider.Lama:
                    FillList(profits, this.setProfits_Lama);
                    break;
                case Provider.TechData:
                    FillList(profits, this.setProfits_TD);
                    break;
            }
        }

        private void FillList(Dictionary<string, double> profits, Forms.Controls.SetProfits profitControl)
        {
            foreach(string key in profits.Keys)
            {
                profitControl.listView1.Items.Add(new ListViewItem(new string[] { key, profits[key].ToString() }));
            }
        }

        public ListView.ListViewItemCollection getList(Provider provider)
        {
            switch(provider)
            {
                case Provider.AB:
                    return setProfits_AB.listView1.Items;

                case Provider.Lama:
                    return setProfits_Lama.listView1.Items;

                case Provider.TechData:
                    return setProfits_TD.listView1.Items;
            }

            return null;
        }
    }
}
