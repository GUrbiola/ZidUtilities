namespace ZidUtilities.TesterWin
{
    partial class Form9
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cmbThemes = new System.Windows.Forms.ComboBox();
            this.zidGrid1 = new ZidUtilities.CommonCode.Win.Controls.Grid.ZidGrid();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBox1.Location = new System.Drawing.Point(0, 21);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(1071, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Alternate Row Coloring";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // cmbThemes
            // 
            this.cmbThemes.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbThemes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThemes.FormattingEnabled = true;
            this.cmbThemes.Location = new System.Drawing.Point(0, 0);
            this.cmbThemes.Name = "cmbThemes";
            this.cmbThemes.Size = new System.Drawing.Size(1071, 21);
            this.cmbThemes.TabIndex = 3;
            // 
            // zidGrid1
            // 
            this.zidGrid1.CellFont = new System.Drawing.Font("Calibri", 9F);
            this.zidGrid1.ContextMenuImageSize = new System.Drawing.Size(32, 32);
            this.zidGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zidGrid1.Location = new System.Drawing.Point(0, 38);
            this.zidGrid1.Name = "zidGrid1";
            this.zidGrid1.Size = new System.Drawing.Size(1071, 684);
            this.zidGrid1.TabIndex = 5;
            this.zidGrid1.TitleFont = new System.Drawing.Font("Calibri", 9.25F, System.Drawing.FontStyle.Bold);
            // 
            // Form9
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 722);
            this.Controls.Add(this.zidGrid1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.cmbThemes);
            this.Name = "Form9";
            this.Text = "Form9";
            this.Load += new System.EventHandler(this.Form9_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox cmbThemes;
        private CommonCode.Win.Controls.Grid.ZidGrid zidGrid1;
    }
}