namespace ZidUtilities.TesterWin
{
    partial class Form4
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
            this.SuspendLayout();
            // 
            // addressBar1
            // 
            this.addressBar1.BackColor = System.Drawing.Color.White;
            this.addressBar1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.addressBar1.CurrentNode = null;
            this.addressBar1.ForeColor = System.Drawing.SystemColors.Window;
            this.addressBar1.Location = new System.Drawing.Point(12, 12);
            this.addressBar1.MinimumSize = new System.Drawing.Size(331, 23);
            this.addressBar1.Name = "addressBar1";
            this.addressBar1.RootNode = null;
            this.addressBar1.SelectedStyle = ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline)));
            this.addressBar1.Size = new System.Drawing.Size(510, 23);
            this.addressBar1.TabIndex = 0;
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.addressBar1);
            this.Name = "Form4";
            this.Text = "Form4";
            this.ResumeLayout(false);

        }

        #endregion

        private CommonCode.Win.Controls.AddressBar.AddressBar addressBar1;
    }
}