namespace ZidUtilities.TesterWin
{
    partial class FormTokenSelectTest
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.cmbThemes = new System.Windows.Forms.ComboBox();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.logGroup = new System.Windows.Forms.GroupBox();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.selectedGroup = new System.Windows.Forms.GroupBox();
            this.selectedTextsLabel = new System.Windows.Forms.Label();
            this.selectedValuesLabel = new System.Windows.Forms.Label();
            this.controlsGroup = new System.Windows.Forms.GroupBox();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnAddProgrammatic = new System.Windows.Forms.Button();
            this.btnClearCountries = new System.Windows.Forms.Button();
            this.btnGetValues = new System.Windows.Forms.Button();
            this.maxTokensGroup = new System.Windows.Forms.GroupBox();
            this.maxTokensDesc = new System.Windows.Forms.Label();
            this.singleGroup = new System.Windows.Forms.GroupBox();
            this.singleDesc = new System.Windows.Forms.Label();
            this.genresGroup = new System.Windows.Forms.GroupBox();
            this.genresDesc = new System.Windows.Forms.Label();
            this.countriesGroup = new System.Windows.Forms.GroupBox();
            this.countriesDesc = new System.Windows.Forms.Label();
            this.themeGroup = new System.Windows.Forms.GroupBox();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.themeLabel = new System.Windows.Forms.Label();
            this.tokenSelectMaxTokens = new ZidUtilities.CommonCode.Win.Controls.TokenSelect();
            this.tokenSelectSingle = new ZidUtilities.CommonCode.Win.Controls.TokenSelect();
            this.tokenSelectGenres = new ZidUtilities.CommonCode.Win.Controls.TokenSelect();
            this.tokenSelectCountries = new ZidUtilities.CommonCode.Win.Controls.TokenSelect();
            this.themeManager1 = new ZidUtilities.CommonCode.Win.Controls.ThemeManager(this.components);
            this.headerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.logGroup.SuspendLayout();
            this.selectedGroup.SuspendLayout();
            this.controlsGroup.SuspendLayout();
            this.maxTokensGroup.SuspendLayout();
            this.singleGroup.SuspendLayout();
            this.genresGroup.SuspendLayout();
            this.countriesGroup.SuspendLayout();
            this.themeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.headerPanel.Controls.Add(this.cmbThemes);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(920, 80);
            this.headerPanel.TabIndex = 0;
            // 
            // cmbThemes
            // 
            this.cmbThemes.FormattingEnabled = true;
            this.cmbThemes.Location = new System.Drawing.Point(425, 20);
            this.cmbThemes.Name = "cmbThemes";
            this.cmbThemes.Size = new System.Drawing.Size(465, 21);
            this.cmbThemes.TabIndex = 2;
            this.cmbThemes.SelectedIndexChanged += new System.EventHandler(this.themeComboBox_SelectedIndexChanged);
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Verdana", 10F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.White;
            this.subtitleLabel.Location = new System.Drawing.Point(20, 45);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(345, 17);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "Test autocomplete dropdown with token display";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(20, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(325, 26);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "TokenSelect Control Test";
            // 
            // contentPanel
            // 
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.contentPanel.Controls.Add(this.logGroup);
            this.contentPanel.Controls.Add(this.selectedGroup);
            this.contentPanel.Controls.Add(this.controlsGroup);
            this.contentPanel.Controls.Add(this.maxTokensGroup);
            this.contentPanel.Controls.Add(this.singleGroup);
            this.contentPanel.Controls.Add(this.genresGroup);
            this.contentPanel.Controls.Add(this.countriesGroup);
            this.contentPanel.Controls.Add(this.themeGroup);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 80);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.contentPanel.Size = new System.Drawing.Size(920, 769);
            this.contentPanel.TabIndex = 1;
            // 
            // logGroup
            // 
            this.logGroup.Controls.Add(this.logListBox);
            this.logGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.logGroup.Location = new System.Drawing.Point(10, 720);
            this.logGroup.Name = "logGroup";
            this.logGroup.Size = new System.Drawing.Size(880, 200);
            this.logGroup.TabIndex = 7;
            this.logGroup.TabStop = false;
            this.logGroup.Text = "Event Log";
            // 
            // logListBox
            // 
            this.logListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.logListBox.Font = new System.Drawing.Font("Consolas", 8F);
            this.logListBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.logListBox.FormattingEnabled = true;
            this.logListBox.Location = new System.Drawing.Point(15, 25);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(850, 160);
            this.logListBox.TabIndex = 0;
            // 
            // selectedGroup
            // 
            this.selectedGroup.Controls.Add(this.selectedTextsLabel);
            this.selectedGroup.Controls.Add(this.selectedValuesLabel);
            this.selectedGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.selectedGroup.Location = new System.Drawing.Point(10, 630);
            this.selectedGroup.Name = "selectedGroup";
            this.selectedGroup.Size = new System.Drawing.Size(880, 80);
            this.selectedGroup.TabIndex = 6;
            this.selectedGroup.TabStop = false;
            this.selectedGroup.Text = "Current Selection";
            // 
            // selectedTextsLabel
            // 
            this.selectedTextsLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
            this.selectedTextsLabel.Location = new System.Drawing.Point(15, 48);
            this.selectedTextsLabel.Name = "selectedTextsLabel";
            this.selectedTextsLabel.Size = new System.Drawing.Size(850, 20);
            this.selectedTextsLabel.TabIndex = 1;
            this.selectedTextsLabel.Text = "Texts: None";
            // 
            // selectedValuesLabel
            // 
            this.selectedValuesLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
            this.selectedValuesLabel.Location = new System.Drawing.Point(15, 25);
            this.selectedValuesLabel.Name = "selectedValuesLabel";
            this.selectedValuesLabel.Size = new System.Drawing.Size(850, 20);
            this.selectedValuesLabel.TabIndex = 0;
            this.selectedValuesLabel.Text = "Values: None";
            // 
            // controlsGroup
            // 
            this.controlsGroup.Controls.Add(this.chkReadOnly);
            this.controlsGroup.Controls.Add(this.btnClearLog);
            this.controlsGroup.Controls.Add(this.btnAddProgrammatic);
            this.controlsGroup.Controls.Add(this.btnClearCountries);
            this.controlsGroup.Controls.Add(this.btnGetValues);
            this.controlsGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.controlsGroup.Location = new System.Drawing.Point(10, 520);
            this.controlsGroup.Name = "controlsGroup";
            this.controlsGroup.Size = new System.Drawing.Size(880, 100);
            this.controlsGroup.TabIndex = 5;
            this.controlsGroup.TabStop = false;
            this.controlsGroup.Text = "Control Panel";
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.AutoSize = true;
            this.chkReadOnly.Font = new System.Drawing.Font("Verdana", 9F);
            this.chkReadOnly.Location = new System.Drawing.Point(655, 35);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(156, 18);
            this.chkReadOnly.TabIndex = 4;
            this.chkReadOnly.Text = "Countries Read-Only";
            this.chkReadOnly.UseVisualStyleBackColor = true;
            this.chkReadOnly.CheckedChanged += new System.EventHandler(this.chkReadOnly_CheckedChanged);
            // 
            // btnClearLog
            // 
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnClearLog.ForeColor = System.Drawing.Color.White;
            this.btnClearLog.Location = new System.Drawing.Point(495, 30);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(150, 30);
            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnAddProgrammatic
            // 
            this.btnAddProgrammatic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAddProgrammatic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddProgrammatic.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnAddProgrammatic.ForeColor = System.Drawing.Color.White;
            this.btnAddProgrammatic.Location = new System.Drawing.Point(335, 30);
            this.btnAddProgrammatic.Name = "btnAddProgrammatic";
            this.btnAddProgrammatic.Size = new System.Drawing.Size(150, 30);
            this.btnAddProgrammatic.TabIndex = 2;
            this.btnAddProgrammatic.Text = "Add Token (Code)";
            this.btnAddProgrammatic.UseVisualStyleBackColor = false;
            this.btnAddProgrammatic.Click += new System.EventHandler(this.btnAddProgrammatic_Click);
            // 
            // btnClearCountries
            // 
            this.btnClearCountries.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnClearCountries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearCountries.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnClearCountries.ForeColor = System.Drawing.Color.White;
            this.btnClearCountries.Location = new System.Drawing.Point(175, 30);
            this.btnClearCountries.Name = "btnClearCountries";
            this.btnClearCountries.Size = new System.Drawing.Size(150, 30);
            this.btnClearCountries.TabIndex = 1;
            this.btnClearCountries.Text = "Clear Countries";
            this.btnClearCountries.UseVisualStyleBackColor = false;
            this.btnClearCountries.Click += new System.EventHandler(this.btnClearCountries_Click);
            // 
            // btnGetValues
            // 
            this.btnGetValues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnGetValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetValues.Font = new System.Drawing.Font("Verdana", 9F);
            this.btnGetValues.ForeColor = System.Drawing.Color.White;
            this.btnGetValues.Location = new System.Drawing.Point(15, 30);
            this.btnGetValues.Name = "btnGetValues";
            this.btnGetValues.Size = new System.Drawing.Size(150, 30);
            this.btnGetValues.TabIndex = 0;
            this.btnGetValues.Text = "Get Selected Values";
            this.btnGetValues.UseVisualStyleBackColor = false;
            this.btnGetValues.Click += new System.EventHandler(this.btnGetValues_Click);
            // 
            // maxTokensGroup
            // 
            this.maxTokensGroup.Controls.Add(this.tokenSelectMaxTokens);
            this.maxTokensGroup.Controls.Add(this.maxTokensDesc);
            this.maxTokensGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.maxTokensGroup.Location = new System.Drawing.Point(10, 390);
            this.maxTokensGroup.Name = "maxTokensGroup";
            this.maxTokensGroup.Size = new System.Drawing.Size(880, 120);
            this.maxTokensGroup.TabIndex = 4;
            this.maxTokensGroup.TabStop = false;
            this.maxTokensGroup.Text = "Test 4: Team Members (Max 3 Tokens)";
            // 
            // maxTokensDesc
            // 
            this.maxTokensDesc.Font = new System.Drawing.Font("Verdana", 8F);
            this.maxTokensDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.maxTokensDesc.Location = new System.Drawing.Point(15, 25);
            this.maxTokensDesc.Name = "maxTokensDesc";
            this.maxTokensDesc.Size = new System.Drawing.Size(850, 20);
            this.maxTokensDesc.TabIndex = 0;
            this.maxTokensDesc.Text = "Select up to 3 team members. Control prevents adding more than the maximum.";
            // 
            // singleGroup
            // 
            this.singleGroup.Controls.Add(this.tokenSelectSingle);
            this.singleGroup.Controls.Add(this.singleDesc);
            this.singleGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.singleGroup.Location = new System.Drawing.Point(10, 260);
            this.singleGroup.Name = "singleGroup";
            this.singleGroup.Size = new System.Drawing.Size(880, 120);
            this.singleGroup.TabIndex = 3;
            this.singleGroup.TabStop = false;
            this.singleGroup.Text = "Test 3: Priority (Single Selection Only)";
            // 
            // singleDesc
            // 
            this.singleDesc.Font = new System.Drawing.Font("Verdana", 8F);
            this.singleDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.singleDesc.Location = new System.Drawing.Point(15, 25);
            this.singleDesc.Name = "singleDesc";
            this.singleDesc.Size = new System.Drawing.Size(850, 20);
            this.singleDesc.TabIndex = 0;
            this.singleDesc.Text = "Select priority level. Only one selection allowed - selecting another removes the" +
    " previous one.";
            // 
            // genresGroup
            // 
            this.genresGroup.Controls.Add(this.tokenSelectGenres);
            this.genresGroup.Controls.Add(this.genresDesc);
            this.genresGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.genresGroup.Location = new System.Drawing.Point(10, 130);
            this.genresGroup.Name = "genresGroup";
            this.genresGroup.Size = new System.Drawing.Size(880, 120);
            this.genresGroup.TabIndex = 2;
            this.genresGroup.TabStop = false;
            this.genresGroup.Text = "Test 2: Movie Genres (Multiple Selection - Separate Lists)";
            // 
            // genresDesc
            // 
            this.genresDesc.Font = new System.Drawing.Font("Verdana", 8F);
            this.genresDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.genresDesc.Location = new System.Drawing.Point(15, 25);
            this.genresDesc.Name = "genresDesc";
            this.genresDesc.Size = new System.Drawing.Size(850, 20);
            this.genresDesc.TabIndex = 0;
            this.genresDesc.Text = "Select movie genres. Uses separate display and value lists. Minimum 2 characters " +
    "to show dropdown.";
            // 
            // countriesGroup
            // 
            this.countriesGroup.Controls.Add(this.tokenSelectCountries);
            this.countriesGroup.Controls.Add(this.countriesDesc);
            this.countriesGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.countriesGroup.Location = new System.Drawing.Point(10, 10);
            this.countriesGroup.Name = "countriesGroup";
            this.countriesGroup.Size = new System.Drawing.Size(880, 110);
            this.countriesGroup.TabIndex = 1;
            this.countriesGroup.TabStop = false;
            this.countriesGroup.Text = "Test 1: Countries (Multiple Selection - Dictionary)";
            // 
            // countriesDesc
            // 
            this.countriesDesc.Font = new System.Drawing.Font("Verdana", 8F);
            this.countriesDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.countriesDesc.Location = new System.Drawing.Point(15, 25);
            this.countriesDesc.Name = "countriesDesc";
            this.countriesDesc.Size = new System.Drawing.Size(850, 20);
            this.countriesDesc.TabIndex = 0;
            this.countriesDesc.Text = "Type to search and select multiple countries. Uses Dictionary<string, object> dat" +
    "a source.";
            // 
            // themeGroup
            // 
            this.themeGroup.Controls.Add(this.themeComboBox);
            this.themeGroup.Controls.Add(this.themeLabel);
            this.themeGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.themeGroup.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.themeGroup.Location = new System.Drawing.Point(20, 20);
            this.themeGroup.Name = "themeGroup";
            this.themeGroup.Size = new System.Drawing.Size(870, 80);
            this.themeGroup.TabIndex = 0;
            this.themeGroup.TabStop = false;
            this.themeGroup.Text = "Theme Selection";
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.Font = new System.Drawing.Font("Verdana", 9F);
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(80, 27);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(200, 22);
            this.themeComboBox.TabIndex = 1;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.themeComboBox_SelectedIndexChanged);
            // 
            // themeLabel
            // 
            this.themeLabel.AutoSize = true;
            this.themeLabel.Font = new System.Drawing.Font("Verdana", 9F);
            this.themeLabel.Location = new System.Drawing.Point(15, 30);
            this.themeLabel.Name = "themeLabel";
            this.themeLabel.Size = new System.Drawing.Size(54, 14);
            this.themeLabel.TabIndex = 0;
            this.themeLabel.Text = "Theme:";
            // 
            // tokenSelectMaxTokens
            // 
            this.tokenSelectMaxTokens.BackColor = System.Drawing.Color.White;
            this.tokenSelectMaxTokens.Location = new System.Drawing.Point(15, 50);
            this.tokenSelectMaxTokens.MaxTokens = 3;
            this.tokenSelectMaxTokens.MinimumSize = new System.Drawing.Size(100, 30);
            this.tokenSelectMaxTokens.Name = "tokenSelectMaxTokens";
            this.tokenSelectMaxTokens.Placeholder = "Select up to 3 team members...";
            this.tokenSelectMaxTokens.Size = new System.Drawing.Size(850, 30);
            this.tokenSelectMaxTokens.TabIndex = 1;
            this.tokenSelectMaxTokens.OnTokenAdded += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenAdded);
            this.tokenSelectMaxTokens.OnTokenRemoved += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenRemoved);
            // 
            // tokenSelectSingle
            // 
            this.tokenSelectSingle.AllowMultipleSelection = false;
            this.tokenSelectSingle.BackColor = System.Drawing.Color.White;
            this.tokenSelectSingle.Location = new System.Drawing.Point(15, 50);
            this.tokenSelectSingle.MinimumCharactersForDropdown = 0;
            this.tokenSelectSingle.MinimumSize = new System.Drawing.Size(100, 30);
            this.tokenSelectSingle.Name = "tokenSelectSingle";
            this.tokenSelectSingle.Placeholder = "Select priority...";
            this.tokenSelectSingle.Size = new System.Drawing.Size(850, 30);
            this.tokenSelectSingle.TabIndex = 1;
            this.tokenSelectSingle.OnTokenAdded += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenAdded);
            this.tokenSelectSingle.OnTokenRemoved += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenRemoved);
            // 
            // tokenSelectGenres
            // 
            this.tokenSelectGenres.BackColor = System.Drawing.Color.White;
            this.tokenSelectGenres.Location = new System.Drawing.Point(15, 50);
            this.tokenSelectGenres.MinimumCharactersForDropdown = 0;
            this.tokenSelectGenres.MinimumSize = new System.Drawing.Size(100, 30);
            this.tokenSelectGenres.Name = "tokenSelectGenres";
            this.tokenSelectGenres.Placeholder = "Type at least 2 characters to search genres...";
            this.tokenSelectGenres.Size = new System.Drawing.Size(850, 30);
            this.tokenSelectGenres.TabIndex = 1;
            this.tokenSelectGenres.OnTokenAdded += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenAdded);
            this.tokenSelectGenres.OnTokenRemoved += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenRemoved);
            // 
            // tokenSelectCountries
            // 
            this.tokenSelectCountries.BackColor = System.Drawing.Color.White;
            this.tokenSelectCountries.Location = new System.Drawing.Point(15, 50);
            this.tokenSelectCountries.MinimumSize = new System.Drawing.Size(100, 30);
            this.tokenSelectCountries.Name = "tokenSelectCountries";
            this.tokenSelectCountries.Placeholder = "Type to search countries...";
            this.tokenSelectCountries.Size = new System.Drawing.Size(850, 30);
            this.tokenSelectCountries.TabIndex = 1;
            this.tokenSelectCountries.OnTokenAdded += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenAdded);
            this.tokenSelectCountries.OnTokenRemoved += new System.EventHandler<ZidUtilities.CommonCode.Win.Controls.TokenSelect.TokenEventArgs>(this.TokenSelect_OnTokenRemoved);
            // 
            // themeManager1
            // 
            this.themeManager1.ExcludedControlTypes.Add("ICSharpCode.TextEditor.TextEditorControl");
            this.themeManager1.ExcludedControlTypes.Add("ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor");
            this.themeManager1.ExcludedControlTypes.Add("ICSharpCode.TextEditor.TextEditorControl");
            this.themeManager1.ExcludedControlTypes.Add("ZidUtilities.CommonCode.ICSharpTextEditor.ExtendedEditor");
            this.themeManager1.ParentForm = this;
            this.themeManager1.Theme = ZidUtilities.CommonCode.Win.ZidThemes.Default;
            // 
            // FormTokenSelectTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 849);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "FormTokenSelectTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TokenSelect Control Test";
            this.Load += new System.EventHandler(this.FormTokenSelectTest_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            this.logGroup.ResumeLayout(false);
            this.selectedGroup.ResumeLayout(false);
            this.controlsGroup.ResumeLayout(false);
            this.controlsGroup.PerformLayout();
            this.maxTokensGroup.ResumeLayout(false);
            this.singleGroup.ResumeLayout(false);
            this.genresGroup.ResumeLayout(false);
            this.countriesGroup.ResumeLayout(false);
            this.themeGroup.ResumeLayout(false);
            this.themeGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.GroupBox themeGroup;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label themeLabel;
        private System.Windows.Forms.GroupBox countriesGroup;
        private CommonCode.Win.Controls.TokenSelect tokenSelectCountries;
        private System.Windows.Forms.Label countriesDesc;
        private System.Windows.Forms.GroupBox genresGroup;
        private CommonCode.Win.Controls.TokenSelect tokenSelectGenres;
        private System.Windows.Forms.Label genresDesc;
        private System.Windows.Forms.GroupBox singleGroup;
        private CommonCode.Win.Controls.TokenSelect tokenSelectSingle;
        private System.Windows.Forms.Label singleDesc;
        private System.Windows.Forms.GroupBox maxTokensGroup;
        private CommonCode.Win.Controls.TokenSelect tokenSelectMaxTokens;
        private System.Windows.Forms.Label maxTokensDesc;
        private System.Windows.Forms.GroupBox controlsGroup;
        private System.Windows.Forms.Button btnGetValues;
        private System.Windows.Forms.Button btnClearCountries;
        private System.Windows.Forms.Button btnAddProgrammatic;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkReadOnly;
        private System.Windows.Forms.GroupBox selectedGroup;
        private System.Windows.Forms.Label selectedValuesLabel;
        private System.Windows.Forms.Label selectedTextsLabel;
        private System.Windows.Forms.GroupBox logGroup;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.ComboBox cmbThemes;
        private CommonCode.Win.Controls.ThemeManager themeManager1;
    }
}
