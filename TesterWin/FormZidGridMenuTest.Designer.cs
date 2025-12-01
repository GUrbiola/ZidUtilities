namespace ZidUtilities.TesterWin
{
    partial class FormZidGridMenuTest
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
            this.zidGrid1 = new ZidUtilities.CommonCode.Win.Controls.Grid.ZidGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // zidGrid1
            //
            this.zidGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zidGrid1.CellFont = new System.Drawing.Font("Verdana", 9F);
            this.zidGrid1.DataSource = null;
            this.zidGrid1.EnableAlternatingRows = true;
            this.zidGrid1.Location = new System.Drawing.Point(12, 70);
            this.zidGrid1.Name = "zidGrid1";
            this.zidGrid1.Size = new System.Drawing.Size(760, 368);
            this.zidGrid1.TabIndex = 0;
            this.zidGrid1.Theme = ZidUtilities.CommonCode.Win.ZidThemes.None;
            this.zidGrid1.TitleFont = new System.Drawing.Font("Verdana", 9.25F, System.Drawing.FontStyle.Bold);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(580, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "ZidGrid Header Context Menu Test\r\nRight-click on any column header to see the me" +
    "nu with plugins and custom options";
            //
            // FormZidGridMenuTest
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zidGrid1);
            this.Name = "FormZidGridMenuTest";
            this.Text = "ZidGrid Menu Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonCode.Win.Controls.Grid.ZidGrid zidGrid1;
        private System.Windows.Forms.Label label1;
    }
}
