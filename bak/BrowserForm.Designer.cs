
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
            this.browserPanel = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MaxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // browserPanel
            // 
            this.browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserPanel.Location = new System.Drawing.Point(0, 24);
            this.browserPanel.Name = "browserPanel";
            this.browserPanel.Size = new System.Drawing.Size(1028, 707);
            this.browserPanel.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaxToolStripMenuItem,
            this.MinToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1028, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MaxToolStripMenuItem
            // 
            this.MaxToolStripMenuItem.Name = "MaxToolStripMenuItem";
            this.MaxToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.MaxToolStripMenuItem.Text = "拡大";
            this.MaxToolStripMenuItem.Click += new System.EventHandler(this.MaxToolStripMenuItem_Click);
            // 
            // MinToolStripMenuItem
            // 
            this.MinToolStripMenuItem.Name = "MinToolStripMenuItem";
            this.MinToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.MinToolStripMenuItem.Text = "縮小";
            this.MinToolStripMenuItem.Click += new System.EventHandler(this.MinToolStripMenuItem_Click);
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(500, 500);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Controls.Add(this.browserPanel);
            this.Controls.Add(this.menuStrip1);
            this.Name = "BrowserForm";
            this.ShowInTaskbar = false;
            this.Text = "gesobrowser";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowserForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

    }
}