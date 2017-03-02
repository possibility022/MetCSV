namespace METCSV.Forms.Controls
{
    partial class SetProfits
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.Category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Profit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbProvident = new System.Windows.Forms.TextBox();
            this.tbProfit = new System.Windows.Forms.TextBox();
            this.btnSaveProfit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Category,
            this.Profit});
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(274, 557);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // Category
            // 
            this.Category.Text = "Kategoria";
            this.Category.Width = 175;
            // 
            // Profit
            // 
            this.Profit.Text = "Oprocentowanie";
            this.Profit.Width = 91;
            // 
            // tbProvident
            // 
            this.tbProvident.Location = new System.Drawing.Point(283, 3);
            this.tbProvident.Name = "tbProvident";
            this.tbProvident.Size = new System.Drawing.Size(141, 20);
            this.tbProvident.TabIndex = 2;
            // 
            // tbProfit
            // 
            this.tbProfit.Location = new System.Drawing.Point(283, 29);
            this.tbProfit.Name = "tbProfit";
            this.tbProfit.Size = new System.Drawing.Size(141, 20);
            this.tbProfit.TabIndex = 3;
            // 
            // btnSaveProfit
            // 
            this.btnSaveProfit.Location = new System.Drawing.Point(283, 55);
            this.btnSaveProfit.Name = "btnSaveProfit";
            this.btnSaveProfit.Size = new System.Drawing.Size(141, 23);
            this.btnSaveProfit.TabIndex = 4;
            this.btnSaveProfit.Text = "Zmień";
            this.btnSaveProfit.UseVisualStyleBackColor = true;
            this.btnSaveProfit.Click += new System.EventHandler(this.btnSaveProfit_Click);
            // 
            // SetProfits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSaveProfit);
            this.Controls.Add(this.tbProfit);
            this.Controls.Add(this.tbProvident);
            this.Controls.Add(this.listView1);
            this.Name = "SetProfits";
            this.Size = new System.Drawing.Size(429, 569);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColumnHeader Category;
        private System.Windows.Forms.ColumnHeader Profit;
        private System.Windows.Forms.TextBox tbProvident;
        private System.Windows.Forms.TextBox tbProfit;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnSaveProfit;
    }
}
