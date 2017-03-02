namespace METCSV.Forms
{
    partial class GroupController
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
            this.lvProductsInGroup = new System.Windows.Forms.ListView();
            this.lvProductsToSolve = new System.Windows.Forms.ListView();
            this.lvGroups = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.productViewer2 = new METCSV.Forms.Controls.ProductViewer();
            this.productViewer1 = new METCSV.Forms.Controls.ProductViewer();
            this.SuspendLayout();
            // 
            // lvProductsInGroup
            // 
            this.lvProductsInGroup.HideSelection = false;
            this.lvProductsInGroup.Location = new System.Drawing.Point(81, 12);
            this.lvProductsInGroup.Name = "lvProductsInGroup";
            this.lvProductsInGroup.Size = new System.Drawing.Size(243, 411);
            this.lvProductsInGroup.TabIndex = 0;
            this.lvProductsInGroup.UseCompatibleStateImageBehavior = false;
            this.lvProductsInGroup.View = System.Windows.Forms.View.List;
            this.lvProductsInGroup.SelectedIndexChanged += new System.EventHandler(this.lvProductsInGroup_SelectedIndexChanged);
            this.lvProductsInGroup.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvProductsInGroup_MouseDoubleClick);
            // 
            // lvProductsToSolve
            // 
            this.lvProductsToSolve.HideSelection = false;
            this.lvProductsToSolve.Location = new System.Drawing.Point(551, 12);
            this.lvProductsToSolve.Name = "lvProductsToSolve";
            this.lvProductsToSolve.Size = new System.Drawing.Size(215, 411);
            this.lvProductsToSolve.TabIndex = 1;
            this.lvProductsToSolve.UseCompatibleStateImageBehavior = false;
            this.lvProductsToSolve.View = System.Windows.Forms.View.List;
            this.lvProductsToSolve.SelectedIndexChanged += new System.EventHandler(this.lvProductsToSolve_SelectedIndexChanged);
            this.lvProductsToSolve.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvProductsToSolve_MouseDoubleClick);
            // 
            // lvGroups
            // 
            this.lvGroups.HideSelection = false;
            this.lvGroups.Location = new System.Drawing.Point(12, 12);
            this.lvGroups.Name = "lvGroups";
            this.lvGroups.Size = new System.Drawing.Size(63, 411);
            this.lvGroups.TabIndex = 2;
            this.lvGroups.UseCompatibleStateImageBehavior = false;
            this.lvGroups.View = System.Windows.Forms.View.List;
            this.lvGroups.SelectedIndexChanged += new System.EventHandler(this.lvGroups_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(373, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = ">>>>>";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(373, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "<<<<<";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(373, 75);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(127, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Nowa Grupa";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // productViewer2
            // 
            this.productViewer2.Location = new System.Drawing.Point(422, 429);
            this.productViewer2.Name = "productViewer2";
            this.productViewer2.Size = new System.Drawing.Size(344, 399);
            this.productViewer2.TabIndex = 7;
            // 
            // productViewer1
            // 
            this.productViewer1.Location = new System.Drawing.Point(12, 429);
            this.productViewer1.Name = "productViewer1";
            this.productViewer1.Size = new System.Drawing.Size(344, 399);
            this.productViewer1.TabIndex = 6;
            // 
            // GroupController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 820);
            this.Controls.Add(this.productViewer2);
            this.Controls.Add(this.productViewer1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lvGroups);
            this.Controls.Add(this.lvProductsToSolve);
            this.Controls.Add(this.lvProductsInGroup);
            this.Name = "GroupController";
            this.Text = "GroupMaker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvProductsInGroup;
        private System.Windows.Forms.ListView lvProductsToSolve;
        private System.Windows.Forms.ListView lvGroups;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private Controls.ProductViewer productViewer1;
        private Controls.ProductViewer productViewer2;
    }
}