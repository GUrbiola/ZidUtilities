namespace ZidUtilities.TesterWin
{
    partial class Form7
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
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.grpProcessing = new System.Windows.Forms.GroupBox();
            this.btnProcessingDeterminate = new System.Windows.Forms.Button();
            this.btnProcessingIndeterminate = new System.Windows.Forms.Button();
            this.grpComplexSelection = new System.Windows.Forms.GroupBox();
            this.btnComplexSelectionSingle = new System.Windows.Forms.Button();
            this.btnComplexSelection = new System.Windows.Forms.Button();
            this.grpMultiSelection = new System.Windows.Forms.GroupBox();
            this.btnMultiSelection = new System.Windows.Forms.Button();
            this.grpSingleSelection = new System.Windows.Forms.GroupBox();
            this.btnSingleSelectionDataTable = new System.Windows.Forms.Button();
            this.btnSingleSelection = new System.Windows.Forms.Button();
            this.grpTextInput = new System.Windows.Forms.GroupBox();
            this.btnPhoneInput = new System.Windows.Forms.Button();
            this.btnEmailInput = new System.Windows.Forms.Button();
            this.btnTextInput = new System.Windows.Forms.Button();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.btnTestAllStyles = new System.Windows.Forms.Button();
            this.chkRequired = new System.Windows.Forms.CheckBox();
            this.cmbInputFormat = new System.Windows.Forms.ComboBox();
            this.lblInputFormat = new System.Windows.Forms.Label();
            this.cmbDialogStyle = new System.Windows.Forms.ComboBox();
            this.lblDialogStyle = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.pnlResultsHeader = new System.Windows.Forms.Panel();
            this.btnClearResults = new System.Windows.Forms.Button();
            this.lblResults = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSqlConnection = new System.Windows.Forms.Button();
            this.grpMessageBox = new System.Windows.Forms.GroupBox();
            this.btnMessageBoxAllIcons = new System.Windows.Forms.Button();
            this.btnMessageBoxAbortRetryIgnore = new System.Windows.Forms.Button();
            this.btnMessageBoxRetryCancel = new System.Windows.Forms.Button();
            this.btnMessageBoxYesNoCancel = new System.Windows.Forms.Button();
            this.btnMessageBoxYesNo = new System.Windows.Forms.Button();
            this.btnMessageBoxOKCancel = new System.Windows.Forms.Button();
            this.btnMessageBoxOK = new System.Windows.Forms.Button();
            this.pnlLeft.SuspendLayout();
            this.grpProcessing.SuspendLayout();
            this.grpComplexSelection.SuspendLayout();
            this.grpMultiSelection.SuspendLayout();
            this.grpSingleSelection.SuspendLayout();
            this.grpTextInput.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlResultsHeader.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpMessageBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.AutoScroll = true;
            this.pnlLeft.Controls.Add(this.grpMessageBox);
            this.pnlLeft.Controls.Add(this.groupBox1);
            this.pnlLeft.Controls.Add(this.grpProcessing);
            this.pnlLeft.Controls.Add(this.grpComplexSelection);
            this.pnlLeft.Controls.Add(this.grpMultiSelection);
            this.pnlLeft.Controls.Add(this.grpSingleSelection);
            this.pnlLeft.Controls.Add(this.grpTextInput);
            this.pnlLeft.Controls.Add(this.grpOptions);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(10);
            this.pnlLeft.Size = new System.Drawing.Size(374, 661);
            this.pnlLeft.TabIndex = 0;
            // 
            // grpProcessing
            // 
            this.grpProcessing.Controls.Add(this.btnProcessingDeterminate);
            this.grpProcessing.Controls.Add(this.btnProcessingIndeterminate);
            this.grpProcessing.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpProcessing.Location = new System.Drawing.Point(10, 508);
            this.grpProcessing.Name = "grpProcessing";
            this.grpProcessing.Size = new System.Drawing.Size(337, 90);
            this.grpProcessing.TabIndex = 5;
            this.grpProcessing.TabStop = false;
            this.grpProcessing.Text = "Processing Dialog";
            // 
            // btnProcessingDeterminate
            // 
            this.btnProcessingDeterminate.Location = new System.Drawing.Point(10, 54);
            this.btnProcessingDeterminate.Name = "btnProcessingDeterminate";
            this.btnProcessingDeterminate.Size = new System.Drawing.Size(310, 25);
            this.btnProcessingDeterminate.TabIndex = 1;
            this.btnProcessingDeterminate.Text = "Show with Progress (Determinate)";
            this.btnProcessingDeterminate.UseVisualStyleBackColor = true;
            this.btnProcessingDeterminate.Click += new System.EventHandler(this.btnProcessingDeterminate_Click);
            // 
            // btnProcessingIndeterminate
            // 
            this.btnProcessingIndeterminate.Location = new System.Drawing.Point(10, 23);
            this.btnProcessingIndeterminate.Name = "btnProcessingIndeterminate";
            this.btnProcessingIndeterminate.Size = new System.Drawing.Size(310, 25);
            this.btnProcessingIndeterminate.TabIndex = 0;
            this.btnProcessingIndeterminate.Text = "Show Indeterminate";
            this.btnProcessingIndeterminate.UseVisualStyleBackColor = true;
            this.btnProcessingIndeterminate.Click += new System.EventHandler(this.btnProcessingIndeterminate_Click);
            // 
            // grpComplexSelection
            // 
            this.grpComplexSelection.Controls.Add(this.btnComplexSelectionSingle);
            this.grpComplexSelection.Controls.Add(this.btnComplexSelection);
            this.grpComplexSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpComplexSelection.Location = new System.Drawing.Point(10, 418);
            this.grpComplexSelection.Name = "grpComplexSelection";
            this.grpComplexSelection.Size = new System.Drawing.Size(337, 90);
            this.grpComplexSelection.TabIndex = 4;
            this.grpComplexSelection.TabStop = false;
            this.grpComplexSelection.Text = "Complex Object Selection Dialog";
            // 
            // btnComplexSelectionSingle
            // 
            this.btnComplexSelectionSingle.Location = new System.Drawing.Point(10, 54);
            this.btnComplexSelectionSingle.Name = "btnComplexSelectionSingle";
            this.btnComplexSelectionSingle.Size = new System.Drawing.Size(310, 25);
            this.btnComplexSelectionSingle.TabIndex = 1;
            this.btnComplexSelectionSingle.Text = "Single Selection (List<Product>)";
            this.btnComplexSelectionSingle.UseVisualStyleBackColor = true;
            this.btnComplexSelectionSingle.Click += new System.EventHandler(this.btnComplexSelectionSingle_Click);
            // 
            // btnComplexSelection
            // 
            this.btnComplexSelection.Location = new System.Drawing.Point(10, 23);
            this.btnComplexSelection.Name = "btnComplexSelection";
            this.btnComplexSelection.Size = new System.Drawing.Size(310, 25);
            this.btnComplexSelection.TabIndex = 0;
            this.btnComplexSelection.Text = "Multi Selection (DataTable)";
            this.btnComplexSelection.UseVisualStyleBackColor = true;
            this.btnComplexSelection.Click += new System.EventHandler(this.btnComplexSelection_Click);
            // 
            // grpMultiSelection
            // 
            this.grpMultiSelection.Controls.Add(this.btnMultiSelection);
            this.grpMultiSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMultiSelection.Location = new System.Drawing.Point(10, 358);
            this.grpMultiSelection.Name = "grpMultiSelection";
            this.grpMultiSelection.Size = new System.Drawing.Size(337, 60);
            this.grpMultiSelection.TabIndex = 3;
            this.grpMultiSelection.TabStop = false;
            this.grpMultiSelection.Text = "Multi Selection Dialog";
            // 
            // btnMultiSelection
            // 
            this.btnMultiSelection.Location = new System.Drawing.Point(10, 23);
            this.btnMultiSelection.Name = "btnMultiSelection";
            this.btnMultiSelection.Size = new System.Drawing.Size(310, 25);
            this.btnMultiSelection.TabIndex = 0;
            this.btnMultiSelection.Text = "Select Multiple Items (List<string>)";
            this.btnMultiSelection.UseVisualStyleBackColor = true;
            this.btnMultiSelection.Click += new System.EventHandler(this.btnMultiSelection_Click);
            // 
            // grpSingleSelection
            // 
            this.grpSingleSelection.Controls.Add(this.btnSingleSelectionDataTable);
            this.grpSingleSelection.Controls.Add(this.btnSingleSelection);
            this.grpSingleSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSingleSelection.Location = new System.Drawing.Point(10, 268);
            this.grpSingleSelection.Name = "grpSingleSelection";
            this.grpSingleSelection.Size = new System.Drawing.Size(337, 90);
            this.grpSingleSelection.TabIndex = 2;
            this.grpSingleSelection.TabStop = false;
            this.grpSingleSelection.Text = "Single Selection Dialog";
            // 
            // btnSingleSelectionDataTable
            // 
            this.btnSingleSelectionDataTable.Location = new System.Drawing.Point(10, 54);
            this.btnSingleSelectionDataTable.Name = "btnSingleSelectionDataTable";
            this.btnSingleSelectionDataTable.Size = new System.Drawing.Size(310, 25);
            this.btnSingleSelectionDataTable.TabIndex = 1;
            this.btnSingleSelectionDataTable.Text = "Select from DataTable";
            this.btnSingleSelectionDataTable.UseVisualStyleBackColor = true;
            this.btnSingleSelectionDataTable.Click += new System.EventHandler(this.btnSingleSelectionDataTable_Click);
            // 
            // btnSingleSelection
            // 
            this.btnSingleSelection.Location = new System.Drawing.Point(10, 23);
            this.btnSingleSelection.Name = "btnSingleSelection";
            this.btnSingleSelection.Size = new System.Drawing.Size(310, 25);
            this.btnSingleSelection.TabIndex = 0;
            this.btnSingleSelection.Text = "Select from List<string>";
            this.btnSingleSelection.UseVisualStyleBackColor = true;
            this.btnSingleSelection.Click += new System.EventHandler(this.btnSingleSelection_Click);
            // 
            // grpTextInput
            // 
            this.grpTextInput.Controls.Add(this.btnPhoneInput);
            this.grpTextInput.Controls.Add(this.btnEmailInput);
            this.grpTextInput.Controls.Add(this.btnTextInput);
            this.grpTextInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTextInput.Location = new System.Drawing.Point(10, 148);
            this.grpTextInput.Name = "grpTextInput";
            this.grpTextInput.Size = new System.Drawing.Size(337, 120);
            this.grpTextInput.TabIndex = 1;
            this.grpTextInput.TabStop = false;
            this.grpTextInput.Text = "Text Input Dialog";
            // 
            // btnPhoneInput
            // 
            this.btnPhoneInput.Location = new System.Drawing.Point(10, 85);
            this.btnPhoneInput.Name = "btnPhoneInput";
            this.btnPhoneInput.Size = new System.Drawing.Size(310, 25);
            this.btnPhoneInput.TabIndex = 2;
            this.btnPhoneInput.Text = "Phone Number Input (Required)";
            this.btnPhoneInput.UseVisualStyleBackColor = true;
            this.btnPhoneInput.Click += new System.EventHandler(this.btnPhoneInput_Click);
            // 
            // btnEmailInput
            // 
            this.btnEmailInput.Location = new System.Drawing.Point(10, 54);
            this.btnEmailInput.Name = "btnEmailInput";
            this.btnEmailInput.Size = new System.Drawing.Size(310, 25);
            this.btnEmailInput.TabIndex = 1;
            this.btnEmailInput.Text = "Email Input (Required)";
            this.btnEmailInput.UseVisualStyleBackColor = true;
            this.btnEmailInput.Click += new System.EventHandler(this.btnEmailInput_Click);
            // 
            // btnTextInput
            // 
            this.btnTextInput.Location = new System.Drawing.Point(10, 23);
            this.btnTextInput.Name = "btnTextInput";
            this.btnTextInput.Size = new System.Drawing.Size(310, 25);
            this.btnTextInput.TabIndex = 0;
            this.btnTextInput.Text = "Basic Text Input";
            this.btnTextInput.UseVisualStyleBackColor = true;
            this.btnTextInput.Click += new System.EventHandler(this.btnTextInput_Click);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.btnTestAllStyles);
            this.grpOptions.Controls.Add(this.chkRequired);
            this.grpOptions.Controls.Add(this.cmbInputFormat);
            this.grpOptions.Controls.Add(this.lblInputFormat);
            this.grpOptions.Controls.Add(this.cmbDialogStyle);
            this.grpOptions.Controls.Add(this.lblDialogStyle);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(10, 10);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(337, 138);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Global Options";
            // 
            // btnTestAllStyles
            // 
            this.btnTestAllStyles.Location = new System.Drawing.Point(10, 101);
            this.btnTestAllStyles.Name = "btnTestAllStyles";
            this.btnTestAllStyles.Size = new System.Drawing.Size(310, 25);
            this.btnTestAllStyles.TabIndex = 5;
            this.btnTestAllStyles.Text = "Test All Styles";
            this.btnTestAllStyles.UseVisualStyleBackColor = true;
            this.btnTestAllStyles.Click += new System.EventHandler(this.btnTestAllStyles_Click);
            // 
            // chkRequired
            // 
            this.chkRequired.AutoSize = true;
            this.chkRequired.Location = new System.Drawing.Point(220, 78);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(69, 17);
            this.chkRequired.TabIndex = 4;
            this.chkRequired.Text = "Required";
            this.chkRequired.UseVisualStyleBackColor = true;
            // 
            // cmbInputFormat
            // 
            this.cmbInputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInputFormat.FormattingEnabled = true;
            this.cmbInputFormat.Location = new System.Drawing.Point(93, 75);
            this.cmbInputFormat.Name = "cmbInputFormat";
            this.cmbInputFormat.Size = new System.Drawing.Size(121, 21);
            this.cmbInputFormat.TabIndex = 3;
            // 
            // lblInputFormat
            // 
            this.lblInputFormat.AutoSize = true;
            this.lblInputFormat.Location = new System.Drawing.Point(10, 78);
            this.lblInputFormat.Name = "lblInputFormat";
            this.lblInputFormat.Size = new System.Drawing.Size(69, 13);
            this.lblInputFormat.TabIndex = 2;
            this.lblInputFormat.Text = "Input Format:";
            // 
            // cmbDialogStyle
            // 
            this.cmbDialogStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDialogStyle.FormattingEnabled = true;
            this.cmbDialogStyle.Location = new System.Drawing.Point(93, 48);
            this.cmbDialogStyle.Name = "cmbDialogStyle";
            this.cmbDialogStyle.Size = new System.Drawing.Size(227, 21);
            this.cmbDialogStyle.TabIndex = 1;
            //
            // lblDialogStyle
            //
            this.lblDialogStyle.AutoSize = true;
            this.lblDialogStyle.Location = new System.Drawing.Point(10, 51);
            this.lblDialogStyle.Name = "lblDialogStyle";
            this.lblDialogStyle.Size = new System.Drawing.Size(66, 13);
            this.lblDialogStyle.TabIndex = 0;
            this.lblDialogStyle.Text = "Theme:";
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.txtResults);
            this.pnlRight.Controls.Add(this.pnlResultsHeader);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(374, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(10);
            this.pnlRight.Size = new System.Drawing.Size(807, 661);
            this.pnlRight.TabIndex = 1;
            // 
            // txtResults
            // 
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResults.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResults.Location = new System.Drawing.Point(10, 50);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(787, 601);
            this.txtResults.TabIndex = 1;
            this.txtResults.WordWrap = false;
            // 
            // pnlResultsHeader
            // 
            this.pnlResultsHeader.Controls.Add(this.btnClearResults);
            this.pnlResultsHeader.Controls.Add(this.lblResults);
            this.pnlResultsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResultsHeader.Location = new System.Drawing.Point(10, 10);
            this.pnlResultsHeader.Name = "pnlResultsHeader";
            this.pnlResultsHeader.Size = new System.Drawing.Size(787, 40);
            this.pnlResultsHeader.TabIndex = 0;
            // 
            // btnClearResults
            // 
            this.btnClearResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearResults.Location = new System.Drawing.Point(700, 8);
            this.btnClearResults.Name = "btnClearResults";
            this.btnClearResults.Size = new System.Drawing.Size(80, 25);
            this.btnClearResults.TabIndex = 1;
            this.btnClearResults.Text = "Clear";
            this.btnClearResults.UseVisualStyleBackColor = true;
            this.btnClearResults.Click += new System.EventHandler(this.btnClearResults_Click);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResults.Location = new System.Drawing.Point(3, 10);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(64, 21);
            this.lblResults.TabIndex = 0;
            this.lblResults.Text = "Results";
            // 
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.btnSqlConnection);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(10, 858);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 60);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sql Connection Dialog";
            //
            // btnSqlConnection
            //
            this.btnSqlConnection.Location = new System.Drawing.Point(10, 23);
            this.btnSqlConnection.Name = "btnSqlConnection";
            this.btnSqlConnection.Size = new System.Drawing.Size(310, 25);
            this.btnSqlConnection.TabIndex = 0;
            this.btnSqlConnection.Text = "Show MS SQL Connection Dialog";
            this.btnSqlConnection.UseVisualStyleBackColor = true;
            this.btnSqlConnection.Click += new System.EventHandler(this.btnSqlConnection_Click);
            //
            // grpMessageBox
            //
            this.grpMessageBox.Controls.Add(this.btnMessageBoxAllIcons);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxAbortRetryIgnore);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxRetryCancel);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxYesNoCancel);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxYesNo);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxOKCancel);
            this.grpMessageBox.Controls.Add(this.btnMessageBoxOK);
            this.grpMessageBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMessageBox.Location = new System.Drawing.Point(10, 598);
            this.grpMessageBox.Name = "grpMessageBox";
            this.grpMessageBox.Size = new System.Drawing.Size(337, 260);
            this.grpMessageBox.TabIndex = 7;
            this.grpMessageBox.TabStop = false;
            this.grpMessageBox.Text = "MessageBox Dialog";
            //
            // btnMessageBoxAllIcons
            //
            this.btnMessageBoxAllIcons.Location = new System.Drawing.Point(10, 220);
            this.btnMessageBoxAllIcons.Name = "btnMessageBoxAllIcons";
            this.btnMessageBoxAllIcons.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxAllIcons.TabIndex = 6;
            this.btnMessageBoxAllIcons.Text = "Show All Icons";
            this.btnMessageBoxAllIcons.UseVisualStyleBackColor = true;
            this.btnMessageBoxAllIcons.Click += new System.EventHandler(this.btnMessageBoxAllIcons_Click);
            //
            // btnMessageBoxAbortRetryIgnore
            //
            this.btnMessageBoxAbortRetryIgnore.Location = new System.Drawing.Point(10, 189);
            this.btnMessageBoxAbortRetryIgnore.Name = "btnMessageBoxAbortRetryIgnore";
            this.btnMessageBoxAbortRetryIgnore.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxAbortRetryIgnore.TabIndex = 5;
            this.btnMessageBoxAbortRetryIgnore.Text = "AbortRetryIgnore (Warning)";
            this.btnMessageBoxAbortRetryIgnore.UseVisualStyleBackColor = true;
            this.btnMessageBoxAbortRetryIgnore.Click += new System.EventHandler(this.btnMessageBoxAbortRetryIgnore_Click);
            //
            // btnMessageBoxRetryCancel
            //
            this.btnMessageBoxRetryCancel.Location = new System.Drawing.Point(10, 158);
            this.btnMessageBoxRetryCancel.Name = "btnMessageBoxRetryCancel";
            this.btnMessageBoxRetryCancel.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxRetryCancel.TabIndex = 4;
            this.btnMessageBoxRetryCancel.Text = "RetryCancel (Error)";
            this.btnMessageBoxRetryCancel.UseVisualStyleBackColor = true;
            this.btnMessageBoxRetryCancel.Click += new System.EventHandler(this.btnMessageBoxRetryCancel_Click);
            //
            // btnMessageBoxYesNoCancel
            //
            this.btnMessageBoxYesNoCancel.Location = new System.Drawing.Point(10, 127);
            this.btnMessageBoxYesNoCancel.Name = "btnMessageBoxYesNoCancel";
            this.btnMessageBoxYesNoCancel.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxYesNoCancel.TabIndex = 3;
            this.btnMessageBoxYesNoCancel.Text = "YesNoCancel (Question)";
            this.btnMessageBoxYesNoCancel.UseVisualStyleBackColor = true;
            this.btnMessageBoxYesNoCancel.Click += new System.EventHandler(this.btnMessageBoxYesNoCancel_Click);
            //
            // btnMessageBoxYesNo
            //
            this.btnMessageBoxYesNo.Location = new System.Drawing.Point(10, 96);
            this.btnMessageBoxYesNo.Name = "btnMessageBoxYesNo";
            this.btnMessageBoxYesNo.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxYesNo.TabIndex = 2;
            this.btnMessageBoxYesNo.Text = "YesNo (Question)";
            this.btnMessageBoxYesNo.UseVisualStyleBackColor = true;
            this.btnMessageBoxYesNo.Click += new System.EventHandler(this.btnMessageBoxYesNo_Click);
            //
            // btnMessageBoxOKCancel
            //
            this.btnMessageBoxOKCancel.Location = new System.Drawing.Point(10, 65);
            this.btnMessageBoxOKCancel.Name = "btnMessageBoxOKCancel";
            this.btnMessageBoxOKCancel.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxOKCancel.TabIndex = 1;
            this.btnMessageBoxOKCancel.Text = "OKCancel (Question)";
            this.btnMessageBoxOKCancel.UseVisualStyleBackColor = true;
            this.btnMessageBoxOKCancel.Click += new System.EventHandler(this.btnMessageBoxOKCancel_Click);
            //
            // btnMessageBoxOK
            //
            this.btnMessageBoxOK.Location = new System.Drawing.Point(10, 34);
            this.btnMessageBoxOK.Name = "btnMessageBoxOK";
            this.btnMessageBoxOK.Size = new System.Drawing.Size(310, 25);
            this.btnMessageBoxOK.TabIndex = 0;
            this.btnMessageBoxOK.Text = "OK (Information)";
            this.btnMessageBoxOK.UseVisualStyleBackColor = true;
            this.btnMessageBoxOK.Click += new System.EventHandler(this.btnMessageBoxOK_Click);
            // 
            // Form7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 661);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "Form7";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generic Dialog Forms Test";
            this.pnlLeft.ResumeLayout(false);
            this.grpProcessing.ResumeLayout(false);
            this.grpComplexSelection.ResumeLayout(false);
            this.grpMultiSelection.ResumeLayout(false);
            this.grpSingleSelection.ResumeLayout(false);
            this.grpTextInput.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlResultsHeader.ResumeLayout(false);
            this.pnlResultsHeader.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.grpMessageBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.ComboBox cmbDialogStyle;
        private System.Windows.Forms.Label lblDialogStyle;
        private System.Windows.Forms.ComboBox cmbInputFormat;
        private System.Windows.Forms.Label lblInputFormat;
        private System.Windows.Forms.CheckBox chkRequired;
        private System.Windows.Forms.GroupBox grpTextInput;
        private System.Windows.Forms.Button btnTextInput;
        private System.Windows.Forms.Button btnEmailInput;
        private System.Windows.Forms.Button btnPhoneInput;
        private System.Windows.Forms.GroupBox grpSingleSelection;
        private System.Windows.Forms.Button btnSingleSelection;
        private System.Windows.Forms.Button btnSingleSelectionDataTable;
        private System.Windows.Forms.GroupBox grpMultiSelection;
        private System.Windows.Forms.Button btnMultiSelection;
        private System.Windows.Forms.GroupBox grpComplexSelection;
        private System.Windows.Forms.Button btnComplexSelection;
        private System.Windows.Forms.Button btnComplexSelectionSingle;
        private System.Windows.Forms.GroupBox grpProcessing;
        private System.Windows.Forms.Button btnProcessingIndeterminate;
        private System.Windows.Forms.Button btnProcessingDeterminate;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Panel pnlResultsHeader;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.Button btnClearResults;
        private System.Windows.Forms.Button btnTestAllStyles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSqlConnection;
        private System.Windows.Forms.GroupBox grpMessageBox;
        private System.Windows.Forms.Button btnMessageBoxOK;
        private System.Windows.Forms.Button btnMessageBoxOKCancel;
        private System.Windows.Forms.Button btnMessageBoxYesNo;
        private System.Windows.Forms.Button btnMessageBoxYesNoCancel;
        private System.Windows.Forms.Button btnMessageBoxRetryCancel;
        private System.Windows.Forms.Button btnMessageBoxAbortRetryIgnore;
        private System.Windows.Forms.Button btnMessageBoxAllIcons;
    }
}
