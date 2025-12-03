namespace ZidUtilities.TesterWin
{
    partial class FormThemeManagerTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormThemeManagerTest));
            this.extendedEditor1 = new ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.SuspendLayout();
            // 
            // extendedEditor1
            // 
            this.extendedEditor1.BtnClearBookmarks.Enabled = true;
            this.extendedEditor1.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon")));
            this.extendedEditor1.BtnClearBookmarks.Name = "ClearBookmarks";
            this.extendedEditor1.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.extendedEditor1.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.extendedEditor1.BtnClearBookmarks.Visible = true;
            this.extendedEditor1.BtnComment.Enabled = true;
            this.extendedEditor1.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon1")));
            this.extendedEditor1.BtnComment.Name = "Comment";
            this.extendedEditor1.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.extendedEditor1.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.extendedEditor1.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.extendedEditor1.BtnComment.Visible = true;
            this.extendedEditor1.BtnKill.Enabled = true;
            this.extendedEditor1.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon2")));
            this.extendedEditor1.BtnKill.Name = "Kill";
            this.extendedEditor1.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.extendedEditor1.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.extendedEditor1.BtnKill.Visible = true;
            this.extendedEditor1.BtnLoadFromFile.Enabled = true;
            this.extendedEditor1.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon3")));
            this.extendedEditor1.BtnLoadFromFile.Name = "LoadFromFile";
            this.extendedEditor1.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extendedEditor1.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.extendedEditor1.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.extendedEditor1.BtnLoadFromFile.Visible = true;
            this.extendedEditor1.BtnNextBookmark.Enabled = true;
            this.extendedEditor1.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon4")));
            this.extendedEditor1.BtnNextBookmark.Name = "NextBookmark";
            this.extendedEditor1.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.extendedEditor1.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.extendedEditor1.BtnNextBookmark.Visible = true;
            this.extendedEditor1.BtnPreviousBookmark.Enabled = true;
            this.extendedEditor1.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon5")));
            this.extendedEditor1.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.extendedEditor1.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.extendedEditor1.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.extendedEditor1.BtnPreviousBookmark.Visible = true;
            this.extendedEditor1.BtnRun.Enabled = true;
            this.extendedEditor1.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon6")));
            this.extendedEditor1.BtnRun.Name = "Run";
            this.extendedEditor1.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.extendedEditor1.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.extendedEditor1.BtnRun.Visible = true;
            this.extendedEditor1.BtnSaveToFile.Enabled = true;
            this.extendedEditor1.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon7")));
            this.extendedEditor1.BtnSaveToFile.Name = "SaveToFile";
            this.extendedEditor1.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.extendedEditor1.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.extendedEditor1.BtnSaveToFile.Visible = true;
            this.extendedEditor1.BtnSearch.Enabled = true;
            this.extendedEditor1.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon8")));
            this.extendedEditor1.BtnSearch.Name = "Search";
            this.extendedEditor1.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.extendedEditor1.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.extendedEditor1.BtnSearch.Visible = true;
            this.extendedEditor1.BtnStop.Enabled = true;
            this.extendedEditor1.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon9")));
            this.extendedEditor1.BtnStop.Name = "Stop";
            this.extendedEditor1.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.extendedEditor1.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.extendedEditor1.BtnStop.Visible = true;
            this.extendedEditor1.BtnToggleBookmark.Enabled = true;
            this.extendedEditor1.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon10")));
            this.extendedEditor1.BtnToggleBookmark.Name = "ToggleBookmark";
            this.extendedEditor1.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.extendedEditor1.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.extendedEditor1.BtnToggleBookmark.Visible = true;
            this.extendedEditor1.BtnUncomment.Enabled = true;
            this.extendedEditor1.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon11")));
            this.extendedEditor1.BtnUncomment.Name = "Uncomment";
            this.extendedEditor1.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.extendedEditor1.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.extendedEditor1.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.extendedEditor1.BtnUncomment.Visible = true;
            this.extendedEditor1.Dock = System.Windows.Forms.DockStyle.Right;
            this.extendedEditor1.EditorText = "select *  from Table";
            this.extendedEditor1.ImpCollapseOutlining.Enabled = true;
            this.extendedEditor1.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.extendedEditor1.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extendedEditor1.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.extendedEditor1.ImpExpandOutlining.Enabled = true;
            this.extendedEditor1.ImpExpandOutlining.Name = "ExpandOutlining";
            this.extendedEditor1.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extendedEditor1.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.extendedEditor1.ImpSearchBackward.Enabled = true;
            this.extendedEditor1.ImpSearchBackward.Name = "SearchBackward";
            this.extendedEditor1.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.extendedEditor1.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.ImpSearchForward.Enabled = true;
            this.extendedEditor1.ImpSearchForward.Name = "SearchForward";
            this.extendedEditor1.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.extendedEditor1.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.ImpSilentSearch.Enabled = true;
            this.extendedEditor1.ImpSilentSearch.Name = "SilentSearch";
            this.extendedEditor1.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.extendedEditor1.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.ImpToggleOutlining.Enabled = true;
            this.extendedEditor1.ImpToggleOutlining.Name = "ToggleOutlining";
            this.extendedEditor1.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.extendedEditor1.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.extendedEditor1.ImpToLowerCase.Enabled = true;
            this.extendedEditor1.ImpToLowerCase.Name = "ToLower";
            this.extendedEditor1.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.extendedEditor1.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.ImpToUpperCase.Enabled = true;
            this.extendedEditor1.ImpToUpperCase.Name = "ToUpper";
            this.extendedEditor1.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.extendedEditor1.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.extendedEditor1.Location = new System.Drawing.Point(1005, 0);
            this.extendedEditor1.Name = "extendedEditor1";
            this.extendedEditor1.ShowToolbar = true;
            this.extendedEditor1.Size = new System.Drawing.Size(343, 763);
            this.extendedEditor1.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.TransactSQL;
            this.extendedEditor1.TabIndex = 0;
            this.extendedEditor1.TrackToolbarShortcuts = false;
            this.extendedEditor1.Txt01Helper.Text = "TextBoxHelper1";
            this.extendedEditor1.Txt01Helper.ToolTip = "";
            this.extendedEditor1.Txt01Helper.Visible = true;
            this.extendedEditor1.Txt02Helper.Text = "TextBoxHelper2";
            this.extendedEditor1.Txt02Helper.ToolTip = "";
            this.extendedEditor1.Txt02Helper.Visible = true;
            // 
            // FormThemeManagerTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 763);
            this.Controls.Add(this.extendedEditor1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormThemeManagerTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Theme Manager Test Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormThemeManagerTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CommonCode.ICSharpTextEditor.ExtendedEditor extendedEditor1;
    }
}
