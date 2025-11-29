namespace Ez_SQL.Custom_Controls
{
    partial class SideToSideTextComparer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideToSideTextComparer));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LabTxt1 = new System.Windows.Forms.Label();
            this.LabTxt2 = new System.Windows.Forms.Label();
            this.LeftText = new ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.RightText = new ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LeftText);
            this.splitContainer1.Panel1.Controls.Add(this.LabTxt1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RightText);
            this.splitContainer1.Panel2.Controls.Add(this.LabTxt2);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Size = new System.Drawing.Size(732, 461);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 0;
            // 
            // LabTxt1
            // 
            this.LabTxt1.AutoSize = true;
            this.LabTxt1.Dock = System.Windows.Forms.DockStyle.Top;
            this.LabTxt1.Location = new System.Drawing.Point(2, 2);
            this.LabTxt1.Name = "LabTxt1";
            this.LabTxt1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.LabTxt1.Size = new System.Drawing.Size(37, 23);
            this.LabTxt1.TabIndex = 1;
            this.LabTxt1.Text = "Text 1";
            // 
            // LabTxt2
            // 
            this.LabTxt2.AutoSize = true;
            this.LabTxt2.Dock = System.Windows.Forms.DockStyle.Top;
            this.LabTxt2.Location = new System.Drawing.Point(2, 2);
            this.LabTxt2.Name = "LabTxt2";
            this.LabTxt2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.LabTxt2.Size = new System.Drawing.Size(37, 23);
            this.LabTxt2.TabIndex = 3;
            this.LabTxt2.Text = "Text 2";
            // 
            // LeftText
            // 
            this.LeftText.BtnClearBookmarks.Enabled = true;
            this.LeftText.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon")));
            this.LeftText.BtnClearBookmarks.Name = "ClearBookmarks";
            this.LeftText.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.LeftText.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.LeftText.BtnClearBookmarks.Visible = true;
            this.LeftText.BtnComment.Enabled = true;
            this.LeftText.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon1")));
            this.LeftText.BtnComment.Name = "Comment";
            this.LeftText.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.LeftText.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.LeftText.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.LeftText.BtnComment.Visible = true;
            this.LeftText.BtnKill.Enabled = true;
            this.LeftText.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon2")));
            this.LeftText.BtnKill.Name = "Kill";
            this.LeftText.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.LeftText.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.LeftText.BtnKill.Visible = true;
            this.LeftText.BtnLoadFromFile.Enabled = true;
            this.LeftText.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon3")));
            this.LeftText.BtnLoadFromFile.Name = "LoadFromFile";
            this.LeftText.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.LeftText.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.LeftText.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.LeftText.BtnLoadFromFile.Visible = true;
            this.LeftText.BtnNextBookmark.Enabled = true;
            this.LeftText.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon4")));
            this.LeftText.BtnNextBookmark.Name = "NextBookmark";
            this.LeftText.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.LeftText.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.LeftText.BtnNextBookmark.Visible = true;
            this.LeftText.BtnPreviousBookmark.Enabled = true;
            this.LeftText.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon5")));
            this.LeftText.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.LeftText.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.LeftText.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.LeftText.BtnPreviousBookmark.Visible = true;
            this.LeftText.BtnRun.Enabled = true;
            this.LeftText.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon6")));
            this.LeftText.BtnRun.Name = "Run";
            this.LeftText.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.LeftText.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.LeftText.BtnRun.Visible = true;
            this.LeftText.BtnSaveToFile.Enabled = true;
            this.LeftText.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon7")));
            this.LeftText.BtnSaveToFile.Name = "SaveToFile";
            this.LeftText.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.LeftText.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.LeftText.BtnSaveToFile.Visible = true;
            this.LeftText.BtnSearch.Enabled = true;
            this.LeftText.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon8")));
            this.LeftText.BtnSearch.Name = "Search";
            this.LeftText.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.LeftText.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.LeftText.BtnSearch.Visible = true;
            this.LeftText.BtnStop.Enabled = true;
            this.LeftText.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon9")));
            this.LeftText.BtnStop.Name = "Stop";
            this.LeftText.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.LeftText.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.LeftText.BtnStop.Visible = true;
            this.LeftText.BtnToggleBookmark.Enabled = true;
            this.LeftText.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon10")));
            this.LeftText.BtnToggleBookmark.Name = "ToggleBookmark";
            this.LeftText.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.LeftText.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.LeftText.BtnToggleBookmark.Visible = true;
            this.LeftText.BtnUncomment.Enabled = true;
            this.LeftText.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon11")));
            this.LeftText.BtnUncomment.Name = "Uncomment";
            this.LeftText.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.LeftText.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.LeftText.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.LeftText.BtnUncomment.Visible = true;
            this.LeftText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftText.EditorText = "";
            this.LeftText.ImpCollapseOutlining.Enabled = true;
            this.LeftText.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.LeftText.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.LeftText.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.LeftText.ImpExpandOutlining.Enabled = true;
            this.LeftText.ImpExpandOutlining.Name = "ExpandOutlining";
            this.LeftText.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.LeftText.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.LeftText.ImpSearchBackward.Enabled = true;
            this.LeftText.ImpSearchBackward.Name = "SearchBackward";
            this.LeftText.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.LeftText.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.ImpSearchForward.Enabled = true;
            this.LeftText.ImpSearchForward.Name = "SearchForward";
            this.LeftText.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.LeftText.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.ImpSilentSearch.Enabled = true;
            this.LeftText.ImpSilentSearch.Name = "SilentSearch";
            this.LeftText.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.LeftText.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.ImpToggleOutlining.Enabled = true;
            this.LeftText.ImpToggleOutlining.Name = "ToggleOutlining";
            this.LeftText.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.LeftText.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.LeftText.ImpToLowerCase.Enabled = true;
            this.LeftText.ImpToLowerCase.Name = "ToLower";
            this.LeftText.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.LeftText.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.ImpToUpperCase.Enabled = true;
            this.LeftText.ImpToUpperCase.Name = "ToUpper";
            this.LeftText.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.LeftText.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.LeftText.Location = new System.Drawing.Point(2, 25);
            this.LeftText.Name = "LeftText";
            this.LeftText.ShowToolbar = false;
            this.LeftText.Size = new System.Drawing.Size(354, 434);
            this.LeftText.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.LeftText.TabIndex = 2;
            this.LeftText.TrackToolbarShortcuts = false;
            this.LeftText.Txt01Helper.Text = "TextBoxHelper1";
            this.LeftText.Txt01Helper.ToolTip = "";
            this.LeftText.Txt01Helper.Visible = true;
            this.LeftText.Txt02Helper.Text = "TextBoxHelper2";
            this.LeftText.Txt02Helper.ToolTip = "";
            this.LeftText.Txt02Helper.Visible = true;
            // 
            // RightText
            // 
            this.RightText.BtnClearBookmarks.Enabled = true;
            this.RightText.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon12")));
            this.RightText.BtnClearBookmarks.Name = "ClearBookmarks";
            this.RightText.BtnClearBookmarks.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F2)));
            this.RightText.BtnClearBookmarks.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnClearBookmarks.Tooltip = "Clears all bookmarks (Ctr + + Shift + F2)";
            this.RightText.BtnClearBookmarks.Visible = true;
            this.RightText.BtnComment.Enabled = true;
            this.RightText.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon13")));
            this.RightText.BtnComment.Name = "Comment";
            this.RightText.BtnComment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.RightText.BtnComment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.RightText.BtnComment.Tooltip = "Comment selected code lines (Ctr + K, C)";
            this.RightText.BtnComment.Visible = true;
            this.RightText.BtnKill.Enabled = true;
            this.RightText.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon14")));
            this.RightText.BtnKill.Name = "Kill";
            this.RightText.BtnKill.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.RightText.BtnKill.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnKill.Tooltip = "Kills thread executing code (Ctr + F5)";
            this.RightText.BtnKill.Visible = true;
            this.RightText.BtnLoadFromFile.Enabled = true;
            this.RightText.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon15")));
            this.RightText.BtnLoadFromFile.Name = "LoadFromFile";
            this.RightText.BtnLoadFromFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.RightText.BtnLoadFromFile.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.RightText.BtnLoadFromFile.Tooltip = "Load text from file into the editor (Ctr + O, P)";
            this.RightText.BtnLoadFromFile.Visible = true;
            this.RightText.BtnNextBookmark.Enabled = true;
            this.RightText.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon16")));
            this.RightText.BtnNextBookmark.Name = "NextBookmark";
            this.RightText.BtnNextBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.RightText.BtnNextBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark (Ctr + F2)";
            this.RightText.BtnNextBookmark.Visible = true;
            this.RightText.BtnPreviousBookmark.Enabled = true;
            this.RightText.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon17")));
            this.RightText.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.RightText.BtnPreviousBookmark.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.RightText.BtnPreviousBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark (Shift + F2)";
            this.RightText.BtnPreviousBookmark.Visible = true;
            this.RightText.BtnRun.Enabled = true;
            this.RightText.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon18")));
            this.RightText.BtnRun.Name = "Run";
            this.RightText.BtnRun.ShortCut = System.Windows.Forms.Keys.F5;
            this.RightText.BtnRun.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnRun.Tooltip = "Executes selected/all code (F5)";
            this.RightText.BtnRun.Visible = true;
            this.RightText.BtnSaveToFile.Enabled = true;
            this.RightText.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon19")));
            this.RightText.BtnSaveToFile.Name = "SaveToFile";
            this.RightText.BtnSaveToFile.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.RightText.BtnSaveToFile.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnSaveToFile.Tooltip = "Save text on editor to a file (Ctr + S)";
            this.RightText.BtnSaveToFile.Visible = true;
            this.RightText.BtnSearch.Enabled = true;
            this.RightText.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon20")));
            this.RightText.BtnSearch.Name = "Search";
            this.RightText.BtnSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.RightText.BtnSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor (Ctr + F)";
            this.RightText.BtnSearch.Visible = true;
            this.RightText.BtnStop.Enabled = true;
            this.RightText.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon21")));
            this.RightText.BtnStop.Name = "Stop";
            this.RightText.BtnStop.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.RightText.BtnStop.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnStop.Tooltip = "Stops code execution (Shift + F5)";
            this.RightText.BtnStop.Visible = true;
            this.RightText.BtnToggleBookmark.Enabled = true;
            this.RightText.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon22")));
            this.RightText.BtnToggleBookmark.Name = "ToggleBookmark";
            this.RightText.BtnToggleBookmark.ShortCut = System.Windows.Forms.Keys.F2;
            this.RightText.BtnToggleBookmark.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line (F2)";
            this.RightText.BtnToggleBookmark.Visible = true;
            this.RightText.BtnUncomment.Enabled = true;
            this.RightText.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon23")));
            this.RightText.BtnUncomment.Name = "Uncomment";
            this.RightText.BtnUncomment.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.RightText.BtnUncomment.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.RightText.BtnUncomment.Tooltip = "Uncomment selected code lines (Ctr + K, U)";
            this.RightText.BtnUncomment.Visible = true;
            this.RightText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightText.EditorText = "";
            this.RightText.ImpCollapseOutlining.Enabled = true;
            this.RightText.ImpCollapseOutlining.Name = "CollapseOutlining";
            this.RightText.ImpCollapseOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.RightText.ImpCollapseOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.RightText.ImpExpandOutlining.Enabled = true;
            this.RightText.ImpExpandOutlining.Name = "ExpandOutlining";
            this.RightText.ImpExpandOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.RightText.ImpExpandOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.RightText.ImpSearchBackward.Enabled = true;
            this.RightText.ImpSearchBackward.Name = "SearchBackward";
            this.RightText.ImpSearchBackward.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.RightText.ImpSearchBackward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.ImpSearchForward.Enabled = true;
            this.RightText.ImpSearchForward.Name = "SearchForward";
            this.RightText.ImpSearchForward.ShortCut = System.Windows.Forms.Keys.F3;
            this.RightText.ImpSearchForward.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.ImpSilentSearch.Enabled = true;
            this.RightText.ImpSilentSearch.Name = "SilentSearch";
            this.RightText.ImpSilentSearch.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.RightText.ImpSilentSearch.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.ImpToggleOutlining.Enabled = true;
            this.RightText.ImpToggleOutlining.Name = "ToggleOutlining";
            this.RightText.ImpToggleOutlining.ShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.RightText.ImpToggleOutlining.ThenShortCut = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.RightText.ImpToLowerCase.Enabled = true;
            this.RightText.ImpToLowerCase.Name = "ToLower";
            this.RightText.ImpToLowerCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.RightText.ImpToLowerCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.ImpToUpperCase.Enabled = true;
            this.RightText.ImpToUpperCase.Name = "ToUpper";
            this.RightText.ImpToUpperCase.ShortCut = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.RightText.ImpToUpperCase.ThenShortCut = System.Windows.Forms.Keys.None;
            this.RightText.Location = new System.Drawing.Point(2, 25);
            this.RightText.Name = "RightText";
            this.RightText.ShowToolbar = false;
            this.RightText.Size = new System.Drawing.Size(366, 434);
            this.RightText.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.RightText.TabIndex = 4;
            this.RightText.TrackToolbarShortcuts = false;
            this.RightText.Txt01Helper.Text = "TextBoxHelper1";
            this.RightText.Txt01Helper.ToolTip = "";
            this.RightText.Txt01Helper.Visible = true;
            this.RightText.Txt02Helper.Text = "TextBoxHelper2";
            this.RightText.Txt02Helper.ToolTip = "";
            this.RightText.Txt02Helper.Visible = true;
            // 
            // SideToSideTextComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SideToSideTextComparer";
            this.Size = new System.Drawing.Size(732, 461);
            this.SizeChanged += new System.EventHandler(this.SideToSideTextComparer_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label LabTxt1;
        private System.Windows.Forms.Label LabTxt2;
        private ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor LeftText;
        private ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor RightText;
    }
}
