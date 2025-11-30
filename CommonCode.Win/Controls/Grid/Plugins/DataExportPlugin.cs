using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Files;
using ZidUtilities.CommonCode.Win.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.Plugins
{
    /// <summary>
    /// Plugin that exports ZidGrid data to various formats (TXT, CSV, XLSX, HTML)
    /// using the DataExporter class from CommonCode.Files.
    /// </summary>
    public class DataExportPlugin : IZidGridPlugin
    {
        public string MenuText => "Export Data...";

        public Image MenuImage => null; // You can set an icon here

        public bool Enabled => true;

        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null || context.DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show export options dialog
            using (var exportDialog = new ExportOptionsDialog())
            {
                if (exportDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the data to export
                    DataTable dataTable = GetDataTableFromGrid(context);

                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Unable to retrieve data for export.", "Export Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    bool success = false;
                    string errorMessage = null;
                    string filePath = exportDialog.SelectedFilePath;

                    // Create processing dialog manager (runs on separate UI thread)
                    using (var dialogManager = new ProcessingDialogManager(
                        "Exporting Data",
                        "Preparing export...",
                        DialogStyle.Information,
                        null,
                        true))
                    {
                        // Perform the export using DataExporter with progress reporting
                        PerformExportWithProgress(dataTable, exportDialog.SelectedFormat,
                            exportDialog.SelectedFilePath, dialogManager,
                            out success, out errorMessage);

                        // Dialog will be closed when dialogManager is disposed
                    }

                    // Show result
                    if (success)
                    {
                        MessageBox.Show($"Data exported successfully to:\n{filePath}",
                            "Export Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Error exporting data:\n{errorMessage}", "Export Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Extracts data from the DataGridView into a DataTable.
        /// </summary>
        private DataTable GetDataTableFromGrid(ZidGridPluginContext context)
        {
            // If we have a DataSource, try to use it
            if (context.DataSource != null)
            {
                // Try to convert various data source types to DataTable
                if (context.DataSource is DataTable dt)
                {
                    return dt.Copy();
                }
                else if (context.DataSource is DataView dv)
                {
                    return dv.ToTable();
                }
                else if (context.DataSource is DataSet ds && ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Copy();
                }
            }

            // Fallback: manually construct DataTable from grid
            DataTable table = new DataTable("GridData");

            // Add columns
            foreach (DataGridViewColumn column in context.DataGridView.Columns)
            {
                if (column.Visible)
                {
                    table.Columns.Add(column.HeaderText ?? column.Name, typeof(string));
                }
            }

            // Add rows
            foreach (DataGridViewRow row in context.DataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dataRow = table.NewRow();
                    int columnIndex = 0;

                    foreach (DataGridViewColumn column in context.DataGridView.Columns)
                    {
                        if (column.Visible)
                        {
                            object cellValue = row.Cells[column.Index].Value;
                            dataRow[columnIndex] = cellValue != null ? cellValue.ToString() : "";
                            columnIndex++;
                        }
                    }

                    table.Rows.Add(dataRow);
                }
            }

            return table;
        }

        /// <summary>
        /// Performs the export using the DataExporter class with progress reporting.
        /// Uses ProcessingDialogManager for proper thread-safe dialog updates.
        /// </summary>
        private void PerformExportWithProgress(DataTable data, ExportTo format, string filePath,
            ProcessingDialogManager dialogManager, out bool success, out string errorMessage)
        {
            success = false;
            errorMessage = null;

            try
            {
                // Create and configure DataExporter
                DataExporter exporter = new DataExporter();
                exporter.ExportType = format;
                exporter.WriteHeaders = true;
                exporter.ExportWithStyles = true;
                exporter.UseAlternateRowStyles = true;
                // Always use Simple style for clean, professional output
                exporter.ExportExcelStyle = ExcelStyle.Simple;
                exporter.ExportHtmlStyle = ExcelStyle.Simple;

                // Hook up progress events for real-time updates
                exporter.OnStartExportation += (firedAt, records, progress, exportType) =>
                {
                    dialogManager.UpdateMessage($"Exporting {records} rows...");
                    dialogManager.UpdateProgress(0);
                };

                exporter.OnProgress += (firedAt, records, progress, exportType) =>
                {
                    int percentage = records > 0 ? (int)((progress * 100.0) / records) : 0;
                    dialogManager.Update($"Exporting row {progress} of {records}...", percentage);
                };

                // Perform synchronous export
                // The DataExporter will fire progress events which update the dialog
                exporter.ExportToFile(filePath, data, false);

                // Success
                dialogManager.UpdateMessage("Export completed successfully!");
                Thread.Sleep(500); // Brief pause to show completion message
                success = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                dialogManager.UpdateMessage($"Export failed: {ex.Message}");
                Thread.Sleep(1000); // Show error message
            }
        }
    }

    /// <summary>
    /// Dialog for selecting export format and options.
    /// </summary>
    internal class ExportOptionsDialog : Form
    {
        private ComboBox cmbFormat;
        private TextBox txtFilePath;
        private Button btnBrowse;
        private Button btnOK;
        private Button btnCancel;
        private Label lblFormat;
        private Label lblFilePath;

        public ExportTo SelectedFormat { get; private set; }
        public string SelectedFilePath { get; private set; }

        public ExportOptionsDialog()
        {
            InitializeComponent();
            PopulateFormats();
        }

        private void InitializeComponent()
        {
            this.Text = "Export Data";
            this.Size = new Size(500, 215);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Format label and combo
            lblFormat = new Label();
            lblFormat.Text = "Export Format:";
            lblFormat.Location = new Point(20, 20);
            lblFormat.AutoSize = true;

            cmbFormat = new ComboBox();
            cmbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFormat.Location = new Point(20, 45);
            cmbFormat.Size = new Size(440, 25);
            cmbFormat.SelectedIndexChanged += CmbFormat_SelectedIndexChanged;

            // File path label, textbox, and browse button
            lblFilePath = new Label();
            lblFilePath.Text = "Save to:";
            lblFilePath.Location = new Point(20, 80);
            lblFilePath.AutoSize = true;

            txtFilePath = new TextBox();
            txtFilePath.Location = new Point(20, 105);
            txtFilePath.Size = new Size(360, 25);
            txtFilePath.ReadOnly = true;

            btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Location = new Point(385, 103);
            btnBrowse.Size = new Size(75, 27);
            btnBrowse.Click += BtnBrowse_Click;

            // OK and Cancel buttons
            btnOK = new Button();
            btnOK.Text = "Export";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(295, 140);
            btnOK.Size = new Size(80, 30);
            btnOK.Enabled = false;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(380, 140);
            btnCancel.Size = new Size(80, 30);

            this.Controls.AddRange(new Control[] {
                lblFormat, cmbFormat,
                lblFilePath, txtFilePath, btnBrowse,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void PopulateFormats()
        {
            cmbFormat.Items.Add(new FormatItem("Excel Workbook (*.xlsx)", ExportTo.XLSX, "xlsx"));
            cmbFormat.Items.Add(new FormatItem("CSV File (*.csv)", ExportTo.CSV, "csv"));
            cmbFormat.Items.Add(new FormatItem("Text File (*.txt)", ExportTo.TXT, "txt"));
            cmbFormat.Items.Add(new FormatItem("HTML File (*.html)", ExportTo.HTML, "html"));
            cmbFormat.SelectedIndex = 0;
        }

        private void CmbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear file path when format changes
            txtFilePath.Text = "";
            btnOK.Enabled = false;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (cmbFormat.SelectedItem == null)
            {
                MessageBox.Show("Please select an export format first.", "Export Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormatItem selectedFormat = (FormatItem)cmbFormat.SelectedItem;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = $"{selectedFormat.Description}|*.{selectedFormat.Extension}|All files (*.*)|*.*";
            saveDialog.DefaultExt = selectedFormat.Extension;
            saveDialog.FileName = $"GridData_{DateTime.Now:yyyyMMdd_HHmmss}.{selectedFormat.Extension}";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = saveDialog.FileName;
                SelectedFormat = selectedFormat.Format;
                SelectedFilePath = saveDialog.FileName;

                btnOK.Enabled = true;
            }
        }

        private class FormatItem
        {
            public string Description { get; set; }
            public ExportTo Format { get; set; }
            public string Extension { get; set; }

            public FormatItem(string description, ExportTo format, string extension)
            {
                Description = description;
                Format = format;
                Extension = extension;
            }

            public override string ToString()
            {
                return Description;
            }
        }
    }
}
