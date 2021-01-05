
namespace gesobrowser
{
    partial class BrowserForm
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

		private void InitializeComponent()
		{
//            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.browserPanel = new System.Windows.Forms.Panel();
            this.browserPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            /*            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                        this.comboBox1.FormattingEnabled = true;
                        this.comboBox1.Items.AddRange(new object[] {
                        "100",
                        "90",
                        "80",
                        "70",
                        "60",
                        "50",
                        "40",
                        "30",
                        "20",
                        "10",
                        "0",
                        "-10",
                        "-20",
                        "-30",
                        "-40",
                        "-50",
                        "-60",
                        "-70",
                        "-80",
                        "-90",
                        "-100"});
                        this.comboBox1.Location = new System.Drawing.Point(-3, -2);
                        this.comboBox1.Name = "comboBox1";
                        this.comboBox1.Size = new System.Drawing.Size(89, 20);
                        this.comboBox1.TabIndex = 3;
                        this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            */            // 
                          // tabControl1
                          // 
                          //            this.tabControl1.ItemSize = new System.Drawing.Size(0, 0);
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(206, 240);
//            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 2;
            this.tabControl1.AutoSize = true;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.DrawMode= System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            // 
            // browserPanel
            // 
            this.browserPanel.AutoSize = true;
            this.browserPanel.Controls.Add(this.tabControl1);
            this.browserPanel.Location = new System.Drawing.Point(-3, 14);
            this.browserPanel.Name = "browserPanel";
            this.browserPanel.Size = new System.Drawing.Size(502, 474);
            this.browserPanel.TabIndex = 1;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.browserPanel);
//            this.Controls.Add(this.comboBox1);
            this.Name = "BrowserForm";
            this.ShowInTaskbar = false;
            this.Text = "gesobrowser";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowserForm_FormClosing);
            this.browserPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

//        private System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Panel browserPanel;
    }
}