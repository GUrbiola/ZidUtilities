namespace CommonCode.ICSharpTextEditor
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
            this.components = new System.ComponentModel.Container();
            this.CtrlEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.CtrlToolbar = new System.Windows.Forms.ToolStrip();
            this.tsRun = new System.Windows.Forms.ToolStripButton();
            this.tsStop = new System.Windows.Forms.ToolStripButton();
            this.tsKill = new System.Windows.Forms.ToolStripButton();
            this.tsComment = new System.Windows.Forms.ToolStripButton();
            this.tsUncomment = new System.Windows.Forms.ToolStripButton();
            this.tsSearch = new System.Windows.Forms.ToolStripButton();
            this.tsToggleBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsPreviousBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsNextBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsClearBookmarks = new System.Windows.Forms.ToolStripButton();
            this.tsSaveToFile = new System.Windows.Forms.ToolStripButton();
            this.tsLoadFromFile = new System.Windows.Forms.ToolStripButton();
            this.tsText2 = new System.Windows.Forms.ToolStripTextBox();
            this.tsText1 = new System.Windows.Forms.ToolStripTextBox();
            this.FoldingRefresher = new System.Windows.Forms.Timer(this.components);
            this.CtrlToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // CtrlEditor
            // 
            this.CtrlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CtrlEditor.IsIconBarVisible = true;
            this.CtrlEditor.IsReadOnly = false;
            this.CtrlEditor.Location = new System.Drawing.Point(0, 30);
            this.CtrlEditor.Name = "CtrlEditor";
            this.CtrlEditor.ShowVRuler = false;
            this.CtrlEditor.Size = new System.Drawing.Size(767, 336);
            this.CtrlEditor.TabIndex = 0;
            // 
            // CtrlToolbar
            // 
            this.CtrlToolbar.AutoSize = false;
            this.CtrlToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.CtrlToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsRun,
            this.tsStop,
            this.tsKill,
            this.tsComment,
            this.tsUncomment,
            this.tsSearch,
            this.tsToggleBookmark,
            this.tsPreviousBookmark,
            this.tsNextBookmark,
            this.tsClearBookmarks,
            this.tsSaveToFile,
            this.tsLoadFromFile,
            this.tsText2,
            this.tsText1});
            this.CtrlToolbar.Location = new System.Drawing.Point(0, 0);
            this.CtrlToolbar.Name = "CtrlToolbar";
            this.CtrlToolbar.Size = new System.Drawing.Size(767, 30);
            this.CtrlToolbar.TabIndex = 1;
            this.CtrlToolbar.Text = "toolStrip1";
            // 
            // tsRun
            // 
            this.tsRun.AutoSize = false;
            this.tsRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRun.Image = global::CommonCode.ICSharpTextEditor.Content.Play;
            this.tsRun.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRun.Name = "tsRun";
            this.tsRun.Size = new System.Drawing.Size(28, 28);
            this.tsRun.Click += new System.EventHandler(this.tsRun_Click);
            // 
            // tsStop
            // 
            this.tsStop.AutoSize = false;
            this.tsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsStop.Image = global::CommonCode.ICSharpTextEditor.Content.Stop;
            this.tsStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsStop.Name = "tsStop";
            this.tsStop.Size = new System.Drawing.Size(28, 28);
            this.tsStop.Click += new System.EventHandler(this.tsStop_Click);
            // 
            // tsKill
            // 
            this.tsKill.AutoSize = false;
            this.tsKill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsKill.Image = global::CommonCode.ICSharpTextEditor.Content.RedAlert;
            this.tsKill.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsKill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsKill.Name = "tsKill";
            this.tsKill.Size = new System.Drawing.Size(28, 28);
            this.tsKill.Text = "toolStripButton1";
            this.tsKill.Click += new System.EventHandler(this.tsKill_Click);
            // 
            // tsComment
            // 
            this.tsComment.AutoSize = false;
            this.tsComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsComment.Image = global::CommonCode.ICSharpTextEditor.Content.Comment;
            this.tsComment.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsComment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsComment.Name = "tsComment";
            this.tsComment.Size = new System.Drawing.Size(28, 28);
            this.tsComment.Click += new System.EventHandler(this.BtnComment_Click);
            // 
            // tsUncomment
            // 
            this.tsUncomment.AutoSize = false;
            this.tsUncomment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUncomment.Image = global::CommonCode.ICSharpTextEditor.Content.UnComment;
            this.tsUncomment.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsUncomment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUncomment.Name = "tsUncomment";
            this.tsUncomment.Size = new System.Drawing.Size(28, 28);
            this.tsUncomment.Click += new System.EventHandler(this.BtnUncomment_Click);
            // 
            // tsSearch
            // 
            this.tsSearch.AutoSize = false;
            this.tsSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSearch.Image = global::CommonCode.ICSharpTextEditor.Content.Search;
            this.tsSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSearch.Name = "tsSearch";
            this.tsSearch.Size = new System.Drawing.Size(28, 28);
            this.tsSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // tsToggleBookmark
            // 
            this.tsToggleBookmark.AutoSize = false;
            this.tsToggleBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsToggleBookmark.Image = global::CommonCode.ICSharpTextEditor.Content.Bookmark;
            this.tsToggleBookmark.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsToggleBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsToggleBookmark.Name = "tsToggleBookmark";
            this.tsToggleBookmark.Size = new System.Drawing.Size(28, 28);
            this.tsToggleBookmark.Click += new System.EventHandler(this.BtnBookmark_Click);
            // 
            // tsPreviousBookmark
            // 
            this.tsPreviousBookmark.AutoSize = false;
            this.tsPreviousBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPreviousBookmark.Image = global::CommonCode.ICSharpTextEditor.Content.Previous;
            this.tsPreviousBookmark.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsPreviousBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPreviousBookmark.Name = "tsPreviousBookmark";
            this.tsPreviousBookmark.Size = new System.Drawing.Size(28, 28);
            this.tsPreviousBookmark.Click += new System.EventHandler(this.BtnPrevious_Click);
            // 
            // tsNextBookmark
            // 
            this.tsNextBookmark.AutoSize = false;
            this.tsNextBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsNextBookmark.Image = global::CommonCode.ICSharpTextEditor.Content.Next;
            this.tsNextBookmark.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsNextBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNextBookmark.Name = "tsNextBookmark";
            this.tsNextBookmark.Size = new System.Drawing.Size(28, 28);
            this.tsNextBookmark.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // tsClearBookmarks
            // 
            this.tsClearBookmarks.AutoSize = false;
            this.tsClearBookmarks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsClearBookmarks.Image = global::CommonCode.ICSharpTextEditor.Content.DelBookmark;
            this.tsClearBookmarks.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsClearBookmarks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsClearBookmarks.Name = "tsClearBookmarks";
            this.tsClearBookmarks.Size = new System.Drawing.Size(28, 28);
            this.tsClearBookmarks.Click += new System.EventHandler(this.BtnClearBookmarks_Click);
            // 
            // tsSaveToFile
            // 
            this.tsSaveToFile.AutoSize = false;
            this.tsSaveToFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSaveToFile.Image = global::CommonCode.ICSharpTextEditor.Content.Save;
            this.tsSaveToFile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSaveToFile.Name = "tsSaveToFile";
            this.tsSaveToFile.Size = new System.Drawing.Size(28, 28);
            this.tsSaveToFile.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // tsLoadFromFile
            // 
            this.tsLoadFromFile.AutoSize = false;
            this.tsLoadFromFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsLoadFromFile.Image = global::CommonCode.ICSharpTextEditor.Content.Open;
            this.tsLoadFromFile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsLoadFromFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsLoadFromFile.Name = "tsLoadFromFile";
            this.tsLoadFromFile.Size = new System.Drawing.Size(28, 28);
            this.tsLoadFromFile.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // tsText2
            // 
            this.tsText2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsText2.Enabled = false;
            this.tsText2.Name = "tsText2";
            this.tsText2.Size = new System.Drawing.Size(200, 30);
            this.tsText2.Text = "TextBoxHelper2";
            // 
            // tsText1
            // 
            this.tsText1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsText1.Enabled = false;
            this.tsText1.Name = "tsText1";
            this.tsText1.Size = new System.Drawing.Size(200, 30);
            this.tsText1.Text = "TextBoxHelper1";
            // 
            // FoldingRefresher
            // 
            this.FoldingRefresher.Interval = 200;
            this.FoldingRefresher.Tick += new System.EventHandler(this.FoldingRefresher_Tick);
            // 
            // ExtendedEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CtrlEditor);
            this.Controls.Add(this.CtrlToolbar);
            this.Name = "ExtendedEditor";
            this.Size = new System.Drawing.Size(767, 366);
            this.CtrlToolbar.ResumeLayout(false);
            this.CtrlToolbar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl CtrlEditor;
        private System.Windows.Forms.ToolStrip CtrlToolbar;
        private System.Windows.Forms.ToolStripButton tsRun;
        private System.Windows.Forms.ToolStripButton tsStop;
        private System.Windows.Forms.ToolStripButton tsKill;
        private System.Windows.Forms.ToolStripButton tsComment;
        private System.Windows.Forms.ToolStripButton tsUncomment;
        private System.Windows.Forms.ToolStripButton tsSearch;
        private System.Windows.Forms.ToolStripButton tsToggleBookmark;
        private System.Windows.Forms.ToolStripButton tsPreviousBookmark;
        private System.Windows.Forms.ToolStripButton tsNextBookmark;
        private System.Windows.Forms.ToolStripButton tsClearBookmarks;
        private System.Windows.Forms.ToolStripButton tsSaveToFile;
        private System.Windows.Forms.ToolStripButton tsLoadFromFile;
        private System.Windows.Forms.ToolStripTextBox tsText2;
        private System.Windows.Forms.ToolStripTextBox tsText1;
        private System.Windows.Forms.Timer FoldingRefresher;
    }
}
