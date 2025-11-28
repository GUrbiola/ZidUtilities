namespace TesterWin
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
            this.extendedEditor1 = new CommonCode.ICSharpTextEditor.ExtendedEditor();
            this.SuspendLayout();
            // 
            // extendedEditor1
            // 
            this.extendedEditor1.BtnClearBookmarks.Enabled = true;
            this.extendedEditor1.BtnClearBookmarks.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon")));
            this.extendedEditor1.BtnClearBookmarks.Name = "ClearBookmarks";
            this.extendedEditor1.BtnClearBookmarks.Tooltip = "Clears all bookmarks";
            this.extendedEditor1.BtnClearBookmarks.Visible = true;
            this.extendedEditor1.BtnComment.Enabled = false;
            this.extendedEditor1.BtnComment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon1")));
            this.extendedEditor1.BtnComment.Name = "Comment";
            this.extendedEditor1.BtnComment.Tooltip = "Comment selected code lines";
            this.extendedEditor1.BtnComment.Visible = true;
            this.extendedEditor1.BtnKill.Enabled = false;
            this.extendedEditor1.BtnKill.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon2")));
            this.extendedEditor1.BtnKill.Name = "Kill";
            this.extendedEditor1.BtnKill.Tooltip = "Kills thread executing code";
            this.extendedEditor1.BtnKill.Visible = true;
            this.extendedEditor1.BtnLoadFromFile.Enabled = true;
            this.extendedEditor1.BtnLoadFromFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon3")));
            this.extendedEditor1.BtnLoadFromFile.Name = "LoadFromFile";
            this.extendedEditor1.BtnLoadFromFile.Tooltip = "Load text from file into the editor";
            this.extendedEditor1.BtnLoadFromFile.Visible = true;
            this.extendedEditor1.BtnNextBookmark.Enabled = true;
            this.extendedEditor1.BtnNextBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon4")));
            this.extendedEditor1.BtnNextBookmark.Name = "NextBookmark";
            this.extendedEditor1.BtnNextBookmark.Tooltip = "Moves cursor/position to the next bookmark";
            this.extendedEditor1.BtnNextBookmark.Visible = true;
            this.extendedEditor1.BtnPreviousBookmark.Enabled = true;
            this.extendedEditor1.BtnPreviousBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon5")));
            this.extendedEditor1.BtnPreviousBookmark.Name = "PreviousBookmark";
            this.extendedEditor1.BtnPreviousBookmark.Tooltip = "Moves cursor/position to the previous bookmark";
            this.extendedEditor1.BtnPreviousBookmark.Visible = true;
            this.extendedEditor1.BtnRun.Enabled = true;
            this.extendedEditor1.BtnRun.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon6")));
            this.extendedEditor1.BtnRun.Name = "Run";
            this.extendedEditor1.BtnRun.Tooltip = "Executes selected/all code";
            this.extendedEditor1.BtnRun.Visible = true;
            this.extendedEditor1.BtnSaveToFile.Enabled = true;
            this.extendedEditor1.BtnSaveToFile.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon7")));
            this.extendedEditor1.BtnSaveToFile.Name = "SaveToFile";
            this.extendedEditor1.BtnSaveToFile.Tooltip = "Save text on editor to a file";
            this.extendedEditor1.BtnSaveToFile.Visible = true;
            this.extendedEditor1.BtnSearch.Enabled = true;
            this.extendedEditor1.BtnSearch.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon8")));
            this.extendedEditor1.BtnSearch.Name = "Search";
            this.extendedEditor1.BtnSearch.Tooltip = "Shows dialog to search/replace text from editor";
            this.extendedEditor1.BtnSearch.Visible = true;
            this.extendedEditor1.BtnStop.Enabled = true;
            this.extendedEditor1.BtnStop.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon9")));
            this.extendedEditor1.BtnStop.Name = "Stop";
            this.extendedEditor1.BtnStop.Tooltip = "Stops code execution";
            this.extendedEditor1.BtnStop.Visible = true;
            this.extendedEditor1.BtnToggleBookmark.Enabled = true;
            this.extendedEditor1.BtnToggleBookmark.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon10")));
            this.extendedEditor1.BtnToggleBookmark.Name = "ToggleBookmark";
            this.extendedEditor1.BtnToggleBookmark.Tooltip = "Creates or remove a bookmark from the current line";
            this.extendedEditor1.BtnToggleBookmark.Visible = true;
            this.extendedEditor1.BtnUncomment.Enabled = true;
            this.extendedEditor1.BtnUncomment.Icon = ((System.Drawing.Image)(resources.GetObject("resource.Icon11")));
            this.extendedEditor1.BtnUncomment.Name = "Uncomment";
            this.extendedEditor1.BtnUncomment.Tooltip = "Uncomment selected code lines";
            this.extendedEditor1.BtnUncomment.Visible = true;
            this.extendedEditor1.Location = new System.Drawing.Point(0, 0);
            this.extendedEditor1.Name = "extendedEditor1";
            this.extendedEditor1.ShowToolbar = true;
            this.extendedEditor1.Size = new System.Drawing.Size(614, 424);
            this.extendedEditor1.Syntax = CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.extendedEditor1.TabIndex = 0;
            this.extendedEditor1.Txt01Helper = null;
            this.extendedEditor1.Txt02Helper = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.extendedEditor1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CommonCode.ICSharpTextEditor.ExtendedEditor extendedEditor1;
    }
}

