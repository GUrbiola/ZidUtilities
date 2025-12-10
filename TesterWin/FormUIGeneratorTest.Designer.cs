using ZidUtilities.CommonCode.Win;

namespace ZidUtilities.TesterWin
{
    partial class FormUIGeneratorTest
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpInitialization = new System.Windows.Forms.GroupBox();
            this.btnInitialize = new System.Windows.Forms.Button();
            this.rdoDictionary = new System.Windows.Forms.RadioButton();
            this.rdoDataTable = new System.Windows.Forms.RadioButton();
            this.rdoSqlConnection = new System.Windows.Forms.RadioButton();
            this.grpConfiguration = new System.Windows.Forms.GroupBox();
            this.btnConfigureForeignKeys = new System.Windows.Forms.Button();
            this.btnConfigureExclusions = new System.Windows.Forms.Button();
            this.btnConfigureAliases = new System.Windows.Forms.Button();
            this.grpLayout = new System.Windows.Forms.GroupBox();
            this.lblCurrentLayout = new System.Windows.Forms.Label();
            this.btnSetLayoutHtml = new System.Windows.Forms.Button();
            this.btnSetLayoutColumns = new System.Windows.Forms.Button();
            this.btnSetLayoutAuto = new System.Windows.Forms.Button();
            this.grpDialogs = new System.Windows.Forms.GroupBox();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.btnShowDelete = new System.Windows.Forms.Button();
            this.btnShowUpdate = new System.Windows.Forms.Button();
            this.btnShowInsert = new System.Windows.Forms.Button();
            this.grpTestMethods = new System.Windows.Forms.GroupBox();
            this.btnClearConfig = new System.Windows.Forms.Button();
            this.btnTestGetters = new System.Windows.Forms.Button();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.grpInitialization.SuspendLayout();
            this.grpConfiguration.SuspendLayout();
            this.grpLayout.SuspendLayout();
            this.grpDialogs.SuspendLayout();
            this.grpTestMethods.SuspendLayout();
            this.grpResults.SuspendLayout();
            this.SuspendLayout();
            //
            // grpInitialization
            //
            this.grpInitialization.Controls.Add(this.btnInitialize);
            this.grpInitialization.Controls.Add(this.rdoDictionary);
            this.grpInitialization.Controls.Add(this.rdoDataTable);
            this.grpInitialization.Controls.Add(this.rdoSqlConnection);
            this.grpInitialization.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpInitialization.Location = new System.Drawing.Point(12, 12);
            this.grpInitialization.Name = "grpInitialization";
            this.grpInitialization.Size = new System.Drawing.Size(280, 140);
            this.grpInitialization.TabIndex = 0;
            this.grpInitialization.TabStop = false;
            this.grpInitialization.Text = "1. Initialization";
            //
            // btnInitialize
            //
            this.btnInitialize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnInitialize.Location = new System.Drawing.Point(15, 100);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(250, 30);
            this.btnInitialize.TabIndex = 3;
            this.btnInitialize.Text = "Initialize UIGenerator";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
            //
            // rdoDictionary
            //
            this.rdoDictionary.AutoSize = true;
            this.rdoDictionary.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoDictionary.Location = new System.Drawing.Point(15, 75);
            this.rdoDictionary.Name = "rdoDictionary";
            this.rdoDictionary.Size = new System.Drawing.Size(186, 19);
            this.rdoDictionary.TabIndex = 2;
            this.rdoDictionary.Text = "Dictionary (Products)";
            this.rdoDictionary.UseVisualStyleBackColor = true;
            //
            // rdoDataTable
            //
            this.rdoDataTable.AutoSize = true;
            this.rdoDataTable.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoDataTable.Location = new System.Drawing.Point(15, 50);
            this.rdoDataTable.Name = "rdoDataTable";
            this.rdoDataTable.Size = new System.Drawing.Size(130, 19);
            this.rdoDataTable.TabIndex = 1;
            this.rdoDataTable.Text = "DataTable (Users)";
            this.rdoDataTable.UseVisualStyleBackColor = true;
            //
            // rdoSqlConnection
            //
            this.rdoSqlConnection.AutoSize = true;
            this.rdoSqlConnection.Checked = true;
            this.rdoSqlConnection.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoSqlConnection.Location = new System.Drawing.Point(15, 25);
            this.rdoSqlConnection.Name = "rdoSqlConnection";
            this.rdoSqlConnection.Size = new System.Drawing.Size(211, 19);
            this.rdoSqlConnection.TabIndex = 0;
            this.rdoSqlConnection.TabStop = true;
            this.rdoSqlConnection.Text = "SqlConnection (ArticleProposal)";
            this.rdoSqlConnection.UseVisualStyleBackColor = true;
            //
            // grpConfiguration
            //
            this.grpConfiguration.Controls.Add(this.btnConfigureForeignKeys);
            this.grpConfiguration.Controls.Add(this.btnConfigureExclusions);
            this.grpConfiguration.Controls.Add(this.btnConfigureAliases);
            this.grpConfiguration.Enabled = false;
            this.grpConfiguration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpConfiguration.Location = new System.Drawing.Point(12, 158);
            this.grpConfiguration.Name = "grpConfiguration";
            this.grpConfiguration.Size = new System.Drawing.Size(280, 160);
            this.grpConfiguration.TabIndex = 1;
            this.grpConfiguration.TabStop = false;
            this.grpConfiguration.Text = "2. Configuration";
            //
            // btnConfigureForeignKeys
            //
            this.btnConfigureForeignKeys.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnConfigureForeignKeys.Location = new System.Drawing.Point(15, 110);
            this.btnConfigureForeignKeys.Name = "btnConfigureForeignKeys";
            this.btnConfigureForeignKeys.Size = new System.Drawing.Size(250, 35);
            this.btnConfigureForeignKeys.TabIndex = 2;
            this.btnConfigureForeignKeys.Text = "Configure Foreign Key Lookups";
            this.btnConfigureForeignKeys.UseVisualStyleBackColor = true;
            this.btnConfigureForeignKeys.Click += new System.EventHandler(this.btnConfigureForeignKeys_Click);
            //
            // btnConfigureExclusions
            //
            this.btnConfigureExclusions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnConfigureExclusions.Location = new System.Drawing.Point(15, 68);
            this.btnConfigureExclusions.Name = "btnConfigureExclusions";
            this.btnConfigureExclusions.Size = new System.Drawing.Size(250, 35);
            this.btnConfigureExclusions.TabIndex = 1;
            this.btnConfigureExclusions.Text = "Configure Exclusions (with defaults)";
            this.btnConfigureExclusions.UseVisualStyleBackColor = true;
            this.btnConfigureExclusions.Click += new System.EventHandler(this.btnConfigureExclusions_Click);
            //
            // btnConfigureAliases
            //
            this.btnConfigureAliases.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnConfigureAliases.Location = new System.Drawing.Point(15, 26);
            this.btnConfigureAliases.Name = "btnConfigureAliases";
            this.btnConfigureAliases.Size = new System.Drawing.Size(250, 35);
            this.btnConfigureAliases.TabIndex = 0;
            this.btnConfigureAliases.Text = "Configure Aliases";
            this.btnConfigureAliases.UseVisualStyleBackColor = true;
            this.btnConfigureAliases.Click += new System.EventHandler(this.btnConfigureAliases_Click);
            //
            // grpLayout
            //
            this.grpLayout.Controls.Add(this.lblCurrentLayout);
            this.grpLayout.Controls.Add(this.btnSetLayoutHtml);
            this.grpLayout.Controls.Add(this.btnSetLayoutColumns);
            this.grpLayout.Controls.Add(this.btnSetLayoutAuto);
            this.grpLayout.Enabled = false;
            this.grpLayout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpLayout.Location = new System.Drawing.Point(12, 324);
            this.grpLayout.Name = "grpLayout";
            this.grpLayout.Size = new System.Drawing.Size(280, 185);
            this.grpLayout.TabIndex = 2;
            this.grpLayout.TabStop = false;
            this.grpLayout.Text = "3. Layout Configuration";
            //
            // lblCurrentLayout
            //
            this.lblCurrentLayout.AutoSize = true;
            this.lblCurrentLayout.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblCurrentLayout.ForeColor = System.Drawing.Color.Green;
            this.lblCurrentLayout.Location = new System.Drawing.Point(15, 160);
            this.lblCurrentLayout.Name = "lblCurrentLayout";
            this.lblCurrentLayout.Size = new System.Drawing.Size(179, 13);
            this.lblCurrentLayout.TabIndex = 3;
            this.lblCurrentLayout.Text = "Current Layout: Auto (2-column)";
            //
            // btnSetLayoutHtml
            //
            this.btnSetLayoutHtml.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSetLayoutHtml.Location = new System.Drawing.Point(15, 110);
            this.btnSetLayoutHtml.Name = "btnSetLayoutHtml";
            this.btnSetLayoutHtml.Size = new System.Drawing.Size(250, 40);
            this.btnSetLayoutHtml.TabIndex = 2;
            this.btnSetLayoutHtml.Text = "HTML Table Layout\r\n(Custom field positioning)";
            this.btnSetLayoutHtml.UseVisualStyleBackColor = true;
            this.btnSetLayoutHtml.Click += new System.EventHandler(this.btnSetLayoutHtml_Click);
            //
            // btnSetLayoutColumns
            //
            this.btnSetLayoutColumns.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSetLayoutColumns.Location = new System.Drawing.Point(15, 68);
            this.btnSetLayoutColumns.Name = "btnSetLayoutColumns";
            this.btnSetLayoutColumns.Size = new System.Drawing.Size(250, 35);
            this.btnSetLayoutColumns.TabIndex = 1;
            this.btnSetLayoutColumns.Text = "Column Count Layout (1-4 columns)";
            this.btnSetLayoutColumns.UseVisualStyleBackColor = true;
            this.btnSetLayoutColumns.Click += new System.EventHandler(this.btnSetLayoutColumns_Click);
            //
            // btnSetLayoutAuto
            //
            this.btnSetLayoutAuto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSetLayoutAuto.Location = new System.Drawing.Point(15, 26);
            this.btnSetLayoutAuto.Name = "btnSetLayoutAuto";
            this.btnSetLayoutAuto.Size = new System.Drawing.Size(250, 35);
            this.btnSetLayoutAuto.TabIndex = 0;
            this.btnSetLayoutAuto.Text = "Auto Layout (Default 2-column)";
            this.btnSetLayoutAuto.UseVisualStyleBackColor = true;
            this.btnSetLayoutAuto.Click += new System.EventHandler(this.btnSetLayoutAuto_Click);
            //
            // grpDialogs
            //
            this.grpDialogs.Controls.Add(this.cmbTheme);
            this.grpDialogs.Controls.Add(this.lblTheme);
            this.grpDialogs.Controls.Add(this.btnShowDelete);
            this.grpDialogs.Controls.Add(this.btnShowUpdate);
            this.grpDialogs.Controls.Add(this.btnShowInsert);
            this.grpDialogs.Enabled = false;
            this.grpDialogs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpDialogs.Location = new System.Drawing.Point(12, 515);
            this.grpDialogs.Name = "grpDialogs";
            this.grpDialogs.Size = new System.Drawing.Size(280, 220);
            this.grpDialogs.TabIndex = 3;
            this.grpDialogs.TabStop = false;
            this.grpDialogs.Text = "4. Test Dialogs";
            //
            // cmbTheme
            //
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(70, 25);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(195, 23);
            this.cmbTheme.TabIndex = 4;
            //
            // lblTheme
            //
            this.lblTheme.AutoSize = true;
            this.lblTheme.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTheme.Location = new System.Drawing.Point(15, 28);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(48, 15);
            this.lblTheme.TabIndex = 3;
            this.lblTheme.Text = "Theme:";
            //
            // btnShowDelete
            //
            this.btnShowDelete.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnShowDelete.Location = new System.Drawing.Point(15, 165);
            this.btnShowDelete.Name = "btnShowDelete";
            this.btnShowDelete.Size = new System.Drawing.Size(250, 40);
            this.btnShowDelete.TabIndex = 2;
            this.btnShowDelete.Text = "Show DELETE Dialog\r\n(Read-only confirmation)";
            this.btnShowDelete.UseVisualStyleBackColor = true;
            this.btnShowDelete.Click += new System.EventHandler(this.btnShowDelete_Click);
            //
            // btnShowUpdate
            //
            this.btnShowUpdate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnShowUpdate.Location = new System.Drawing.Point(15, 115);
            this.btnShowUpdate.Name = "btnShowUpdate";
            this.btnShowUpdate.Size = new System.Drawing.Size(250, 40);
            this.btnShowUpdate.TabIndex = 1;
            this.btnShowUpdate.Text = "Show UPDATE Dialog\r\n(With sample data)";
            this.btnShowUpdate.UseVisualStyleBackColor = true;
            this.btnShowUpdate.Click += new System.EventHandler(this.btnShowUpdate_Click);
            //
            // btnShowInsert
            //
            this.btnShowInsert.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnShowInsert.Location = new System.Drawing.Point(15, 60);
            this.btnShowInsert.Name = "btnShowInsert";
            this.btnShowInsert.Size = new System.Drawing.Size(250, 45);
            this.btnShowInsert.TabIndex = 0;
            this.btnShowInsert.Text = "Show INSERT Dialog\r\n(Create new record)";
            this.btnShowInsert.UseVisualStyleBackColor = true;
            this.btnShowInsert.Click += new System.EventHandler(this.btnShowInsert_Click);
            //
            // grpTestMethods
            //
            this.grpTestMethods.Controls.Add(this.btnClearConfig);
            this.grpTestMethods.Controls.Add(this.btnTestGetters);
            this.grpTestMethods.Enabled = false;
            this.grpTestMethods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpTestMethods.Location = new System.Drawing.Point(12, 741);
            this.grpTestMethods.Name = "grpTestMethods";
            this.grpTestMethods.Size = new System.Drawing.Size(280, 120);
            this.grpTestMethods.TabIndex = 4;
            this.grpTestMethods.TabStop = false;
            this.grpTestMethods.Text = "5. Test Methods";
            //
            // btnClearConfig
            //
            this.btnClearConfig.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClearConfig.Location = new System.Drawing.Point(15, 70);
            this.btnClearConfig.Name = "btnClearConfig";
            this.btnClearConfig.Size = new System.Drawing.Size(250, 35);
            this.btnClearConfig.TabIndex = 1;
            this.btnClearConfig.Text = "Clear All Configuration";
            this.btnClearConfig.UseVisualStyleBackColor = true;
            this.btnClearConfig.Click += new System.EventHandler(this.btnClearConfig_Click);
            //
            // btnTestGetters
            //
            this.btnTestGetters.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnTestGetters.Location = new System.Drawing.Point(15, 28);
            this.btnTestGetters.Name = "btnTestGetters";
            this.btnTestGetters.Size = new System.Drawing.Size(250, 35);
            this.btnTestGetters.TabIndex = 0;
            this.btnTestGetters.Text = "Test Getter Methods";
            this.btnTestGetters.UseVisualStyleBackColor = true;
            this.btnTestGetters.Click += new System.EventHandler(this.btnTestGetters_Click);
            //
            // grpResults
            //
            this.grpResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResults.Controls.Add(this.txtResults);
            this.grpResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpResults.Location = new System.Drawing.Point(298, 12);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(774, 849);
            this.grpResults.TabIndex = 5;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Test Results and Output";
            //
            // txtResults
            //
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResults.Location = new System.Drawing.Point(15, 25);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(745, 810);
            this.txtResults.TabIndex = 0;
            this.txtResults.WordWrap = false;
            //
            // FormUIGeneratorTest
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 868);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpTestMethods);
            this.Controls.Add(this.grpDialogs);
            this.Controls.Add(this.grpLayout);
            this.Controls.Add(this.grpConfiguration);
            this.Controls.Add(this.grpInitialization);
            this.MinimumSize = new System.Drawing.Size(1100, 750);
            this.Name = "FormUIGeneratorTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UIGenerator Comprehensive Test Suite";
            this.Load += new System.EventHandler(this.FormUIGeneratorTest_Load);
            this.grpInitialization.ResumeLayout(false);
            this.grpInitialization.PerformLayout();
            this.grpConfiguration.ResumeLayout(false);
            this.grpLayout.ResumeLayout(false);
            this.grpLayout.PerformLayout();
            this.grpDialogs.ResumeLayout(false);
            this.grpDialogs.PerformLayout();
            this.grpTestMethods.ResumeLayout(false);
            this.grpResults.ResumeLayout(false);
            this.grpResults.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpInitialization;
        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.RadioButton rdoDictionary;
        private System.Windows.Forms.RadioButton rdoDataTable;
        private System.Windows.Forms.RadioButton rdoSqlConnection;

        private System.Windows.Forms.GroupBox grpConfiguration;
        private System.Windows.Forms.Button btnConfigureAliases;
        private System.Windows.Forms.Button btnConfigureExclusions;
        private System.Windows.Forms.Button btnConfigureForeignKeys;

        private System.Windows.Forms.GroupBox grpLayout;
        private System.Windows.Forms.Button btnSetLayoutAuto;
        private System.Windows.Forms.Button btnSetLayoutColumns;
        private System.Windows.Forms.Button btnSetLayoutHtml;
        private System.Windows.Forms.Label lblCurrentLayout;

        private System.Windows.Forms.GroupBox grpDialogs;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Button btnShowInsert;
        private System.Windows.Forms.Button btnShowUpdate;
        private System.Windows.Forms.Button btnShowDelete;

        private System.Windows.Forms.GroupBox grpTestMethods;
        private System.Windows.Forms.Button btnTestGetters;
        private System.Windows.Forms.Button btnClearConfig;

        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.TextBox txtResults;

        private void FormUIGeneratorTest_Load(object sender, System.EventArgs e)
        {
            // Populate theme combo box
            cmbTheme.Items.Clear();
            foreach (ZidThemes theme in System.Enum.GetValues(typeof(ZidThemes)))
            {
                cmbTheme.Items.Add(theme);
            }
            cmbTheme.SelectedItem = ZidThemes.Default;

            // Initialize current layout label
            lblCurrentLayout.Text = "Current Layout: Auto (2-column)";
            lblCurrentLayout.ForeColor = System.Drawing.Color.Green;

            AppendResult("===================================================");
            AppendResult("   UIGenerator Comprehensive Test Suite");
            AppendResult("===================================================");
            AppendResult("");
            AppendResult("STEP 1: Initialize UIGenerator");
            AppendResult("  • SqlConnection: ArticleProposal table (auto-detects FK/PK)");
            AppendResult("  • DataTable: Users table (test data included)");
            AppendResult("  • Dictionary: Products (simple field definitions)");
            AppendResult("");
            AppendResult("STEP 2: Configure (All modes supported)");
            AppendResult("  • Aliases: User-friendly field names");
            AppendResult("  • Exclusions: Fields with default values");
            AppendResult("  • Foreign Keys: Lookup dropdowns");
            AppendResult("");
            AppendResult("STEP 3: Set Layout");
            AppendResult("  • Auto: Default 2-column label/control");
            AppendResult("  • Column Count: 1-4 column grid");
            AppendResult("  • HTML Table: Custom field positioning with colspan");
            AppendResult("");
            AppendResult("STEP 4: Test Dialogs");
            AppendResult("  • INSERT: Create new records");
            AppendResult("  • UPDATE: Modify existing records");
            AppendResult("  • DELETE: Confirmation with read-only fields");
            AppendResult("  • Theme: Select visual theme for dialogs");
            AppendResult("");
            AppendResult("STEP 5: Test Methods");
            AppendResult("  • Getters: Retrieve all configuration");
            AppendResult("  • Clear: Reset all configuration");
            AppendResult("");
            AppendResult("===================================================");
            AppendResult("");
            AppendResult("Select an initialization method and click 'Initialize'");
            AppendResult("");
        }
    }
}
