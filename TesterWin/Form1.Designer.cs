namespace ZidUtilities.TesterWin
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadHtmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJavaScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extEditor = new CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFilesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(937, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadFilesToolStripMenuItem
            // 
            this.loadFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSQLToolStripMenuItem,
            this.loadCToolStripMenuItem,
            this.loadHtmlToolStripMenuItem,
            this.loadJavaScriptToolStripMenuItem,
            this.loadCssToolStripMenuItem,
            this.loadXmlToolStripMenuItem,
            this.loadJsonToolStripMenuItem,
            this.removeHighlightingToolStripMenuItem});
            this.loadFilesToolStripMenuItem.Name = "loadFilesToolStripMenuItem";
            this.loadFilesToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.loadFilesToolStripMenuItem.Text = "Load Files";
            // 
            // loadSQLToolStripMenuItem
            // 
            this.loadSQLToolStripMenuItem.Name = "loadSQLToolStripMenuItem";
            this.loadSQLToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadSQLToolStripMenuItem.Text = "Load SQL";
            this.loadSQLToolStripMenuItem.Click += new System.EventHandler(this.loadSQLToolStripMenuItem_Click);
            // 
            // loadCToolStripMenuItem
            // 
            this.loadCToolStripMenuItem.Name = "loadCToolStripMenuItem";
            this.loadCToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadCToolStripMenuItem.Text = "Load C#";
            this.loadCToolStripMenuItem.Click += new System.EventHandler(this.loadCToolStripMenuItem_Click);
            // 
            // loadHtmlToolStripMenuItem
            // 
            this.loadHtmlToolStripMenuItem.Name = "loadHtmlToolStripMenuItem";
            this.loadHtmlToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadHtmlToolStripMenuItem.Text = "Load Html";
            this.loadHtmlToolStripMenuItem.Click += new System.EventHandler(this.loadHtmlToolStripMenuItem_Click);
            // 
            // loadJavaScriptToolStripMenuItem
            // 
            this.loadJavaScriptToolStripMenuItem.Name = "loadJavaScriptToolStripMenuItem";
            this.loadJavaScriptToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadJavaScriptToolStripMenuItem.Text = "Load JavaScript";
            this.loadJavaScriptToolStripMenuItem.Click += new System.EventHandler(this.loadJavaScriptToolStripMenuItem_Click);
            // 
            // loadCssToolStripMenuItem
            // 
            this.loadCssToolStripMenuItem.Name = "loadCssToolStripMenuItem";
            this.loadCssToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadCssToolStripMenuItem.Text = "Load Css";
            this.loadCssToolStripMenuItem.Click += new System.EventHandler(this.loadCssToolStripMenuItem_Click);
            // 
            // loadXmlToolStripMenuItem
            // 
            this.loadXmlToolStripMenuItem.Name = "loadXmlToolStripMenuItem";
            this.loadXmlToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadXmlToolStripMenuItem.Text = "Load Xml";
            this.loadXmlToolStripMenuItem.Click += new System.EventHandler(this.loadXmlToolStripMenuItem_Click);
            // 
            // loadJsonToolStripMenuItem
            // 
            this.loadJsonToolStripMenuItem.Name = "loadJsonToolStripMenuItem";
            this.loadJsonToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadJsonToolStripMenuItem.Text = "Load Json";
            this.loadJsonToolStripMenuItem.Click += new System.EventHandler(this.loadJsonToolStripMenuItem_Click);
            // 
            // removeHighlightingToolStripMenuItem
            // 
            this.removeHighlightingToolStripMenuItem.Name = "removeHighlightingToolStripMenuItem";
            this.removeHighlightingToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.removeHighlightingToolStripMenuItem.Text = "Remove Highlighting";
            this.removeHighlightingToolStripMenuItem.Click += new System.EventHandler(this.removeHighlightingToolStripMenuItem_Click);
            // 
            // extEditor
            // 
            this.extEditor.BtnClearBookmarks.Enabled = true;
            this.extEditor.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon")));
            this.extEditor.BtnClearBookmarks.Name = "ClearBookmarks";
            this.extEditor.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.extEditor.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.extEditor.BtnClearBookmarks.Visible = true;
            this.extEditor.BtnComment.Enabled = true;
            this.extEditor.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon1")));
            this.extEditor.BtnComment.Name = "Comment";
            this.extEditor.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.extEditor.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.extEditor.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.extEditor.BtnComment.Visible = true;
            this.extEditor.BtnKill.Enabled = true;
            this.extEditor.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon2")));
            this.extEditor.BtnKill.Name = "Kill";
            this.extEditor.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.extEditor.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.extEditor.BtnKill.Visible = true;
            this.extEditor.BtnLoadFromFile.Enabled = true;
            this.extEditor.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon3")));
            this.extEditor.BtnLoadFromFile.Name = "LoadFromFile";
            this.extEditor.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extEditor.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.extEditor.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.extEditor.BtnLoadFromFile.Visible = true;
            this.extEditor.BtnNextBookmark.Enabled = true;
            this.extEditor.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon4")));
            this.extEditor.BtnNextBookmark.Name = "NextBookmark";
            this.extEditor.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.extEditor.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.extEditor.BtnNextBookmark.Visible = true;
            this.extEditor.BtnPreviousBookmark.Enabled = true;
            this.extEditor.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon5")));
            this.extEditor.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.extEditor.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.extEditor.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.extEditor.BtnPreviousBookmark.Visible = true;
            this.extEditor.BtnRun.Enabled = true;
            this.extEditor.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon6")));
            this.extEditor.BtnRun.Name = "Run";
            this.extEditor.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.extEditor.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.extEditor.BtnRun.Visible = true;
            this.extEditor.BtnSaveToFile.Enabled = true;
            this.extEditor.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon7")));
            this.extEditor.BtnSaveToFile.Name = "SaveToFile";
            this.extEditor.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.extEditor.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.extEditor.BtnSaveToFile.Visible = true;
            this.extEditor.BtnSearch.Enabled = true;
            this.extEditor.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon8")));
            this.extEditor.BtnSearch.Name = "Search";
            this.extEditor.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.extEditor.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.extEditor.BtnSearch.Visible = true;
            this.extEditor.BtnStop.Enabled = true;
            this.extEditor.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon9")));
            this.extEditor.BtnStop.Name = "Stop";
            this.extEditor.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.extEditor.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.extEditor.BtnStop.Visible = true;
            this.extEditor.BtnToggleBookmark.Enabled = true;
            this.extEditor.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon10")));
            this.extEditor.BtnToggleBookmark.Name = "ToggleBookmark";
            this.extEditor.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.extEditor.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.extEditor.BtnToggleBookmark.Visible = true;
            this.extEditor.BtnUncomment.Enabled = true;
            this.extEditor.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon11")));
            this.extEditor.BtnUncomment.Name = "Uncomment";
            this.extEditor.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.extEditor.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.extEditor.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.extEditor.BtnUncomment.Visible = true;
            this.extEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extEditor.ImpCollapseOutlining.Enabled = false;
            this.extEditor.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.extEditor.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extEditor.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.extEditor.ImpExpandOutlining.Enabled = true;
            this.extEditor.ImpExpandOutlining.Name = "ExpandOutlining";
            this.extEditor.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extEditor.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.extEditor.ImpSearchBackward.Enabled = true;
            this.extEditor.ImpSearchBackward.Name = "SearchBackward";
            this.extEditor.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.extEditor.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.ImpSearchForward.Enabled = true;
            this.extEditor.ImpSearchForward.Name = "SearchForward";
            this.extEditor.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.extEditor.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.ImpSilentSearch.Enabled = true;
            this.extEditor.ImpSilentSearch.Name = "SilentSearch";
            this.extEditor.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.extEditor.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.ImpToggleOutlining.Enabled = true;
            this.extEditor.ImpToggleOutlining.Name = "ToggleOutlining";
            this.extEditor.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extEditor.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.extEditor.ImpToLowerCase.Enabled = true;
            this.extEditor.ImpToLowerCase.Name = "ToLower";
            this.extEditor.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.extEditor.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.ImpToUpperCase.Enabled = true;
            this.extEditor.ImpToUpperCase.Name = "ToUpper";
            this.extEditor.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.extEditor.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extEditor.Location = new System.Drawing.Point(0, 24);
            this.extEditor.Name = "extEditor";
            this.extEditor.ShowToolbar = true;
            this.extEditor.Size = new System.Drawing.Size(937, 572);
            this.extEditor.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.extEditor.TabIndex = 2;
            this.extEditor.TrackToolbarShortcuts = true;
            this.extEditor.Txt01Helper.Text = "TextBoxHelper1";
            this.extEditor.Txt01Helper.ToolTip = "";
            this.extEditor.Txt01Helper.Visible = true;
            this.extEditor.Txt02Helper.Text = "TextBoxHelper2";
            this.extEditor.Txt02Helper.ToolTip = "";
            this.extEditor.Txt02Helper.Visible = true;
            this.extEditor.OnRun += new CommonCode.ICSharpTextEditor.OnToolbarButtonClick(this.extEditor_OnRun);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 596);
            this.Controls.Add(this.extEditor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSQLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadHtmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJavaScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCssToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeHighlightingToolStripMenuItem;
        private CommonCode.ICSharpTextEditor.ExtendedEditor extEditor;
    }
}

