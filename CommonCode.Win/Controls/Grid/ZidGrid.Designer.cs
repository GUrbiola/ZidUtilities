namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    partial class ZidGrid
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
            ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter.GridFilterFactories.DefaultGridFilterFactory defaultGridFilterFactory1 = new ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter.GridFilterFactories.DefaultGridFilterFactory();
            this.GridView = new System.Windows.Forms.DataGridView();
            this.filterExtender = new ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter.DataGridFilterExtender(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterExtender)).BeginInit();
            this.SuspendLayout();
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Location = new System.Drawing.Point(3, 21);
            this.GridView.Name = "GridView";
            this.GridView.ReadOnly = true;
            this.GridView.Size = new System.Drawing.Size(480, 392);
            this.GridView.TabIndex = 0;
            this.GridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridView_DataBindingComplete_1);
            this.GridView.SelectionChanged += new System.EventHandler(this.GridView_SelectionChanged);
            // 
            // filterExtender
            // 
            this.filterExtender.DataGridView = this.GridView;
            defaultGridFilterFactory1.CreateDistinctGridFilters = false;
            defaultGridFilterFactory1.DefaultGridFilterType = typeof(ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter.GridFilters.TextGridFilter);
            defaultGridFilterFactory1.DefaultShowDateInBetweenOperator = false;
            defaultGridFilterFactory1.DefaultShowNumericInBetweenOperator = false;
            defaultGridFilterFactory1.HandleEnumerationTypes = true;
            defaultGridFilterFactory1.MaximumDistinctValues = 20;
            this.filterExtender.FilterFactory = defaultGridFilterFactory1;
            // 
            // ZidGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GridView);
            this.Name = "ZidGrid";
            this.Size = new System.Drawing.Size(486, 413);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterExtender)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView GridView;
        private ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter.DataGridFilterExtender filterExtender;
    }
}
