using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win;
using ZidUtilities.CommonCode.Win.CRUD;

namespace ZidUtilities.TesterWin
{
    public partial class FormUIGeneratorTest : Form
    {
        private UIGenerator _generator;
        private DataTable _testDataTable;
        private Dictionary<string, object> _testData;

        public FormUIGeneratorTest()
        {
            InitializeComponent();
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            // Create test DataTable for DataTable initialization
            _testDataTable = new DataTable("Users");
            _testDataTable.Columns.Add("UserId", typeof(int));
            _testDataTable.Columns.Add("Username", typeof(string));
            _testDataTable.Columns.Add("Email", typeof(string));
            _testDataTable.Columns.Add("Password", typeof(string));
            _testDataTable.Columns.Add("BirthDate", typeof(DateTime));
            _testDataTable.Columns.Add("IsActive", typeof(bool));
            _testDataTable.Columns.Add("Salary", typeof(decimal));
            _testDataTable.Columns.Add("RoleId", typeof(int));
            _testDataTable.Columns.Add("Notes", typeof(string));

            _testDataTable.Columns["UserId"].AutoIncrement = true;
            _testDataTable.PrimaryKey = new DataColumn[] { _testDataTable.Columns["UserId"] };

            _testDataTable.Rows.Add(1, "admin", "admin@test.com", "password123", new DateTime(1990, 1, 15), true, 75000m, 1, "Administrator");
            _testDataTable.Rows.Add(2, "user1", "user1@test.com", "pass456", new DateTime(1995, 6, 20), true, 50000m, 2, "Regular user");

            // Create test data for Dictionary initialization
            _testData = new Dictionary<string, object>
            {
                { "ProductId", 1 },
                { "ProductName", "Test Product" },
                { "Price", 99.99m },
                { "StockQuantity", 100 },
                { "CategoryId", 5 },
                { "Description", "Product description" },
                { "IsAvailable", true },
                { "CreatedDate", DateTime.Now }
            };
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                ClearResults();
                AppendResult("=== INITIALIZATION TEST ===");

                if (rdoSqlConnection.Checked)
                {
                    AppendResult("Connecting to SQL Server...");
                    using (var conn = new SqlConnection("Data Source=Main\\SQLSERVER;Initial Catalog=FaradayWiki;Integrated Security=False;User ID=SqlAdmin;Password=SqlAdmin123"))
                    {
                        _generator = new UIGenerator(conn, "ArticleProposal");
                    }
                    AppendResult("✓ Initialized from SQL Connection");
                    AppendResult($"  Table: ArticleProposal");

                    var fields = _generator.GetFieldNames();
                    AppendResult($"  Fields: {string.Join(", ", fields)}");

                    var pkFields = _generator.GetPrimaryKeyFields();
                    AppendResult($"  Primary Keys (auto-detected): {string.Join(", ", pkFields)}");

                    var fkFields = _generator.GetForeignKeyFields();
                    AppendResult($"  Foreign Keys (auto-detected): {fkFields.Count}");
                    foreach (var fk in fkFields)
                    {
                        var fkInfo = _generator.GetForeignKeyInfo(fk);
                        AppendResult($"    • {fk} → {fkInfo.ReferencedTable}.{fkInfo.ReferencedColumn}");
                    }
                }
                else if (rdoDataTable.Checked)
                {
                    _generator = new UIGenerator(_testDataTable);
                    AppendResult("✓ Initialized from DataTable");
                    AppendResult($"  Table: {_testDataTable.TableName}");
                    AppendResult($"  Fields: {_generator.GetFieldNames().Count}");
                    AppendResult($"  Primary Keys: {string.Join(", ", _generator.GetPrimaryKeyFields())}");
                }
                else if (rdoDictionary.Checked)
                {
                    _generator = new UIGenerator(_testData, "Products");
                    AppendResult("✓ Initialized from Dictionary");
                    AppendResult($"  Table: Products");
                    AppendResult($"  Fields: {_generator.GetFieldNames().Count}");
                }

                EnableConfigurationPanel(true);
                AppendResult("\n✓ Initialization successful!");
                AppendResult("→ Configure aliases, masks, exclusions, foreign keys, and layout below");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error:\n{ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppendResult($"✗ ERROR: {ex.Message}");
            }
        }

        private void btnConfigureAliases_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== CONFIGURING ALIASES ===");

                if (rdoSqlConnection.Checked)
                {
                    // ArticleProposal aliases
                    _generator.SetAlias("Id", "Article ID");
                    _generator.SetAlias("Name", "Article Name");
                    _generator.SetAlias("Author", "Author Name");
                    _generator.SetAlias("Created", "Creation Date");
                    _generator.SetAlias("Data", "Article Content");
                    _generator.SetAlias("Status", "Current Status");
                    _generator.SetAlias("Approver", "Approved By");
                    _generator.SetAlias("PublishedId", "Published Article ID");
                    _generator.SetAlias("Published", "Publication Date");
                    _generator.SetAlias("ArticleToUpdate", "Updates Article");
                    _generator.SetAlias("Category", "Article Category");
                    _generator.SetAlias("Department", "Department");
                    AppendResult("✓ Set 12 aliases for ArticleProposal fields");
                }
                else if (rdoDataTable.Checked)
                {
                    _generator.SetAlias("UserId", "User ID");
                    _generator.SetAlias("Username", "User Name");
                    _generator.SetAlias("BirthDate", "Date of Birth");
                    _generator.SetAlias("IsActive", "Active Status");
                    _generator.SetAlias("RoleId", "User Role");
                    AppendResult("✓ Set 5 aliases for Users fields");
                }
                else
                {
                    _generator.SetAlias("ProductId", "Product ID");
                    _generator.SetAlias("ProductName", "Product Name");
                    _generator.SetAlias("StockQuantity", "In Stock");
                    _generator.SetAlias("CategoryId", "Category");
                    _generator.SetAlias("CreatedDate", "Created On");
                    AppendResult("✓ Set 5 aliases for Products fields");
                }

                var aliases = _generator.GetAllAliases();
                foreach (var alias in aliases)
                    AppendResult($"  {alias.Key} → {alias.Value}");
            }
            catch (Exception ex)
            {
                ShowError("configuring aliases", ex);
            }
        }

        private void btnConfigureExclusions_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== CONFIGURING EXCLUSIONS ===");

                if (rdoSqlConnection.Checked)
                {
                    // ArticleProposal - exclude Created (has default), Status (has default)
                    _generator.SetExclusion("Created", DateTime.Now);
                    _generator.SetExclusion("Status", "Pending");
                    AppendResult("✓ Excluded 'Created' with default: Current DateTime");
                    AppendResult("✓ Excluded 'Status' with default: 'Pending'");
                }
                else if (rdoDataTable.Checked)
                {
                    _generator.SetExclusion("Notes");
                    AppendResult("✓ Excluded 'Notes' field");
                }
                else
                {
                    _generator.SetExclusion("CreatedDate", DateTime.Now);
                    AppendResult("✓ Excluded 'CreatedDate' with default: Current DateTime");
                }

                var excluded = _generator.GetExcludedFields();
                AppendResult($"  Total excluded fields: {excluded.Count}");
            }
            catch (Exception ex)
            {
                ShowError("configuring exclusions", ex);
            }
        }

        private void btnConfigureForeignKeys_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== CONFIGURING FOREIGN KEY LOOKUPS ===");

                if (rdoSqlConnection.Checked)
                {
                    // For ArticleProposal, foreign keys are auto-detected
                    // We just need to populate lookup values
                    var authors = new Dictionary<string, object>
                    {
                        { "John Doe", "john.doe" },
                        { "Jane Smith", "jane.smith" },
                        { "Bob Johnson", "bob.johnson" }
                    };
                    _generator.SetForeignKey("Author", authors);
                    _generator.SetForeignKey("Approver", authors);
                    AppendResult("✓ Set Author lookup values (3 users)");
                    AppendResult("✓ Set Approver lookup values (3 users)");

                    var categories = new Dictionary<string, object>
                    {
                        { "Technology", 1 },
                        { "Business", 2 },
                        { "Science", 3 },
                        { "Health", 4 }
                    };
                    _generator.SetForeignKey("Category", categories);
                    AppendResult("✓ Set Category lookup values (4 categories)");
                }
                else if (rdoDataTable.Checked)
                {
                    var roles = new Dictionary<string, object>
                    {
                        { "Administrator", 1 },
                        { "User", 2 },
                        { "Guest", 3 },
                        { "Moderator", 4 }
                    };
                    _generator.SetForeignKey("RoleId", roles);
                    AppendResult("✓ Set RoleId lookup values (4 roles)");
                }
                else
                {
                    var categories = new Dictionary<string, object>
                    {
                        { "Electronics", 1 },
                        { "Clothing", 2 },
                        { "Books", 3 },
                        { "Food", 4 },
                        { "Toys", 5 }
                    };
                    _generator.SetForeignKey("CategoryId", categories);
                    AppendResult("✓ Set CategoryId lookup values (5 categories)");
                }

                var fkFields = _generator.GetForeignKeyFields();
                AppendResult($"  Total foreign keys configured: {fkFields.Count}");
            }
            catch (Exception ex)
            {
                ShowError("configuring foreign keys", ex);
            }
        }

        private void btnSetLayoutAuto_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                _generator.ClearLayout();
                AppendResult("\n=== LAYOUT: AUTO (2-column) ===");
                AppendResult("✓ Layout set to Auto (default 2-column)");
                lblCurrentLayout.Text = "Current Layout: Auto (2-column)";
                lblCurrentLayout.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                ShowError("setting auto layout", ex);
            }
        }

        private void btnSetLayoutColumns_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter number of columns (1-4):",
                    "Column Count Layout",
                    "3");

                if (string.IsNullOrEmpty(input)) return;

                if (int.TryParse(input, out int cols) && cols >= 1 && cols <= 4)
                {
                    _generator.SetLayoutByColumnCount(cols);
                    AppendResult($"\n=== LAYOUT: {cols}-COLUMN GRID ===");
                    AppendResult($"✓ Layout set to {cols} columns");
                    lblCurrentLayout.Text = $"Current Layout: {cols}-Column Grid";
                    lblCurrentLayout.ForeColor = Color.Blue;
                }
                else
                {
                    MessageBox.Show("Column count must be between 1 and 4", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowError("setting column layout", ex);
            }
        }

        private void btnSetLayoutHtml_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                string htmlLayout;

                if (rdoSqlConnection.Checked)
                {
                    // ArticleProposal layout
                    htmlLayout = @"<table><tbody>
  <tr><td colspan='4'>Name</td></tr>
  <tr><td colspan='2'>Author</td><td>Created</td><td>Status</td></tr>
  <tr><td colspan='4'>Data</td></tr>
  <tr><td>Published</td><td>ArticleToUpdate</td><td>Category</td><td>Department</td></tr>
</tbody></table>";
                }
                else if (rdoDataTable.Checked)
                {
                    htmlLayout = @"<table><tbody>
  <tr><td colspan='2'>Username</td></tr>
  <tr><td>Email</td><td>Password</td></tr>
  <tr><td>BirthDate</td><td>Salary</td></tr>
  <tr><td>RoleId</td><td>IsActive</td></tr>
</tbody></table>";
                }
                else
                {
                    htmlLayout = @"<table><tbody>
  <tr><td colspan='2'>ProductName</td></tr>
  <tr><td>Price</td><td>StockQuantity</td></tr>
  <tr><td colspan='2'>Description</td></tr>
  <tr><td>CategoryId</td><td>IsAvailable</td></tr>
</tbody></table>";
                }

                _generator.SetLayoutByHtmlTable(htmlLayout);
                AppendResult("\n=== LAYOUT: HTML TABLE ===");
                AppendResult("✓ Layout set from HTML table definition");
                AppendResult("HTML Layout:");
                AppendResult(htmlLayout);
                lblCurrentLayout.Text = "Current Layout: HTML Table";
                lblCurrentLayout.ForeColor = Color.Purple;
            }
            catch (Exception ex)
            {
                ShowError("setting HTML layout", ex);
            }
        }

        private void btnShowInsert_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== INSERT DIALOG ===");
                _generator.Theme = (ZidThemes)cmbTheme.SelectedItem;
                AppendResult($"Theme: {_generator.Theme}");

                var result = _generator.ShowInsertDialog();

                if (result != null)
                {
                    AppendResult("✓ Insert completed successfully");
                    DisplayResult(result);
                }
                else
                {
                    AppendResult("✗ Insert cancelled");
                }
            }
            catch (Exception ex)
            {
                ShowError("showing insert dialog", ex);
            }
        }

        private void btnShowUpdate_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== UPDATE DIALOG ===");
                _generator.Theme = (ZidThemes)cmbTheme.SelectedItem;
                AppendResult($"Theme: {_generator.Theme}");

                CRUDResult result;
                if (rdoDictionary.Checked)
                {
                    result = _generator.ShowUpdateDialog(new Dictionary<string, object>(_testData));
                }
                else if (rdoDataTable.Checked)
                {
                    result = _generator.ShowUpdateDialog(_testDataTable.Rows[0]);
                }
                else
                {
                    // SQL Connection - create sample data
                    var sampleData = new Dictionary<string, object>
                    {
                        { "Id", 1 },
                        { "Name", "Sample Article" },
                        { "Author", "john.doe" },
                        { "Created", DateTime.Now.AddDays(-5) },
                        { "Data", "This is sample article content for testing." },
                        { "Status", "Pending" },
                        { "Approver", DBNull.Value },
                        { "PublishedId", DBNull.Value },
                        { "Published", DBNull.Value },
                        { "ArticleToUpdate", DBNull.Value },
                        { "Category", 1 },
                        { "Department", DBNull.Value }
                    };
                    result = _generator.ShowUpdateDialog(sampleData);
                }

                if (result != null)
                {
                    AppendResult("✓ Update completed successfully");
                    DisplayResult(result);
                }
                else
                {
                    AppendResult("✗ Update cancelled");
                }
            }
            catch (Exception ex)
            {
                ShowError("showing update dialog", ex);
            }
        }

        private void btnShowDelete_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== DELETE DIALOG ===");
                _generator.Theme = (ZidThemes)cmbTheme.SelectedItem;
                AppendResult($"Theme: {_generator.Theme}");

                CRUDResult result;
                if (rdoDictionary.Checked)
                {
                    result = _generator.ShowDeleteDialog(new Dictionary<string, object>(_testData));
                }
                else if (rdoDataTable.Checked)
                {
                    result = _generator.ShowDeleteDialog(_testDataTable.Rows[0]);
                }
                else
                {
                    var sampleData = new Dictionary<string, object>
                    {
                        { "Id", 1 },
                        { "Name", "Article to Delete" },
                        { "Author", "john.doe" },
                        { "Created", DateTime.Now.AddDays(-10) },
                        { "Data", "This article will be deleted." },
                        { "Status", "Draft" },
                        { "Approver", "jane.smith" },
                        { "PublishedId", DBNull.Value },
                        { "Published", DBNull.Value },
                        { "ArticleToUpdate", DBNull.Value },
                        { "Category", 2 },
                        { "Department", 5 }
                    };
                    result = _generator.ShowDeleteDialog(sampleData);
                }

                if (result != null)
                {
                    AppendResult("✓ Delete confirmed");
                    DisplayResult(result);
                }
                else
                {
                    AppendResult("✗ Delete cancelled");
                }
            }
            catch (Exception ex)
            {
                ShowError("showing delete dialog", ex);
            }
        }

        private void btnTestGetters_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== GETTER METHODS TEST ===");

                var fields = _generator.GetFieldNames();
                AppendResult($"GetFieldNames(): {fields.Count} fields");
                AppendResult($"  {string.Join(", ", fields)}");

                var pkFields = _generator.GetPrimaryKeyFields();
                AppendResult($"GetPrimaryKeyFields(): {string.Join(", ", pkFields)}");

                var fkFields = _generator.GetForeignKeyFields();
                AppendResult($"GetForeignKeyFields(): {fkFields.Count} keys");
                foreach (var fk in fkFields)
                {
                    var fkInfo = _generator.GetForeignKeyInfo(fk);
                    if (fkInfo != null && !string.IsNullOrEmpty(fkInfo.ReferencedTable))
                        AppendResult($"  {fk} → {fkInfo.ReferencedTable}.{fkInfo.ReferencedColumn}");
                }

                var aliases = _generator.GetAllAliases();
                AppendResult($"GetAllAliases(): {aliases.Count} aliases");
                foreach (var alias in aliases)
                    AppendResult($"  {alias.Key} = '{alias.Value}'");

                var excluded = _generator.GetExcludedFields();
                AppendResult($"GetExcludedFields(): {string.Join(", ", excluded)}");

                AppendResult("✓ All getter methods tested successfully");
            }
            catch (Exception ex)
            {
                ShowError("testing getters", ex);
            }
        }

        private void btnClearConfig_Click(object sender, EventArgs e)
        {
            if (_generator == null) { ShowNotInitializedWarning(); return; }

            try
            {
                AppendResult("\n=== CLEARING CONFIGURATION ===");

                _generator.ClearAllAliases();
                AppendResult("✓ Cleared all aliases");

                _generator.ClearAllMasks();
                AppendResult("✓ Cleared all masks");

                _generator.ClearAllExclusions();
                AppendResult("✓ Cleared all exclusions");

                _generator.ClearLayout();
                AppendResult("✓ Reset layout to Auto");
                lblCurrentLayout.Text = "Current Layout: Auto (2-column)";
                lblCurrentLayout.ForeColor = Color.Green;

                AppendResult("\n✓ All configuration cleared");
            }
            catch (Exception ex)
            {
                ShowError("clearing configuration", ex);
            }
        }

        private void DisplayResult(CRUDResult result)
        {
            AppendResult($"  Table: {result.TableName}");
            AppendResult($"  Fields: {result.GetFieldNames().Count}");

            AppendResult("\n--- Field Values ---");
            foreach (var fieldName in result.GetFieldNames())
            {
                var value = result[fieldName];
                var displayValue = value == DBNull.Value ? "(NULL)" : value?.ToString();
                AppendResult($"  {fieldName} = {displayValue}");
            }

            AppendResult("\n--- SQL Server Script ---");
            AppendResult(result.SqlServerScript);

            AppendResult("\n--- SQLite Script ---");
            AppendResult(result.SqliteScript);

            AppendResult("\n--- MS Access Script ---");
            AppendResult(result.AccessScript);
        }

        private void EnableConfigurationPanel(bool enabled)
        {
            grpConfiguration.Enabled = enabled;
            grpLayout.Enabled = enabled;
            grpDialogs.Enabled = enabled;
            grpTestMethods.Enabled = enabled;
        }

        private void ShowNotInitializedWarning()
        {
            MessageBox.Show("Please initialize the UIGenerator first.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(string action, Exception ex)
        {
            MessageBox.Show($"Error {action}:\n{ex.Message}\n\n{ex.StackTrace}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            AppendResult($"✗ ERROR: {ex.Message}");
        }

        private void AppendResult(string text)
        {
            txtResults.AppendText(text + Environment.NewLine);
            txtResults.SelectionStart = txtResults.Text.Length;
            txtResults.ScrollToCaret();
        }

        private void ClearResults()
        {
            txtResults.Clear();
        }
    }
}
