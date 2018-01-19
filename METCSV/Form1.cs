using METCSV.Generator;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace METCSV
{
    public partial class Form1 : Form
    {
        static 

        object @lock = new object();
        List<Product> allProducts = new List<Product>();
        TimeSpan stoper;

        System.Threading.Thread loadingThread;

        public Form1()
        {
            InitializeComponent();

            //var reader = new FileSystem.ProductsReader();

            //var lama = reader.GetLamaProducts("LamaXml.xml", "LamaCSV.csv");
            //var techData = reader.GetTechDataProducts("TD_material.csv", "TD_Prices.csv");
            //var met = reader.GetMetProducts("csvmet.csv");
            Database.Log.Logging.addWriteHandler(addLogLine);
        }

        public void addLogLine(string message)
        {
            richTextBox1.Invoke((MethodInvoker)(() => { richTextBox1.Text += "\r\n" + message; }));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            loadingThread = new System.Threading.Thread(loadAll);
            loadingThread.Start();
        }

        private void loadAll()
        {
            DateTime startTime = DateTime.Now;
            Generator.AllOne allOne = new AllOne();
            Global.Result result = allOne.Load();

            DateTime endTime = DateTime.Now;

            lock (@lock)
            {
                this.allProducts = allOne.finalList;
                stoper = endTime.Subtract(startTime);
            }

            if (result == Global.Result.complete)
            {
                Database.Log.log("Koniec: " + stoper.ToString());

                FileSystem.Exporter.exportProducts("tmp.csv", this.allProducts);

                this.button2.Invoke((MethodInvoker)(() => { button2.Enabled = true; }));
            }
            else
            {
                Database.Log.log("Nie ukończono generowania - powstiał jakiś problem. " + stoper.ToString());
            }
        }


        private void export()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileSystem.Exporter.exportProducts(sfd.FileName, this.allProducts);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            export();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Global.ShowProfitsWindows = checkBox1.Checked;
        }
    }
}
