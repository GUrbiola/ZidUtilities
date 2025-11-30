namespace ZidUtilities.CommonCode.Win.Controls.Diff
{
    partial class SideToSideLineComparer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideToSideLineComparer));
            this.Line2 = new ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.Line1 = new ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.SuspendLayout();
            // 
            // Line2
            // 
            this.Line2.BtnClearBookmarks.Enabled = true;
            this.Line2.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon")));
            this.Line2.BtnClearBookmarks.Name = "ClearBookmarks";
            this.Line2.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.Line2.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.Line2.BtnClearBookmarks.Visible = true;
            this.Line2.BtnComment.Enabled = true;
            this.Line2.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon1")));
            this.Line2.BtnComment.Name = "Comment";
            this.Line2.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.Line2.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Line2.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.Line2.BtnComment.Visible = true;
            this.Line2.BtnKill.Enabled = true;
            this.Line2.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon2")));
            this.Line2.BtnKill.Name = "Kill";
            this.Line2.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.Line2.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.Line2.BtnKill.Visible = true;
            this.Line2.BtnLoadFromFile.Enabled = true;
            this.Line2.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon3")));
            this.Line2.BtnLoadFromFile.Name = "LoadFromFile";
            this.Line2.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line2.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.Line2.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.Line2.BtnLoadFromFile.Visible = true;
            this.Line2.BtnNextBookmark.Enabled = true;
            this.Line2.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon4")));
            this.Line2.BtnNextBookmark.Name = "NextBookmark";
            this.Line2.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.Line2.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.Line2.BtnNextBookmark.Visible = true;
            this.Line2.BtnPreviousBookmark.Enabled = true;
            this.Line2.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon5")));
            this.Line2.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.Line2.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.Line2.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.Line2.BtnPreviousBookmark.Visible = true;
            this.Line2.BtnRun.Enabled = true;
            this.Line2.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon6")));
            this.Line2.BtnRun.Name = "Run";
            this.Line2.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.Line2.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.Line2.BtnRun.Visible = true;
            this.Line2.BtnSaveToFile.Enabled = true;
            this.Line2.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon7")));
            this.Line2.BtnSaveToFile.Name = "SaveToFile";
            this.Line2.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.Line2.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.Line2.BtnSaveToFile.Visible = true;
            this.Line2.BtnSearch.Enabled = true;
            this.Line2.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon8")));
            this.Line2.BtnSearch.Name = "Search";
            this.Line2.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.Line2.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.Line2.BtnSearch.Visible = true;
            this.Line2.BtnStop.Enabled = true;
            this.Line2.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon9")));
            this.Line2.BtnStop.Name = "Stop";
            this.Line2.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.Line2.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.Line2.BtnStop.Visible = true;
            this.Line2.BtnToggleBookmark.Enabled = true;
            this.Line2.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon10")));
            this.Line2.BtnToggleBookmark.Name = "ToggleBookmark";
            this.Line2.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.Line2.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.Line2.BtnToggleBookmark.Visible = true;
            this.Line2.BtnUncomment.Enabled = true;
            this.Line2.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon11")));
            this.Line2.BtnUncomment.Name = "Uncomment";
            this.Line2.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.Line2.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.Line2.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.Line2.BtnUncomment.Visible = true;
            this.Line2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Line2.EditorText = "";
            this.Line2.ImpCollapseOutlining.Enabled = true;
            this.Line2.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.Line2.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line2.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Line2.ImpExpandOutlining.Enabled = true;
            this.Line2.ImpExpandOutlining.Name = "ExpandOutlining";
            this.Line2.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line2.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.Line2.ImpSearchBackward.Enabled = true;
            this.Line2.ImpSearchBackward.Name = "SearchBackward";
            this.Line2.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.Line2.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.ImpSearchForward.Enabled = true;
            this.Line2.ImpSearchForward.Name = "SearchForward";
            this.Line2.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.Line2.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.ImpSilentSearch.Enabled = true;
            this.Line2.ImpSilentSearch.Name = "SilentSearch";
            this.Line2.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.Line2.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.ImpToggleOutlining.Enabled = true;
            this.Line2.ImpToggleOutlining.Name = "ToggleOutlining";
            this.Line2.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line2.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.Line2.ImpToLowerCase.Enabled = true;
            this.Line2.ImpToLowerCase.Name = "ToLower";
            this.Line2.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.Line2.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.ImpToUpperCase.Enabled = true;
            this.Line2.ImpToUpperCase.Name = "ToUpper";
            this.Line2.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.Line2.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line2.Location = new System.Drawing.Point(2, 36);
            this.Line2.Name = "Line2";
            this.Line2.ShowToolbar = false;
            this.Line2.Size = new System.Drawing.Size(644, 32);
            this.Line2.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.Line2.TabIndex = 1;
            this.Line2.TrackToolbarShortcuts = false;
            this.Line2.Txt01Helper.Text = "TextBoxHelper1";
            this.Line2.Txt01Helper.ToolTip = "";
            this.Line2.Txt01Helper.Visible = true;
            this.Line2.Txt02Helper.Text = "TextBoxHelper2";
            this.Line2.Txt02Helper.ToolTip = "";
            this.Line2.Txt02Helper.Visible = true;
            // 
            // Line1
            // 
            this.Line1.BtnClearBookmarks.Enabled = true;
            this.Line1.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon12")));
            this.Line1.BtnClearBookmarks.Name = "ClearBookmarks";
            this.Line1.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.Line1.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.Line1.BtnClearBookmarks.Visible = true;
            this.Line1.BtnComment.Enabled = true;
            this.Line1.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon13")));
            this.Line1.BtnComment.Name = "Comment";
            this.Line1.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.Line1.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Line1.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.Line1.BtnComment.Visible = true;
            this.Line1.BtnKill.Enabled = true;
            this.Line1.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon14")));
            this.Line1.BtnKill.Name = "Kill";
            this.Line1.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.Line1.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.Line1.BtnKill.Visible = true;
            this.Line1.BtnLoadFromFile.Enabled = true;
            this.Line1.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon15")));
            this.Line1.BtnLoadFromFile.Name = "LoadFromFile";
            this.Line1.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line1.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.Line1.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.Line1.BtnLoadFromFile.Visible = true;
            this.Line1.BtnNextBookmark.Enabled = true;
            this.Line1.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon16")));
            this.Line1.BtnNextBookmark.Name = "NextBookmark";
            this.Line1.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.Line1.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.Line1.BtnNextBookmark.Visible = true;
            this.Line1.BtnPreviousBookmark.Enabled = true;
            this.Line1.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon17")));
            this.Line1.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.Line1.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.Line1.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.Line1.BtnPreviousBookmark.Visible = true;
            this.Line1.BtnRun.Enabled = true;
            this.Line1.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon18")));
            this.Line1.BtnRun.Name = "Run";
            this.Line1.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.Line1.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.Line1.BtnRun.Visible = true;
            this.Line1.BtnSaveToFile.Enabled = true;
            this.Line1.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon19")));
            this.Line1.BtnSaveToFile.Name = "SaveToFile";
            this.Line1.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.Line1.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.Line1.BtnSaveToFile.Visible = true;
            this.Line1.BtnSearch.Enabled = true;
            this.Line1.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon20")));
            this.Line1.BtnSearch.Name = "Search";
            this.Line1.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.Line1.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.Line1.BtnSearch.Visible = true;
            this.Line1.BtnStop.Enabled = true;
            this.Line1.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon21")));
            this.Line1.BtnStop.Name = "Stop";
            this.Line1.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.Line1.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.Line1.BtnStop.Visible = true;
            this.Line1.BtnToggleBookmark.Enabled = true;
            this.Line1.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon22")));
            this.Line1.BtnToggleBookmark.Name = "ToggleBookmark";
            this.Line1.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.Line1.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.Line1.BtnToggleBookmark.Visible = true;
            this.Line1.BtnUncomment.Enabled = true;
            this.Line1.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon23")));
            this.Line1.BtnUncomment.Name = "Uncomment";
            this.Line1.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.Line1.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.Line1.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.Line1.BtnUncomment.Visible = true;
            this.Line1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Line1.EditorText = "";
            this.Line1.ImpCollapseOutlining.Enabled = true;
            this.Line1.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.Line1.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line1.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Line1.ImpExpandOutlining.Enabled = true;
            this.Line1.ImpExpandOutlining.Name = "ExpandOutlining";
            this.Line1.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line1.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.Line1.ImpSearchBackward.Enabled = true;
            this.Line1.ImpSearchBackward.Name = "SearchBackward";
            this.Line1.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.Line1.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.ImpSearchForward.Enabled = true;
            this.Line1.ImpSearchForward.Name = "SearchForward";
            this.Line1.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.Line1.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.ImpSilentSearch.Enabled = true;
            this.Line1.ImpSilentSearch.Name = "SilentSearch";
            this.Line1.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.Line1.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.ImpToggleOutlining.Enabled = true;
            this.Line1.ImpToggleOutlining.Name = "ToggleOutlining";
            this.Line1.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.Line1.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.Line1.ImpToLowerCase.Enabled = true;
            this.Line1.ImpToLowerCase.Name = "ToLower";
            this.Line1.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.Line1.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.ImpToUpperCase.Enabled = true;
            this.Line1.ImpToUpperCase.Name = "ToUpper";
            this.Line1.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.Line1.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.Line1.Location = new System.Drawing.Point(2, 2);
            this.Line1.Name = "Line1";
            this.Line1.ShowToolbar = false;
            this.Line1.Size = new System.Drawing.Size(644, 32);
            this.Line1.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.Line1.TabIndex = 0;
            this.Line1.TrackToolbarShortcuts = false;
            this.Line1.Txt01Helper.Text = "TextBoxHelper1";
            this.Line1.Txt01Helper.ToolTip = "";
            this.Line1.Txt01Helper.Visible = true;
            this.Line1.Txt02Helper.Text = "TextBoxHelper2";
            this.Line1.Txt02Helper.ToolTip = "";
            this.Line1.Txt02Helper.Visible = true;
            // 
            // SideToSideLineComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Line2);
            this.Controls.Add(this.Line1);
            this.Name = "SideToSideLineComparer";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(648, 70);
            this.SizeChanged += new System.EventHandler(this.SideToSideLineComparer_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private CommonCode.ICSharpTextEditor.ExtendedEditor Line1;
        private CommonCode.ICSharpTextEditor.ExtendedEditor Line2;
    }
}
