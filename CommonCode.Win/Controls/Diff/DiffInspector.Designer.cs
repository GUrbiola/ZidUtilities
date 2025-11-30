namespace ZidUtilities.CommonCode.Win.Controls.Diff
{
    partial class DiffInspector
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
            this.locationPanel = new DoubleBufferedPanel();
            this.TextDiff = new ZidUtilities.CommonCode.Win.Controls.Diff.SideToSideTextComparer();
            this.LineDiff = new ZidUtilities.CommonCode.Win.Controls.Diff.SideToSideLineComparer();
            this.SuspendLayout();
            // 
            // locationPanel
            // 
            this.locationPanel.BackColor = System.Drawing.Color.White;
            this.locationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.locationPanel.Location = new System.Drawing.Point(906, 0);
            this.locationPanel.Name = "locationPanel";
            this.locationPanel.Size = new System.Drawing.Size(60, 682);
            this.locationPanel.TabIndex = 4;
            this.locationPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.locationPanel_Paint);
            this.locationPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.locationPanel_MouseClick);
            // 
            // TextDiff
            // 
            this.TextDiff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextDiff.Highlighting = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.TextDiff.LabelTextLeft = "Left Side Text";
            this.TextDiff.LabelTextRight = "Right Side Text";
            this.TextDiff.Location = new System.Drawing.Point(0, 0);
            this.TextDiff.Name = "TextDiff";
            this.TextDiff.Size = new System.Drawing.Size(906, 612);
            this.TextDiff.TabIndex = 1;
            this.TextDiff.TextLeftSide = "";
            this.TextDiff.TextRightSide = "";
            this.TextDiff.OnLineClicked += new ZidUtilities.CommonCode.Win.Controls.Diff.LineClicked(this.sideToSideTextComparer1_OnLineClicked);
            // 
            // LineDiff
            // 
            this.LineDiff.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LineDiff.Highlighting = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
            this.LineDiff.Location = new System.Drawing.Point(0, 612);
            this.LineDiff.LowerText = "";
            this.LineDiff.Name = "LineDiff";
            this.LineDiff.Padding = new System.Windows.Forms.Padding(2);
            this.LineDiff.Size = new System.Drawing.Size(906, 70);
            this.LineDiff.TabIndex = 0;
            this.LineDiff.UpperText = "";
            // 
            // DiffInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextDiff);
            this.Controls.Add(this.LineDiff);
            this.Controls.Add(this.locationPanel);
            this.Name = "DiffInspector";
            this.Size = new System.Drawing.Size(966, 682);
            this.ResumeLayout(false);

        }

        #endregion

        private SideToSideLineComparer LineDiff;
        private ZidUtilities.CommonCode.Win.Controls.Diff.SideToSideTextComparer TextDiff;
        private DoubleBufferedPanel locationPanel;
    }
}
