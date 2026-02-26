namespace ZidUtilities.CommonCode.Win.Controls
{
    partial class TokenSelect
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                // Dispose dropdown form
                if (_dropdownForm != null)
                {
                    _dropdownForm.Dispose();
                    _dropdownForm = null;
                }

                // Dispose token chips
                if (_tokenChipMap != null)
                {
                    foreach (var chip in _tokenChipMap.Values)
                    {
                        chip.Dispose();
                    }
                    _tokenChipMap.Clear();
                }
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.borderPanel = new System.Windows.Forms.Panel();
            this.tokenPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.inputBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.tokenPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.borderPanel, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(400, 32);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // borderPanel
            // 
            this.borderPanel.BackColor = System.Drawing.Color.White;
            this.borderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.borderPanel.Controls.Add(this.tokenPanel);
            this.borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderPanel.Location = new System.Drawing.Point(0, 0);
            this.borderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Size = new System.Drawing.Size(400, 32);
            this.borderPanel.TabIndex = 0;
            // 
            // tokenPanel
            // 
            this.tokenPanel.Controls.Add(this.inputBox);
            this.tokenPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tokenPanel.Location = new System.Drawing.Point(0, 0);
            this.tokenPanel.Name = "tokenPanel";
            this.tokenPanel.Padding = new System.Windows.Forms.Padding(2);
            this.tokenPanel.Size = new System.Drawing.Size(398, 30);
            this.tokenPanel.TabIndex = 0;
            this.tokenPanel.Click += new System.EventHandler(this.tokenPanel_Click);
            // 
            // inputBox
            // 
            this.inputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputBox.Location = new System.Drawing.Point(5, 8);
            this.inputBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.inputBox.MinimumSize = new System.Drawing.Size(200, 15);
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(200, 13);
            this.inputBox.TabIndex = 0;
            // 
            // TokenSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(100, 32);
            this.Name = "TokenSelect";
            this.Size = new System.Drawing.Size(400, 32);
            this.tableLayoutPanel.ResumeLayout(false);
            this.borderPanel.ResumeLayout(false);
            this.tokenPanel.ResumeLayout(false);
            this.tokenPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel borderPanel;
        private System.Windows.Forms.FlowLayoutPanel tokenPanel;
        private System.Windows.Forms.TextBox inputBox;
    }
}
