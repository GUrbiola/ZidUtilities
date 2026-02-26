using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.CRUD;
using ZidUtilities.CommonCode.Win.Controls;
using ZidUtilities.TesterWin.DataAccess;
using ZidUtilities.CommonCode.Win;

namespace ZidUtilities.TesterWin
{
    public partial class FormVeinsTest : Form
    {
        private SqliteConnector connector;
        private UIGenerator uiGenerator;
        private DataTable veinsTable;
        private Dictionary<string, object> collectionLookup;

        public FormVeinsTest()
        {
            InitializeComponent();
        }

        private void FormVeinsTest_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeDatabase();
                LoadCollectionLookup();
                ConfigureUIGenerator();
                LoadVeinsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}\n\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDatabase()
        {
            // Build connection string to the database file in the TesterWin directory
            string dbPath = Path.Combine(Application.StartupPath, "IconCommanderDb.db");

            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException($"Database file not found at: {dbPath}");
            }

            string connectionString = $"Data Source={dbPath};Version=3;";
            connector = new SqliteConnector(connectionString);

            // Test the connection
            if (!connector.TestConnection())
            {
                throw new Exception($"Failed to connect to database: {connector.LastMessage}");
            }
        }

        private void LoadCollectionLookup()
        {
            // Load Collections table to populate the foreign key lookup
            var response = connector.ExecuteTable("SELECT Id, Name FROM Collections ORDER BY Name");

            if (response.IsFailure || response.Result == null)
            {
                throw new Exception($"Failed to load Collections: {connector.LastMessage}");
            }

            collectionLookup = new Dictionary<string, object>();

            foreach (DataRow row in response.Result.Rows)
            {
                string name = row["Name"].ToString();
                object id = row["Id"];
                collectionLookup[name] = id;
            }
        }

        private void ConfigureUIGenerator()
        {
            // Create a sample DataTable to get the schema
            var schemaResponse = connector.ExecuteTable("SELECT * FROM Veins LIMIT 0");

            if (schemaResponse.IsFailure || schemaResponse.Result == null)
            {
                throw new Exception($"Failed to get Veins schema: {connector.LastMessage}");
            }

            DataTable schema = schemaResponse.Result;

            // Initialize UIGenerator with the schema
            uiGenerator = new UIGenerator(schema, "Veins");

            // Configure the UIGenerator
            // Set primary key
            uiGenerator.SetPrimaryKey("Id");

            // Set Id as auto-increment and excluded from insert
            uiGenerator.SetExclusion("Id");

            // Set foreign key for Collection field
            uiGenerator.SetForeignKey("Collection", collectionLookup);

            // Set Aliases
            uiGenerator.SetAlias("Collection", "Collection");
            uiGenerator.SetAlias("Name", "Vein Name");
            uiGenerator.SetAlias("Description", "Description");
            uiGenerator.SetAlias("Path", "Vein Path");
            uiGenerator.SetAlias("IsIcon", "Contains Icons");
            uiGenerator.SetAlias("IsImage", "Contains Images");
            uiGenerator.SetAlias("IsSvg", "Contains SVG");

            // Set Required Fields
            uiGenerator.SetRequired("Collection");
            uiGenerator.SetRequired("Name");
            uiGenerator.SetRequired("Path");
            uiGenerator.SetRequired("IsIcon");
            uiGenerator.SetRequired("IsImage");
            uiGenerator.SetRequired("IsSvg");

            // Set Field Formatting
            uiGenerator.SetFieldFormat("Path", FieldFormat.Folder);
            uiGenerator.SetFieldFormat("IsIcon", FieldFormat.Check);
            uiGenerator.SetFieldFormat("IsImage", FieldFormat.Check);
            uiGenerator.SetFieldFormat("IsSvg", FieldFormat.Check);

            // Set theme for generated dialogs
            uiGenerator.Theme = ZidThemes.BlackAndWhite;
            uiGenerator.FormWidth = 500;
        }

        private void LoadVeinsData()
        {
            try
            {
                // Load all records from Veins table with Collection name joined
                string sql = @"
                    SELECT
                        v.Id,
                        v.Collection,
                        c.Name AS CollectionName,
                        v.Name,
                        v.Description,
                        v.Path,
                        v.IsIcon,
                        v.IsImage,
                        v.IsSvg
                    FROM Veins v
                    LEFT JOIN Collections c ON v.Collection = c.Id
                    ORDER BY v.Name";

                var response = connector.ExecuteTable(sql);

                if (response.IsFailure || response.Result == null)
                {
                    MessageBox.Show($"Failed to load Veins data: {connector.LastMessage}",
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                veinsTable = response.Result;
                zidGrid1.DataSource = veinsTable;

                // Configure grid columns
                if (zidGrid1.GridControl.Columns.Count > 0)
                {
                    // Hide the Collection ID column (we show CollectionName instead)
                    zidGrid1.GridControl.Columns["Collection"].Visible = false;

                    // Set column headers
                    zidGrid1.GridControl.Columns["Id"].HeaderText = "ID";
                    zidGrid1.GridControl.Columns["CollectionName"].HeaderText = "Collection";
                    zidGrid1.GridControl.Columns["Name"].HeaderText = "Vein Name";
                    zidGrid1.GridControl.Columns["Description"].HeaderText = "Description";
                    zidGrid1.GridControl.Columns["Path"].HeaderText = "Path";
                    zidGrid1.GridControl.Columns["IsIcon"].HeaderText = "Has Icons";
                    zidGrid1.GridControl.Columns["IsImage"].HeaderText = "Has Images";
                    zidGrid1.GridControl.Columns["IsSvg"].HeaderText = "Has SVG";

                    // Set column widths
                    zidGrid1.GridControl.Columns["Id"].Width = 50;
                    zidGrid1.GridControl.Columns["CollectionName"].Width = 150;
                    zidGrid1.GridControl.Columns["Name"].Width = 200;
                    zidGrid1.GridControl.Columns["Description"].Width = 200;
                    zidGrid1.GridControl.Columns["Path"].Width = 250;
                    zidGrid1.GridControl.Columns["IsIcon"].Width = 80;
                    zidGrid1.GridControl.Columns["IsImage"].Width = 80;
                    zidGrid1.GridControl.Columns["IsSvg"].Width = 80;

                    // Set column display order
                    zidGrid1.GridControl.Columns["Id"].DisplayIndex = 0;
                    zidGrid1.GridControl.Columns["CollectionName"].DisplayIndex = 1;
                    zidGrid1.GridControl.Columns["Name"].DisplayIndex = 2;
                    zidGrid1.GridControl.Columns["Description"].DisplayIndex = 3;
                    zidGrid1.GridControl.Columns["Path"].DisplayIndex = 4;
                    zidGrid1.GridControl.Columns["IsIcon"].DisplayIndex = 5;
                    zidGrid1.GridControl.Columns["IsImage"].DisplayIndex = 6;
                    zidGrid1.GridControl.Columns["IsSvg"].DisplayIndex = 7;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}",
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadVeinsData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Show the insert dialog
                var result = uiGenerator.ShowInsertDialog();

                if (result == null)
                {
                    // User cancelled
                    return;
                }

                // Execute the SQLite insert script
                var response = connector.ExecuteNonQuery(result.SqliteScript);

                if (response.IsFailure)
                {
                    MessageBox.Show($"Failed to insert record: {connector.LastMessage}",
                        "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Record added successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the grid
                LoadVeinsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding record: {ex.Message}\n\n{ex.StackTrace}",
                    "Add Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (zidGrid1.SelectedRow == null)
                {
                    MessageBox.Show("Please select a record to edit.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = zidGrid1.SelectedRow;

                // Get the ID of the record to edit
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                // Load the actual record from database
                // UIGenerator expects the foreign key field to contain the actual ID value
                // It will use the lookup dictionary to display the corresponding name in the dropdown
                string sql = $"SELECT * FROM Veins WHERE Id = {id}";

                var recordResponse = connector.ExecuteTable(sql);

                if (recordResponse.IsFailure || recordResponse.Result == null || recordResponse.Result.Rows.Count == 0)
                {
                    MessageBox.Show($"Failed to load record: {connector.LastMessage}",
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow record = recordResponse.Result.Rows[0];

                // Show the update dialog with current values
                var result = uiGenerator.ShowUpdateDialog(record);

                if (result == null)
                {
                    // User cancelled
                    return;
                }

                // Execute the SQLite update script
                var response = connector.ExecuteNonQuery(result.SqliteScript);

                if (response.IsFailure)
                {
                    MessageBox.Show($"Failed to update record: {connector.LastMessage}",
                        "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Record updated successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the grid
                LoadVeinsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating record: {ex.Message}\n\n{ex.StackTrace}",
                    "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (zidGrid1.SelectedRow == null)
                {
                    MessageBox.Show("Please select a record to delete.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = zidGrid1.SelectedRow;

                // Get the ID of the record to delete
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                // Load the actual record from database
                // UIGenerator expects the foreign key field to contain the actual ID value
                // It will use the lookup dictionary to display the corresponding name in the dialog
                string sql = $"SELECT * FROM Veins WHERE Id = {id}";

                var recordResponse = connector.ExecuteTable(sql);

                if (recordResponse.IsFailure || recordResponse.Result == null || recordResponse.Result.Rows.Count == 0)
                {
                    MessageBox.Show($"Failed to load record: {connector.LastMessage}",
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow record = recordResponse.Result.Rows[0];

                // Show the delete confirmation dialog
                var result = uiGenerator.ShowDeleteDialog(record);

                if (result == null)
                {
                    // User cancelled
                    return;
                }

                // Execute the SQLite delete script
                var response = connector.ExecuteNonQuery(result.SqliteScript);

                if (response.IsFailure)
                {
                    MessageBox.Show($"Failed to delete record: {connector.LastMessage}",
                        "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Record deleted successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the grid
                LoadVeinsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}\n\n{ex.StackTrace}",
                    "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
