using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls
{
    partial class ToastForm
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
            this.components = new System.ComponentModel.Container();
            this.labTitle = new System.Windows.Forms.Label();
            this.labText = new System.Windows.Forms.Label();
            this.countDown = new System.Windows.Forms.Timer(this.components);
            this.animator = new System.Windows.Forms.Timer(this.components);
            this.pBar = new ZidUtilities.CommonCode.Win.Controls.VerticalProgressBar();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labTitle
            // 
            this.labTitle.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(39)))), ((int)(((byte)(82)))));
            this.labTitle.Location = new System.Drawing.Point(77, 7);
            this.labTitle.Name = "labTitle";
            this.labTitle.Size = new System.Drawing.Size(365, 23);
            this.labTitle.TabIndex = 1;
            this.labTitle.Text = "labelControl1";
            this.labTitle.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.labTitle.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // labText
            // 
            this.labText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(39)))), ((int)(((byte)(82)))));
            this.labText.Location = new System.Drawing.Point(77, 47);
            this.labText.Name = "labText";
            this.labText.Size = new System.Drawing.Size(403, 13);
            this.labText.TabIndex = 2;
            this.labText.Text = "labelControl2";
            this.labText.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.labText.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // countDown
            // 
            this.countDown.Interval = 50;
            this.countDown.Tick += new System.EventHandler(this.countDown_Tick);
            // 
            // animator
            // 
            this.animator.Interval = 10;
            this.animator.Tick += new System.EventHandler(this.animator_Tick);
            // 
            // pBar
            // 
            this.pBar.BackgroundColor = System.Drawing.SystemColors.Control;
            this.pBar.BarColor = System.Drawing.SystemColors.ActiveCaption;
            this.pBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pBar.Location = new System.Drawing.Point(0, 0);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(11, 70);
            this.pBar.TabIndex = 7;
            this.pBar.Value = 0;
            this.pBar.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.pBar.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackgroundImage = global::ZidUtilities.CommonCode.Win.Resources.Info64;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(12, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::ZidUtilities.CommonCode.Win.Resources.close_32x32;
            this.pictureBox1.Location = new System.Drawing.Point(448, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.btnClose_Click);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 70);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labText);
            this.Controls.Add(this.labTitle);
            this.Controls.Add(this.pBar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToastForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ToastForm";
            this.TopMost = true;
            this.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Label labTitle;
        private Label labText;
        private System.Windows.Forms.Timer countDown;
        private System.Windows.Forms.Timer animator;
        private PictureBox pictureBox1;
        private ZidUtilities.CommonCode.Win.Controls.VerticalProgressBar pBar;
        private PictureBox pictureBox2;
    }
}