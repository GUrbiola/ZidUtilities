using System;
using System.Data;
using System.Drawing;
using System.IO;
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
        /// <summary>
        /// Gets the display text for the menu item.
        /// </summary>
        public string MenuText => "Export Data...";

        /// <summary>
        /// Gets the image/icon for the menu item (optional).
        /// </summary>
        public Image MenuImage => Resources.Export32; // You can set an icon here

        /// <summary>
        /// Gets whether the menu item is enabled.
        /// </summary>
        public bool Enabled => true;

        public event PluginExecuted OnPluginExecuted;

        /// <summary>
        /// Executes the plugin: shows the export options dialog, converts the grid or datasource to a DataTable,
        /// and performs the export to the user-selected file and format.
        /// </summary>
        /// <param name="context">The plugin execution context containing grid and data source references.</param>
        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null || context.DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show export options dialog
            using (var exportDialog = new ExportOptionsDialog(context.Theme))
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

                    PerformExport(dataTable, exportDialog.SelectedFormat, exportDialog.SelectedFilePath);
                    OnPluginExecuted?.Invoke(context, "DataExportPlugin");
                }
            }
        }

        /// <summary>
        /// Extracts data from the DataGridView or fallback datasource into a DataTable.
        /// Builds the DataTable based on visible columns (respecting display order),
        /// converts cell values to appropriate types where possible, and skips new/invisible rows.
        /// </summary>
        /// <param name="context">Plugin context holding the DataGridView and optional DataSource.</param>
        /// <returns>
        /// A DataTable representing the current grid view or a copy/converted table from the datasource;
        /// returns null if no suitable data is available.
        /// </returns>
        private DataTable GetDataTableFromGrid(ZidGridPluginContext context)
        {
            // If context or DataGridView is missing, fallback to original DataSource handling
            if (context == null)
                return null;

            DataGridView grid = context.DataGridView;

            // If we have a grid, build table from what's currently displayed to respect sorting/filtering/column order
            if (grid != null)
            {
                // Get visible columns ordered by display index (this respects user column reordering)
                var visibleColumns = new System.Collections.Generic.List<DataGridViewColumn>();
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (col.Visible)
                        visibleColumns.Add(col);
                }
                visibleColumns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

                DataTable table = new DataTable("GridData");

                // Add columns with unique names; prefer HeaderText then Name
                foreach (DataGridViewColumn col in visibleColumns)
                {
                    string baseName = string.IsNullOrWhiteSpace(col.HeaderText) ? col.Name ?? $"Column{col.Index}" : col.HeaderText;
                    string columnName = baseName;
                    int suffix = 1;
                    while (table.Columns.Contains(columnName))
                    {
                        columnName = $"{baseName}_{suffix}";
                        suffix++;
                    }

                    // Try to set a sensible type: use the column's ValueType if available, otherwise string
                    Type columnType = col.ValueType ?? typeof(string);

                    // Use string as fallback if columnType is Object to avoid schema problems for heterogeneous cells
                    if (columnType == typeof(object))
                        columnType = typeof(string);

                    table.Columns.Add(columnName, columnType);
                }

                // Add rows in the order they appear in the grid (this respects sorting and filtering)
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    // Some filters / bindings may keep rows but mark them not visible; skip invisible rows
                    if (!row.Visible)
                        continue;

                    DataRow dtRow = table.NewRow();
                    for (int i = 0; i < visibleColumns.Count; i++)
                    {
                        DataGridViewColumn col = visibleColumns[i];

                        // Access cell by column index to get the bound value
                        DataGridViewCell cell = row.Cells[col.Index];
                        object value = cell?.Value;

                        // Convert DBNull/ null appropriately
                        if (value == null)
                            dtRow[i] = DBNull.Value;
                        else
                        {
                            // If DataTable column expects string but value is not string, convert to string
                            Type expected = table.Columns[i].DataType;
                            try
                            {
                                if (value == DBNull.Value)
                                    dtRow[i] = DBNull.Value;
                                else if (expected == typeof(string))
                                    dtRow[i] = value.ToString();
                                else if (expected.IsInstanceOfType(value))
                                    dtRow[i] = value;
                                else
                                    dtRow[i] = Convert.ChangeType(value, expected);
                            }
                            catch
                            {
                                // Fallback to string representation on conversion failure
                                dtRow[i] = value.ToString();
                            }
                        }
                    }

                    table.Rows.Add(dtRow);
                }

                return table;
            }

            // If no grid is available, fall back to previous DataSource conversions
            if (context.DataSource != null)
            {
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
                else if (context.DataSource is System.Windows.Forms.BindingSource bs)
                {
                    // Try to extract DataView/DataTable from BindingSource
                    if (bs.List is DataView bsDv)
                        return bsDv.ToTable();
                    if (bs.DataSource is DataTable bsDt)
                        return bsDt.Copy();
                    if (bs.DataSource is DataSet bsDs && bsDs.Tables.Count > 0)
                        return bsDs.Tables[0].Copy();
                }
            }

            // Nothing available
            return null;
        }

        /// <summary>
        /// Performs the export using the DataExporter class with progress reporting shown in a ProcessingDialog.
        /// Configures export options, attaches progress/completion handlers, and invokes the export to file.
        /// </summary>
        /// <param name="data">The DataTable to export.</param>
        /// <param name="format">The export format to use (XLSX/CSV/TXT/HTML).</param>
        /// <param name="filePath">Destination file path where the exported file will be saved.</param>
        private void PerformExport(DataTable data, ExportTo format, string filePath)
        {
            using 
            (
                var dialogManager = new ProcessingDialogManager
                (
                    "Exporting Data",
                    "Preparing export...",
                    DialogStyle.Information, 
                    false
                )
            )
            {
                try
                {
                    // Create and configure DataExporter
                    var exporter = new DataExporter
                    {
                        ExportType = format,
                        WriteHeaders = true,
                        ExportWithStyles = true,
                        UseAlternateRowStyles = true,
                        ExportExcelStyle = ExcelStyle.Simple,
                        ExportHtmlStyle = ExcelStyle.Simple
                    };

                    // Hook up progress events
                    exporter.OnStartExportation += (firedAt, records, progress, exportType) =>
                    {
                        dialogManager.Update($"Exporting {records:N0} rows...", 0);
                    };

                    exporter.OnProgress += (firedAt, records, progress, exportType) =>
                    {
                        int percentage = records > 0 ? (int)((progress * 100.0) / records) : 0;
                        dialogManager.Update($"Exporting row {progress:N0} of {records:N0}...", percentage);
                    };

                    exporter.OnCompletedExportation += (firedAt, records, exportType, streamResult, pathResult) =>
                    {
                        dialogManager.Update("Export completed successfully!", 100);
                    };

                    // Perform the export
                    exporter.ExportToFile(filePath, data, false);

                    Thread.Sleep(500); // Brief pause to show completion message
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data:\n{ex.Message}",
                        "Export Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    /// <summary>
    /// Dialog for selecting export format and target file path.
    /// Presents format choices and allows browsing for a destination file.
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
        private Panel pnlHeader;
        private Label lblTitle;
        private GridThemeHelper.ThemeColors _themeColors;

        /// <summary>
        /// Gets the user-selected export format.
        /// </summary>
        public ExportTo SelectedFormat { get; private set; }

        /// <summary>
        /// Gets the user-selected file path to save the export.
        /// </summary>
        public string SelectedFilePath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ExportOptionsDialog with the supplied theme.
        /// </summary>
        /// <param name="theme">Theme to apply to the dialog controls.</param>
        public ExportOptionsDialog(ZidThemes theme)
        {
            _themeColors = GridThemeHelper.GetThemeColors(theme);
            InitializeComponent();
            ApplyTheme();
            PopulateFormats();
        }

        /// <summary>
        /// Initializes and lays out dialog controls (labels, combo, textbox, buttons).
        /// This method configures control properties and event handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Export Data";
            this.Size = new Size(500, 280);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Header panel
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            lblTitle = new Label();
            lblTitle.Text = "Export Grid Data";
            lblTitle.Font = new Font("Verdana", 10f, FontStyle.Bold);
            lblTitle.Location = new Point(15, 15);
            lblTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblTitle);

            // Format label and combo
            lblFormat = new Label();
            lblFormat.Text = "Export Format:";
            lblFormat.Location = new Point(20, 65);
            lblFormat.AutoSize = true;

            cmbFormat = new ComboBox();
            cmbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFormat.Location = new Point(20, 90);
            cmbFormat.Size = new Size(440, 25);
            cmbFormat.SelectedIndexChanged += CmbFormat_SelectedIndexChanged;

            // File path label, textbox, and browse button
            lblFilePath = new Label();
            lblFilePath.Text = "Save to:";
            lblFilePath.Location = new Point(20, 125);
            lblFilePath.AutoSize = true;

            txtFilePath = new TextBox();
            txtFilePath.Location = new Point(20, 150);
            txtFilePath.Size = new Size(360, 25);
            txtFilePath.ReadOnly = true;

            btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Location = new Point(385, 148);
            btnBrowse.Size = new Size(75, 27);
            btnBrowse.Click += BtnBrowse_Click;

            // OK and Cancel buttons
            btnOK = new Button();
            btnOK.Text = "Export";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(295, 200);
            btnOK.Size = new Size(80, 30);
            btnOK.Enabled = false;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(380, 200);
            btnCancel.Size = new Size(80, 30);

            this.Controls.AddRange(new Control[] {
                pnlHeader,
                lblFormat, cmbFormat,
                lblFilePath, txtFilePath, btnBrowse,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        /// <summary>
        /// Applies the provided theme colors and fonts to the dialog's controls.
        /// </summary>
        private void ApplyTheme()
        {
            // Apply theme to header panel
            pnlHeader.BackColor = _themeColors.HeaderBackColor;
            lblTitle.ForeColor = _themeColors.HeaderForeColor;

            // Apply theme to form
            this.BackColor = _themeColors.BackgroundColor;

            // Apply theme to labels
            lblFormat.ForeColor = _themeColors.DefaultForeColor;
            lblFormat.Font = _themeColors.CellFont;
            lblFilePath.ForeColor = _themeColors.DefaultForeColor;
            lblFilePath.Font = _themeColors.CellFont;

            // Apply theme to controls
            cmbFormat.BackColor = _themeColors.DefaultBackColor;
            cmbFormat.ForeColor = _themeColors.DefaultForeColor;
            cmbFormat.Font = _themeColors.CellFont;

            txtFilePath.BackColor = _themeColors.DefaultBackColor;
            txtFilePath.ForeColor = _themeColors.DefaultForeColor;
            txtFilePath.Font = _themeColors.CellFont;

            // Apply theme to buttons
            btnBrowse.BackColor = _themeColors.DefaultBackColor;
            btnBrowse.ForeColor = _themeColors.DefaultForeColor;
            btnBrowse.FlatStyle = FlatStyle.Flat;

            btnOK.BackColor = _themeColors.HeaderBackColor;
            btnOK.ForeColor = _themeColors.HeaderForeColor;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font(_themeColors.HeaderFont.FontFamily, 9f, FontStyle.Bold);

            btnCancel.BackColor = _themeColors.DefaultBackColor;
            btnCancel.ForeColor = _themeColors.DefaultForeColor;
            btnCancel.FlatStyle = FlatStyle.Flat;
        }

        /// <summary>
        /// Populates the format selection ComboBox with supported export formats.
        /// Each entry includes a description, enum value and default extension.
        /// </summary>
        private void PopulateFormats()
        {
            cmbFormat.Items.Add(new FormatItem("Excel Workbook (*.xlsx)", ExportTo.XLSX, "xlsx"));
            cmbFormat.Items.Add(new FormatItem("CSV File (*.csv)", ExportTo.CSV, "csv"));
            cmbFormat.Items.Add(new FormatItem("Text File (*.txt)", ExportTo.TXT, "txt"));
            cmbFormat.Items.Add(new FormatItem("HTML File (*.html)", ExportTo.HTML, "html"));
            cmbFormat.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles changes to the selected export format.
        /// Clears any previously chosen file path and disables the Export button until a new file is selected.
        /// </summary>
        /// <param name="sender">Event sender (ComboBox).</param>
        /// <param name="e">Event arguments.</param>
        private void CmbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear file path when format changes
            txtFilePath.Text = "";
            btnOK.Enabled = false;
        }

        /// <summary>
        /// Opens a SaveFileDialog configured for the selected format, updates SelectedFormat and SelectedFilePath
        /// when the user chooses a file, and enables the Export button.
        /// </summary>
        /// <param name="sender">Event sender (Browse button).</param>
        /// <param name="e">Event arguments.</param>
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

        /// <summary>
        /// Simple container for format ComboBox items.
        /// Holds a user-facing description, the ExportTo value and the default file extension.
        /// </summary>
        private class FormatItem
        {
            /// <summary>
            /// Description shown in the ComboBox (e.g. "Excel Workbook (*.xlsx)").
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Export format enum value represented by this item.
            /// </summary>
            public ExportTo Format { get; set; }

            /// <summary>
            /// Default file extension (without dot) for this format.
            /// </summary>
            public string Extension { get; set; }

            /// <summary>
            /// Initializes a new FormatItem with the specified description, format and extension.
            /// </summary>
            /// <param name="description">User-visible description.</param>
            /// <param name="format">ExportTo enum value.</param>
            /// <param name="extension">Default file extension (e.g. "xlsx").</param>
            public FormatItem(string description, ExportTo format, string extension)
            {
                Description = description;
                Format = format;
                Extension = extension;
            }

            /// <summary>
            /// Returns the description to display in the ComboBox.
            /// </summary>
            /// <returns>The Description property.</returns>
            public override string ToString()
            {
                return Description;
            }
        }
    }
}
