namespace ZidUtilities.TesterWin
{
    partial class Form2
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
            this.addressBar1 = new ZidUtilities.CommonCode.Win.Controls.AddressBar.AddressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.option1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.option2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondaryMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.only1OptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subOption1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subOption2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subOption3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addressBar1
            // 
            this.addressBar1.BackColor = System.Drawing.Color.White;
            this.addressBar1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.addressBar1.CurrentNode = null;
            this.addressBar1.ForeColor = System.Drawing.SystemColors.Window;
            this.addressBar1.Location = new System.Drawing.Point(196, 202);
            this.addressBar1.MinimumSize = new System.Drawing.Size(331, 23);
            this.addressBar1.Name = "addressBar1";
            this.addressBar1.RootNode = null;
            this.addressBar1.SelectedStyle = ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline)));
            this.addressBar1.Size = new System.Drawing.Size(510, 23);
            this.addressBar1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainMenuToolStripMenuItem,
            this.secondaryMenuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mainMenuToolStripMenuItem
            // 
            this.mainMenuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.option1ToolStripMenuItem,
            this.option2ToolStripMenuItem});
            this.mainMenuToolStripMenuItem.Name = "mainMenuToolStripMenuItem";
            this.mainMenuToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.mainMenuToolStripMenuItem.Text = "Main Menu";
            // 
            // option1ToolStripMenuItem
            // 
            this.option1ToolStripMenuItem.Name = "option1ToolStripMenuItem";
            this.option1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.option1ToolStripMenuItem.Text = "Option 1";
            // 
            // option2ToolStripMenuItem
            // 
            this.option2ToolStripMenuItem.Name = "option2ToolStripMenuItem";
            this.option2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.option2ToolStripMenuItem.Text = "Option 2";
            // 
            // secondaryMenuToolStripMenuItem
            // 
            this.secondaryMenuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.only1OptionToolStripMenuItem});
            this.secondaryMenuToolStripMenuItem.Name = "secondaryMenuToolStripMenuItem";
            this.secondaryMenuToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.secondaryMenuToolStripMenuItem.Text = "Secondary Menu";
            // 
            // only1OptionToolStripMenuItem
            // 
            this.only1OptionToolStripMenuItem.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.only1OptionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subOption1ToolStripMenuItem,
            this.subOption2ToolStripMenuItem,
            this.subOption3ToolStripMenuItem});
            this.only1OptionToolStripMenuItem.Name = "only1OptionToolStripMenuItem";
            this.only1OptionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.only1OptionToolStripMenuItem.Text = "Only 1 Option";
            // 
            // subOption1ToolStripMenuItem
            // 
            this.subOption1ToolStripMenuItem.Name = "subOption1ToolStripMenuItem";
            this.subOption1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.subOption1ToolStripMenuItem.Text = "Sub Option 1";
            // 
            // subOption2ToolStripMenuItem
            // 
            this.subOption2ToolStripMenuItem.Name = "subOption2ToolStripMenuItem";
            this.subOption2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.subOption2ToolStripMenuItem.Text = "Sub option 2";
            // 
            // subOption3ToolStripMenuItem
            // 
            this.subOption3ToolStripMenuItem.Name = "subOption3ToolStripMenuItem";
            this.subOption3ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.subOption3ToolStripMenuItem.Text = "Sub Option 3";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.addressBar1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form2";
            this.Text = "Form2";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonCode.Win.Controls.AddressBar.AddressBar addressBar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mainMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem option1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem option2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondaryMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem only1OptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subOption1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subOption2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subOption3ToolStripMenuItem;
    }
}