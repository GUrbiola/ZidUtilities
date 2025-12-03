namespace ZidUtilities.CommonCode.AvalonEdit
{
    partial class ExtendedEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedEditor));
            this.CtrlToolbar = new System.Windows.Forms.ToolStrip();
            this.tsRun = new System.Windows.Forms.ToolStripButton();
            this.tsStop = new System.Windows.Forms.ToolStripButton();
            this.tsKill = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsComment = new System.Windows.Forms.ToolStripButton();
            this.tsUncomment = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsToggleBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsPreviousBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsNextBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsClearBookmarks = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSaveToFile = new System.Windows.Forms.ToolStripButton();
            this.tsLoadFromFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsText1 = new System.Windows.Forms.ToolStripTextBox();
            this.tsText2 = new System.Windows.Forms.ToolStripTextBox();
            this.CtrlToolbar.SuspendLayout();
            this.SuspendLayout();
            //
            // CtrlToolbar
            //
            this.CtrlToolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CtrlToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsRun,
            this.tsStop,
            this.tsKill,
            this.toolStripSeparator1,
            this.tsComment,
            this.tsUncomment,
            this.toolStripSeparator2,
            this.tsSearch,
            this.toolStripSeparator3,
            this.tsToggleBookmark,
            this.tsPreviousBookmark,
            this.tsNextBookmark,
            this.tsClearBookmarks,
            this.toolStripSeparator4,
            this.tsSaveToFile,
            this.tsLoadFromFile,
            this.toolStripSeparator5,
            this.tsText1,
            this.tsText2});
            this.CtrlToolbar.Location = new System.Drawing.Point(0, 0);
            this.CtrlToolbar.Name = "CtrlToolbar";
            this.CtrlToolbar.Size = new System.Drawing.Size(800, 27);
            this.CtrlToolbar.TabIndex = 0;
            this.CtrlToolbar.Text = "toolStrip1";
            //
            // tsRun
            //
            this.tsRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRun.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Play;
            this.tsRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRun.Name = "tsRun";
            this.tsRun.Size = new System.Drawing.Size(29, 24);
            this.tsRun.Text = "Run";
            this.tsRun.ToolTipText = "Executes selected/all code (F5)";
            this.tsRun.Click += new System.EventHandler(this.tsRun_Click);
            //
            // tsStop
            //
            this.tsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsStop.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Stop;
            this.tsStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsStop.Name = "tsStop";
            this.tsStop.Size = new System.Drawing.Size(29, 24);
            this.tsStop.Text = "Stop";
            this.tsStop.ToolTipText = "Stops code execution (Shift + F5)";
            this.tsStop.Click += new System.EventHandler(this.tsStop_Click);
            //
            // tsKill
            //
            this.tsKill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsKill.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.RedAlert;
            this.tsKill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsKill.Name = "tsKill";
            this.tsKill.Size = new System.Drawing.Size(29, 24);
            this.tsKill.Text = "Kill";
            this.tsKill.ToolTipText = "Kills thread executing code (Ctrl + F5)";
            this.tsKill.Click += new System.EventHandler(this.tsKill_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            //
            // tsComment
            //
            this.tsComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsComment.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Comment;
            this.tsComment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsComment.Name = "tsComment";
            this.tsComment.Size = new System.Drawing.Size(29, 24);
            this.tsComment.Text = "Comment";
            this.tsComment.ToolTipText = "Comment selected code lines (Ctrl + K, C)";
            this.tsComment.Click += new System.EventHandler(this.BtnComment_Click);
            //
            // tsUncomment
            //
            this.tsUncomment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUncomment.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.UnComment;
            this.tsUncomment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUncomment.Name = "tsUncomment";
            this.tsUncomment.Size = new System.Drawing.Size(29, 24);
            this.tsUncomment.Text = "Uncomment";
            this.tsUncomment.ToolTipText = "Uncomment selected code lines (Ctrl + K, U)";
            this.tsUncomment.Click += new System.EventHandler(this.BtnUncomment_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            //
            // tsSearch
            //
            this.tsSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSearch.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Search;
            this.tsSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSearch.Name = "tsSearch";
            this.tsSearch.Size = new System.Drawing.Size(29, 24);
            this.tsSearch.Text = "Search";
            this.tsSearch.ToolTipText = "Shows dialog to search/replace text (Ctrl + F)";
            this.tsSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            //
            // tsToggleBookmark
            //
            this.tsToggleBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsToggleBookmark.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Bookmark;
            this.tsToggleBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsToggleBookmark.Name = "tsToggleBookmark";
            this.tsToggleBookmark.Size = new System.Drawing.Size(29, 24);
            this.tsToggleBookmark.Text = "Toggle Bookmark";
            this.tsToggleBookmark.ToolTipText = "Creates or removes a bookmark (F2)";
            this.tsToggleBookmark.Click += new System.EventHandler(this.BtnBookmark_Click);
            //
            // tsPreviousBookmark
            //
            this.tsPreviousBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPreviousBookmark.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Previous;
            this.tsPreviousBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPreviousBookmark.Name = "tsPreviousBookmark";
            this.tsPreviousBookmark.Size = new System.Drawing.Size(29, 24);
            this.tsPreviousBookmark.Text = "Previous Bookmark";
            this.tsPreviousBookmark.ToolTipText = "Moves to previous bookmark (Shift + F2)";
            this.tsPreviousBookmark.Click += new System.EventHandler(this.BtnPrevious_Click);
            //
            // tsNextBookmark
            //
            this.tsNextBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsNextBookmark.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Next;
            this.tsNextBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNextBookmark.Name = "tsNextBookmark";
            this.tsNextBookmark.Size = new System.Drawing.Size(29, 24);
            this.tsNextBookmark.Text = "Next Bookmark";
            this.tsNextBookmark.ToolTipText = "Moves to next bookmark (Ctrl + F2)";
            this.tsNextBookmark.Click += new System.EventHandler(this.BtnNext_Click);
            //
            // tsClearBookmarks
            //
            this.tsClearBookmarks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsClearBookmarks.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.DelBookmark;
            this.tsClearBookmarks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsClearBookmarks.Name = "tsClearBookmarks";
            this.tsClearBookmarks.Size = new System.Drawing.Size(29, 24);
            this.tsClearBookmarks.Text = "Clear Bookmarks";
            this.tsClearBookmarks.ToolTipText = "Clears all bookmarks (Ctrl + Shift + F2)";
            this.tsClearBookmarks.Click += new System.EventHandler(this.BtnClearBookmarks_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            //
            // tsSaveToFile
            //
            this.tsSaveToFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSaveToFile.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Save;
            this.tsSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSaveToFile.Name = "tsSaveToFile";
            this.tsSaveToFile.Size = new System.Drawing.Size(29, 24);
            this.tsSaveToFile.Text = "Save";
            this.tsSaveToFile.ToolTipText = "Save text to file (Ctrl + S)";
            this.tsSaveToFile.Click += new System.EventHandler(this.BtnSave_Click);
            //
            // tsLoadFromFile
            //
            this.tsLoadFromFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsLoadFromFile.Image = global::ZidUtilities.CommonCode.AvalonEdit.Content.Open;
            this.tsLoadFromFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsLoadFromFile.Name = "tsLoadFromFile";
            this.tsLoadFromFile.Size = new System.Drawing.Size(29, 24);
            this.tsLoadFromFile.Text = "Load";
            this.tsLoadFromFile.ToolTipText = "Load text from file (Ctrl + O, P)";
            this.tsLoadFromFile.Click += new System.EventHandler(this.BtnLoad_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            //
            // tsText1
            //
            this.tsText1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsText1.Name = "tsText1";
            this.tsText1.Size = new System.Drawing.Size(100, 27);
            this.tsText1.Text = "TextBoxHelper1";
            //
            // tsText2
            //
            this.tsText2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsText2.Name = "tsText2";
            this.tsText2.Size = new System.Drawing.Size(100, 27);
            this.tsText2.Text = "TextBoxHelper2";
            //
            // ExtendedEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CtrlToolbar);
            this.Name = "ExtendedEditor";
            this.Size = new System.Drawing.Size(800, 600);
            this.CtrlToolbar.ResumeLayout(false);
            this.CtrlToolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip CtrlToolbar;
        private System.Windows.Forms.ToolStripButton tsRun;
        private System.Windows.Forms.ToolStripButton tsStop;
        private System.Windows.Forms.ToolStripButton tsKill;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsComment;
        private System.Windows.Forms.ToolStripButton tsUncomment;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsToggleBookmark;
        private System.Windows.Forms.ToolStripButton tsPreviousBookmark;
        private System.Windows.Forms.ToolStripButton tsNextBookmark;
        private System.Windows.Forms.ToolStripButton tsClearBookmarks;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsSaveToFile;
        private System.Windows.Forms.ToolStripButton tsLoadFromFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripTextBox tsText1;
        private System.Windows.Forms.ToolStripTextBox tsText2;
    }
}
