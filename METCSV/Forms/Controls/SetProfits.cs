﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace METCSV.Forms.Controls
{
    public partial class SetProfits : UserControl
    {
        public SetProfits()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedItem();
        }

        private void selectedItem()
        {
            try
            {
                tbProvident.Text = listView1.SelectedItems[0].SubItems[0].Text;
                tbProfit.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
            catch (Exception ex) { }
        }

        private void btnSaveProfit_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].SubItems[1].Text = tbProfit.Text;
        }
    }
}
