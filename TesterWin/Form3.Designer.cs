namespace ZidUtilities.TesterWin
{
    partial class Form3
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadTextsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJavaScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffInspector1 = new ZidUtilities.CommonCode.Win.Controls.Diff.DiffInspector();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTextsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1178, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadTextsToolStripMenuItem
            // 
            this.loadTextsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSQLToolStripMenuItem,
            this.loadCToolStripMenuItem,
            this.loadJavaScriptToolStripMenuItem});
            this.loadTextsToolStripMenuItem.Name = "loadTextsToolStripMenuItem";
            this.loadTextsToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.loadTextsToolStripMenuItem.Text = "Load Texts";
            // 
            // loadSQLToolStripMenuItem
            // 
            this.loadSQLToolStripMenuItem.Name = "loadSQLToolStripMenuItem";
            this.loadSQLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadSQLToolStripMenuItem.Text = "Load SQL";
            this.loadSQLToolStripMenuItem.Click += new System.EventHandler(this.loadSQLToolStripMenuItem_Click);
            // 
            // loadCToolStripMenuItem
            // 
            this.loadCToolStripMenuItem.Name = "loadCToolStripMenuItem";
            this.loadCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadCToolStripMenuItem.Text = "Load C#";
            this.loadCToolStripMenuItem.Click += new System.EventHandler(this.loadCToolStripMenuItem_Click);
            // 
            // loadJavaScriptToolStripMenuItem
            // 
            this.loadJavaScriptToolStripMenuItem.Name = "loadJavaScriptToolStripMenuItem";
            this.loadJavaScriptToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadJavaScriptToolStripMenuItem.Text = "Load JavaScript";
            this.loadJavaScriptToolStripMenuItem.Click += new System.EventHandler(this.loadJavaScriptToolStripMenuItem_Click);
            // 
            // diffInspector1
            // 
            this.diffInspector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffInspector1.Highlighting = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.diffInspector1.Location = new System.Drawing.Point(0, 24);
            this.diffInspector1.LowerText = "";
            this.diffInspector1.Name = "diffInspector1";
            this.diffInspector1.ShowEagleView = true;
            this.diffInspector1.Size = new System.Drawing.Size(1178, 762);
            this.diffInspector1.TabIndex = 0;
            this.diffInspector1.TextLeftSide = "";
            this.diffInspector1.TextRightSide = "";
            this.diffInspector1.UpperText = "";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 786);
            this.Controls.Add(this.diffInspector1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form3";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonCode.Win.Controls.Diff.DiffInspector diffInspector1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadTextsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSQLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJavaScriptToolStripMenuItem;
    }
}