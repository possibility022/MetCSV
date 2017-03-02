namespace METCSV
{
    partial class TestingForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.tab_AB = new System.Windows.Forms.TabPage();
            this.setProfits_AB = new METCSV.Forms.Controls.SetProfits();
            this.tab_LAMA = new System.Windows.Forms.TabPage();
            this.setProfits_Lama = new METCSV.Forms.Controls.SetProfits();
            this.tab_TD = new System.Windows.Forms.TabPage();
            this.setProfits_TD = new METCSV.Forms.Controls.SetProfits();
            this.tabs.SuspendLayout();
            this.tab_AB.SuspendLayout();
            this.tab_LAMA.SuspendLayout();
            this.tab_TD.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tab_AB);
            this.tabs.Controls.Add(this.tab_LAMA);
            this.tabs.Controls.Add(this.tab_TD);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(441, 624);
            this.tabs.TabIndex = 0;
            // 
            // tab_AB
            // 
            this.tab_AB.Controls.Add(this.setProfits_AB);
            this.tab_AB.Location = new System.Drawing.Point(4, 22);
            this.tab_AB.Name = "tab_AB";
            this.tab_AB.Padding = new System.Windows.Forms.Padding(3);
            this.tab_AB.Size = new System.Drawing.Size(433, 598);
            this.tab_AB.TabIndex = 0;
            this.tab_AB.Text = "AB";
            this.tab_AB.UseVisualStyleBackColor = true;
            // 
            // setProfits_AB
            // 
            this.setProfits_AB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setProfits_AB.Location = new System.Drawing.Point(3, 3);
            this.setProfits_AB.Name = "setProfits_AB";
            this.setProfits_AB.Size = new System.Drawing.Size(427, 592);
            this.setProfits_AB.TabIndex = 0;
            // 
            // tab_LAMA
            // 
            this.tab_LAMA.Controls.Add(this.setProfits_Lama);
            this.tab_LAMA.Location = new System.Drawing.Point(4, 22);
            this.tab_LAMA.Name = "tab_LAMA";
            this.tab_LAMA.Padding = new System.Windows.Forms.Padding(3);
            this.tab_LAMA.Size = new System.Drawing.Size(433, 598);
            this.tab_LAMA.TabIndex = 1;
            this.tab_LAMA.Text = "Lama";
            this.tab_LAMA.UseVisualStyleBackColor = true;
            // 
            // setProfits_Lama
            // 
            this.setProfits_Lama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setProfits_Lama.Location = new System.Drawing.Point(3, 3);
            this.setProfits_Lama.Name = "setProfits_Lama";
            this.setProfits_Lama.Size = new System.Drawing.Size(427, 592);
            this.setProfits_Lama.TabIndex = 0;
            // 
            // tab_TD
            // 
            this.tab_TD.Controls.Add(this.setProfits_TD);
            this.tab_TD.Location = new System.Drawing.Point(4, 22);
            this.tab_TD.Name = "tab_TD";
            this.tab_TD.Size = new System.Drawing.Size(433, 598);
            this.tab_TD.TabIndex = 2;
            this.tab_TD.Text = "TechData";
            this.tab_TD.UseVisualStyleBackColor = true;
            // 
            // setProfits_TD
            // 
            this.setProfits_TD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setProfits_TD.Location = new System.Drawing.Point(0, 0);
            this.setProfits_TD.Name = "setProfits_TD";
            this.setProfits_TD.Size = new System.Drawing.Size(433, 598);
            this.setProfits_TD.TabIndex = 0;
            // 
            // TestingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 661);
            this.Controls.Add(this.tabs);
            this.Name = "TestingForm";
            this.Text = "TestingForm";
            this.tabs.ResumeLayout(false);
            this.tab_AB.ResumeLayout(false);
            this.tab_LAMA.ResumeLayout(false);
            this.tab_TD.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tab_AB;
        private Forms.Controls.SetProfits setProfits_AB;
        private System.Windows.Forms.TabPage tab_LAMA;
        private Forms.Controls.SetProfits setProfits_Lama;
        private System.Windows.Forms.TabPage tab_TD;
        private Forms.Controls.SetProfits setProfits_TD;
    }
}