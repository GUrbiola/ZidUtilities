using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Forms;

namespace ZidUtilities.TesterWin
{
    /// <summary>
    /// Test form for all generic dialog forms.
    /// </summary>
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        /// <summary>
        /// Loads the combo box values.
        /// </summary>
        private void LoadComboBoxes()
        {
            // Dialog styles
            foreach (DialogStyle style in Enum.GetValues(typeof(DialogStyle)))
            {
                cmbDialogStyle.Items.Add(style);
            }
            cmbDialogStyle.SelectedItem = DialogStyle.Default;

            // Text input formats
            foreach (TextInputFormat format in Enum.GetValues(typeof(TextInputFormat)))
            {
                cmbInputFormat.Items.Add(format);
            }
            cmbInputFormat.SelectedItem = TextInputFormat.None;
        }

        #region TextInputDialog Tests

        private void btnTextInput_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;
            TextInputFormat format = (TextInputFormat)cmbInputFormat.SelectedItem;

            string result = TextInputDialog.ShowDialog(
                "Text Input Test",
                "Please enter some text:",
                style,
                format,
                chkRequired.Checked,
                "",
                null,
                this
            );

            if (result != null)
            {
                txtResults.AppendText($"Text Input: {result}\r\n");
            }
            else
            {
                txtResults.AppendText("Text Input: Cancelled\r\n");
            }
        }

        private void btnEmailInput_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            string result = TextInputDialog.ShowDialog(
                "Email Input",
                "Please enter your email address:",
                style,
                TextInputFormat.Email,
                true,
                "",
                null,
                this
            );

            if (result != null)
            {
                txtResults.AppendText($"Email: {result}\r\n");
            }
            else
            {
                txtResults.AppendText("Email: Cancelled\r\n");
            }
        }

        private void btnPhoneInput_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            string result = TextInputDialog.ShowDialog(
                "Phone Number Input",
                "Please enter your phone number:",
                style,
                TextInputFormat.PhoneNumber,
                true,
                "",
                null,
                this
            );

            if (result != null)
            {
                txtResults.AppendText($"Phone: {result}\r\n");
            }
            else
            {
                txtResults.AppendText("Phone: Cancelled\r\n");
            }
        }

        #endregion

        #region SingleSelectionDialog Tests

        private void btnSingleSelection_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            List<string> fruits = new List<string>
            {
                "Apple", "Banana", "Cherry", "Date", "Elderberry",
                "Fig", "Grape", "Honeydew", "Kiwi", "Lemon",
                "Mango", "Nectarine", "Orange", "Papaya", "Quince"
            };

            object result = SingleSelectionDialog.ShowDialog(
                "Select a Fruit",
                "Please select your favorite fruit:",
                fruits,
                style,
                chkRequired.Checked,
                null,
                this
            );

            if (result != null)
            {
                txtResults.AppendText($"Single Selection: {result}\r\n");
            }
            else
            {
                txtResults.AppendText("Single Selection: Cancelled\r\n");
            }
        }

        private void btnSingleSelectionDataTable_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            // Create sample data table
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ID", typeof(int));

            dt.Rows.Add("John Smith", 1);
            dt.Rows.Add("Jane Doe", 2);
            dt.Rows.Add("Bob Johnson", 3);
            dt.Rows.Add("Alice Williams", 4);
            dt.Rows.Add("Charlie Brown", 5);

            object result = SingleSelectionDialog.ShowDialog(
                "Select a Person",
                "Please select a person from the list:",
                dt,
                "Name",
                "ID",
                style,
                chkRequired.Checked,
                null,
                this
            );

            if (result != null)
            {
                txtResults.AppendText($"Single Selection (ID): {result}\r\n");
            }
            else
            {
                txtResults.AppendText("Single Selection: Cancelled\r\n");
            }
        }

        #endregion

        #region MultiSelectionDialog Tests

        private void btnMultiSelection_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            List<string> colors = new List<string>
            {
                "Red", "Blue", "Green", "Yellow", "Orange",
                "Purple", "Pink", "Brown", "Black", "White",
                "Gray", "Cyan", "Magenta", "Lime", "Navy"
            };

            List<object> results = MultiSelectionDialog.ShowDialog(
                "Select Colors",
                "Please select one or more colors:",
                colors,
                style,
                chkRequired.Checked,
                null,
                this
            );

            if (results != null)
            {
                txtResults.AppendText($"Multi Selection ({results.Count} items): {string.Join(", ", results)}\r\n");
            }
            else
            {
                txtResults.AppendText("Multi Selection: Cancelled\r\n");
            }
        }

        #endregion

        #region ComplexObjectSelectionDialog Tests

        private void btnComplexSelection_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            // Create sample data table
            DataTable dt = new DataTable();
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("EmployeeID", typeof(int));

            dt.Rows.Add("John", "Smith", 30, "New York", 101);
            dt.Rows.Add("Jane", "Doe", 25, "Los Angeles", 102);
            dt.Rows.Add("Bob", "Johnson", 35, "Chicago", 103);
            dt.Rows.Add("Alice", "Williams", 28, "Houston", 104);
            dt.Rows.Add("Charlie", "Brown", 42, "Phoenix", 105);
            dt.Rows.Add("Diana", "Davis", 31, "Philadelphia", 106);
            dt.Rows.Add("Edward", "Miller", 27, "San Antonio", 107);
            dt.Rows.Add("Fiona", "Wilson", 33, "San Diego", 108);

            List<string> displayColumns = new List<string> { "FirstName", "LastName", "Age", "City" };
            List<string> keyColumns = new List<string> { "EmployeeID" };

            List<object> results = ComplexObjectSelectionDialog.ShowDialog(
                "Select Employees",
                "Please select one or more employees from the grid:",
                dt,
                displayColumns,
                keyColumns,
                true, // Allow multiple selection
                style,
                chkRequired.Checked,
                false,
                null,
                this
            );

            if (results != null)
            {
                txtResults.AppendText($"Complex Selection ({results.Count} items): IDs = {string.Join(", ", results)}\r\n");
            }
            else
            {
                txtResults.AppendText("Complex Selection: Cancelled\r\n");
            }
        }

        private void btnComplexSelectionSingle_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            // Sample products
            var products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
                new Product { ProductID = 2, Name = "Mouse", Price = 19.99m, Category = "Electronics" },
                new Product { ProductID = 3, Name = "Keyboard", Price = 49.99m, Category = "Electronics" },
                new Product { ProductID = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics" },
                new Product { ProductID = 5, Name = "Desk", Price = 199.99m, Category = "Furniture" },
                new Product { ProductID = 6, Name = "Chair", Price = 149.99m, Category = "Furniture" }
            };

            List<string> displayProps = new List<string> { "Name", "Price", "Category" };
            List<string> keyProps = new List<string> { "ProductID" };

            List<object> results = ComplexObjectSelectionDialog.ShowDialog(
                "Select a Product",
                "Please select a product:",
                products,
                displayProps,
                keyProps,
                false, // Single selection
                style,
                chkRequired.Checked,
                false,
                null,
                this
            );

            if (results != null && results.Count > 0)
            {
                txtResults.AppendText($"Selected Product ID: {results[0]}\r\n");
            }
            else
            {
                txtResults.AppendText("Product Selection: Cancelled\r\n");
            }
        }

        #endregion

        #region ProcessingDialog Tests

        private void btnProcessingIndeterminate_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            // Show processing dialog on a background thread
            Task.Run(() =>
            {
                using (var dialog = ProcessingDialogManager.Show(
                    "Processing",
                    "Please wait while we process your request...",
                    style,
                    null,
                    true))
                {
                    // Simulate some work
                    Thread.Sleep(3000);

                    dialog.UpdateMessage("Almost done...");
                    Thread.Sleep(2000);
                }

                this.Invoke(new Action(() =>
                {
                    txtResults.AppendText("Processing (Indeterminate): Completed\r\n");
                }));
            });
        }

        private void btnProcessingDeterminate_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;

            // Show processing dialog with progress
            Task.Run(() =>
            {
                using (var dialog = ProcessingDialogManager.Show(
                    "Processing",
                    "Processing items...",
                    style,
                    null,
                    false))
                {
                    for (int i = 0; i <= 100; i += 10)
                    {
                        dialog.Update($"Processing item {i / 10 + 1} of 11...", i);
                        Thread.Sleep(300);
                    }
                }

                this.Invoke(new Action(() =>
                {
                    txtResults.AppendText("Processing (Determinate): Completed\r\n");
                }));
            });
        }

        #endregion

        #region Utility Methods

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            txtResults.Clear();
        }

        private void btnTestAllStyles_Click(object sender, EventArgs e)
        {
            foreach (DialogStyle style in Enum.GetValues(typeof(DialogStyle)))
            {
                string result = TextInputDialog.ShowDialog(
                    $"{style} Style Test",
                    $"This is a test of the {style} dialog style. Enter anything:",
                    style,
                    TextInputFormat.None,
                    false,
                    "",
                    null,
                    this
                );

                if (result != null)
                {
                    txtResults.AppendText($"{style}: {result}\r\n");
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region Helper Classes

        private class Product
        {
            public int ProductID { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
        }

        #endregion

        private void btnSqlConnection_Click(object sender, EventArgs e)
        {
            DialogStyle style = (DialogStyle)cmbDialogStyle.SelectedItem;
            SqlConnectForm sqlConnectForm = new SqlConnectForm();
            sqlConnectForm.Style = style;
            if (sqlConnectForm.ShowDialog(this) == DialogResult.OK)
            {
                txtResults.AppendText($"SQL Connection String: {sqlConnectForm.ConnectionString}\r\n");
            }
            else
            {
                txtResults.AppendText("SQL Connection: Cancelled\r\n");
            }

        }
    }
}
