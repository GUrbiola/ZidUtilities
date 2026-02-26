using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.Plugins
{
    /// <summary>
    /// Plugin that copies selected cells/rows in various formats
    /// </summary>
    public class CopySpecialPlugin : IZidGridPlugin
    {
        /// <summary>
        /// Gets the display text for the menu item.
        /// </summary>
        public string MenuText => "Copy Special...";

        /// <summary>
        /// Gets the image/icon for the menu item (optional, can be null).
        /// </summary>
        public Image MenuImage => Resources.Copy32;

        /// <summary>
        /// Gets whether the menu item is enabled.
        /// </summary>
        public bool Enabled => true;

        public event PluginExecuted OnPluginExecuted;

        /// <summary>
        /// Executes the plugin functionality by showing the Copy Special dialog.
        /// </summary>
        /// <param name="context">The plugin execution context containing grid and theme references.</param>
        /// <remarks>
        /// If the context does not contain a DataGridView, the method returns without action.
        /// The dialog is shown modally using ShowDialog and disposed afterwards.
        /// </remarks>
        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null)
                return;

            // Show copy special dialog
            using (var copyDialog = new CopySpecialDialog(context.DataGridView, context.Theme))
            {
                copyDialog.ShowDialog();
                OnPluginExecuted?.Invoke(context, "CopySpecialPlugin");
            }
        }
    }

    /// <summary>
    /// Copy format options enumeration.
    /// </summary>
    public enum CopyFormat
    {
        /// <summary>
        /// Plain text output, tab-delimited by default.
        /// </summary>
        PlainText,
        /// <summary>
        /// CSV output (comma-separated values).
        /// </summary>
        CSV,
        /// <summary>
        /// HTML table output.
        /// </summary>
        HTML,
        /// <summary>
        /// JSON array output.
        /// </summary>
        JSON,
        /// <summary>
        /// SQL INSERT statements output.
        /// </summary>
        SQL
    }

    /// <summary>
    /// Dialog for selecting copy format and options.
    /// </summary>
    internal class CopySpecialDialog : Form
    {
        private DataGridView _grid;
        private ZidThemes _theme;
        private RadioButton rbPlainText;
        private RadioButton rbCSV;
        private RadioButton rbHTML;
        private RadioButton rbJSON;
        private RadioButton rbSQL;
        private TextBox txtTableName;
        private Label lblTableName;
        private CheckBox chkIncludeHeaders;
        private CheckBox chkSelectedOnly;
        private Button btnCopy;
        private Button btnClose;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopySpecialDialog"/> class.
        /// </summary>
        /// <param name="grid">The DataGridView whose data will be copied.</param>
        /// <param name="theme">The theme to use for dialog styling.</param>
        public CopySpecialDialog(DataGridView grid, ZidThemes theme)
        {
            _grid = grid;
            _theme = theme;
            InitializeComponent();
        }

        /// <summary>
        /// Initializes UI components of the dialog and applies theming.
        /// </summary>
        /// <remarks>
        /// Builds the controls (radio buttons, checkboxes, buttons, etc.) and wires up event handlers.
        /// This method sets control properties such as locations, sizes, colors, and fonts.
        /// </remarks>
        private void InitializeComponent()
        {
            this.Text = "Copy Special";
            this.Size = new Size(500, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Apply theme colors
            var colors = GridThemeHelper.GetThemeColors(_theme);
            this.BackColor = colors.BackgroundColor;
            this.ForeColor = colors.DefaultForeColor;

            // Header panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = colors.HeaderBackColor
            };

            Label headerLabel = new Label
            {
                Text = "Copy Special",
                Font = new Font(colors.HeaderFont.FontFamily, 14F, FontStyle.Bold),
                ForeColor = colors.HeaderForeColor,
                Location = new Point(15, 15),
                AutoSize = true
            };
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // Format selection group
            GroupBox grpFormat = new GroupBox
            {
                Text = "Output Format",
                Location = new Point(15, 75),
                Size = new Size(460, 190),
                ForeColor = colors.HeaderBackColor,
                Font = new Font(colors.CellFont.FontFamily, 10F, FontStyle.Bold)
            };

            rbPlainText = new RadioButton
            {
                Text = "Plain Text (Tab-delimited)",
                Location = new Point(15, 25),
                Size = new Size(430, 25),
                Checked = true,
                ForeColor = colors.DefaultForeColor,
                Font = colors.CellFont
            };
            rbPlainText.CheckedChanged += FormatChanged;
            grpFormat.Controls.Add(rbPlainText);

            rbCSV = new RadioButton
            {
                Text = "CSV (Comma-separated)",
                Location = new Point(15, 55),
                Size = new Size(430, 25),
                ForeColor = colors.DefaultForeColor,
                Font = colors.CellFont
            };
            rbCSV.CheckedChanged += FormatChanged;
            grpFormat.Controls.Add(rbCSV);

            rbHTML = new RadioButton
            {
                Text = "HTML Table",
                Location = new Point(15, 85),
                Size = new Size(430, 25),
                ForeColor = colors.DefaultForeColor,
                Font = colors.CellFont
            };
            rbHTML.CheckedChanged += FormatChanged;
            grpFormat.Controls.Add(rbHTML);

            rbJSON = new RadioButton
            {
                Text = "JSON Array",
                Location = new Point(15, 115),
                Size = new Size(430, 25),
                ForeColor = colors.DefaultForeColor,
                Font = colors.CellFont
            };
            rbJSON.CheckedChanged += FormatChanged;
            grpFormat.Controls.Add(rbJSON);

            rbSQL = new RadioButton
            {
                Text = "SQL INSERT Statements",
                Location = new Point(15, 145),
                Size = new Size(430, 25),
                ForeColor = colors.DefaultForeColor,
                Font = colors.CellFont
            };
            rbSQL.CheckedChanged += FormatChanged;
            grpFormat.Controls.Add(rbSQL);

            this.Controls.Add(grpFormat);

            // Table name for SQL
            lblTableName = new Label
            {
                Text = "Table Name:",
                Location = new Point(30, 275),
                AutoSize = true,
                Visible = false
            };
            this.Controls.Add(lblTableName);

            txtTableName = new TextBox
            {
                Location = new Point(130, 272),
                Size = new Size(345, 25),
                Text = "MyTable",
                BackColor = Color.White,
                ForeColor = Color.Black,
                Visible = false
            };
            this.Controls.Add(txtTableName);

            // Options
            chkIncludeHeaders = new CheckBox
            {
                Text = "Include column headers",
                Location = new Point(30, 310),
                Size = new Size(250, 25),
                Checked = true,
                ForeColor = colors.DefaultForeColor
            };
            this.Controls.Add(chkIncludeHeaders);

            chkSelectedOnly = new CheckBox
            {
                Text = "Selected cells only",
                Location = new Point(30, 340),
                Size = new Size(250, 25),
                ForeColor = colors.DefaultForeColor
            };
            this.Controls.Add(chkSelectedOnly);

            // Action buttons
            btnCopy = new Button
            {
                Text = "Copy to Clipboard",
                Location = new Point(15, 390),
                Size = new Size(200, 40),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(colors.CellFont.FontFamily, 10F, FontStyle.Bold)
            };
            btnCopy.FlatAppearance.BorderColor = colors.AccentColor;
            btnCopy.Click += BtnCopy_Click;
            this.Controls.Add(btnCopy);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(325, 390),
                Size = new Size(150, 40),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Font = new Font(colors.CellFont.FontFamily, 10F)
            };
            btnClose.FlatAppearance.BorderColor = colors.AccentColor;
            this.Controls.Add(btnClose);

            this.CancelButton = btnClose;
        }

        /// <summary>
        /// Handles changes to the selected output format radio buttons.
        /// Shows or hides SQL-specific controls when SQL format is selected.
        /// </summary>
        /// <param name="sender">The radio button that triggered the event.</param>
        /// <param name="e">Event data.</param>
        private void FormatChanged(object sender, EventArgs e)
        {
            // Show/hide table name for SQL format
            bool isSql = rbSQL.Checked;
            lblTableName.Visible = isSql;
            txtTableName.Visible = isSql;
        }

        /// <summary>
        /// Click handler for the "Copy to Clipboard" button.
        /// Generates the output in the selected format and places it on the clipboard.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event data.</param>
        /// <remarks>
        /// Displays informational or error messages to the user via MessageBox.
        /// Sets DialogResult to OK on successful copy and closes the dialog.
        /// </remarks>
        private void BtnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                CopyFormat format = GetSelectedFormat();
                bool includeHeaders = chkIncludeHeaders.Checked;
                bool selectedOnly = chkSelectedOnly.Checked;

                string result = GenerateOutput(format, includeHeaders, selectedOnly);

                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("No data to copy.", "Copy Special",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Clipboard.SetText(result);

                MessageBox.Show($"Data copied to clipboard in {format} format!", "Copy Special",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Determines which copy format is currently selected by the user.
        /// </summary>
        /// <returns>The selected <see cref="CopyFormat"/> enum value. Defaults to <see cref="CopyFormat.PlainText"/>.</returns>
        private CopyFormat GetSelectedFormat()
        {
            if (rbPlainText.Checked) return CopyFormat.PlainText;
            if (rbCSV.Checked) return CopyFormat.CSV;
            if (rbHTML.Checked) return CopyFormat.HTML;
            if (rbJSON.Checked) return CopyFormat.JSON;
            if (rbSQL.Checked) return CopyFormat.SQL;
            return CopyFormat.PlainText;
        }

        /// <summary>
        /// Generates the output string in the requested format.
        /// </summary>
        /// <param name="format">The output format to generate.</param>
        /// <param name="includeHeaders">Whether to include column headers in the output (if supported by format).</param>
        /// <param name="selectedOnly">Whether to process only selected cells/rows.</param>
        /// <returns>A string containing the formatted output ready to be copied to the clipboard.</returns>
        private string GenerateOutput(CopyFormat format, bool includeHeaders, bool selectedOnly)
        {
            switch (format)
            {
                case CopyFormat.PlainText:
                    return GeneratePlainText(includeHeaders, selectedOnly, '\t');
                case CopyFormat.CSV:
                    return GeneratePlainText(includeHeaders, selectedOnly, ',');
                case CopyFormat.HTML:
                    return GenerateHTML(includeHeaders, selectedOnly);
                case CopyFormat.JSON:
                    return GenerateJSON(includeHeaders, selectedOnly);
                case CopyFormat.SQL:
                    return GenerateSQL(selectedOnly);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Generates plain-text or CSV output from the grid.
        /// </summary>
        /// <param name="includeHeaders">Whether to include column headers as the first line.</param>
        /// <param name="selectedOnly">Whether to include only selected rows/cells.</param>
        /// <param name="delimiter">The character used to separate values (e.g., '\t' for tab, ',' for CSV).</param>
        /// <returns>A string with rows separated by newline and values separated by the specified delimiter.</returns>
        private string GeneratePlainText(bool includeHeaders, bool selectedOnly, char delimiter)
        {
            var sb = new StringBuilder();
            var columns = GetVisibleColumns();

            // Headers
            if (includeHeaders)
            {
                if (delimiter == ',')
                {
                    sb.AppendLine(string.Join(",", columns.Select(c => CsvEscape(c.HeaderText))));
                }
                else
                {
                    sb.AppendLine(string.Join(delimiter.ToString(), columns.Select(c => c.HeaderText)));
                }
            }

            // Rows
            foreach (DataGridViewRow row in GetRowsToProcess(selectedOnly))
            {
                var values = new List<string>();
                foreach (var column in columns)
                {
                    var cellValue = row.Cells[column.Index].Value;
                    string value = cellValue == null ? "" : cellValue.ToString();

                    // Escape if CSV format
                    if (delimiter == ',')
                    {
                        value = CsvEscape(value);
                    }

                    values.Add(value);
                }
                sb.AppendLine(string.Join(delimiter.ToString(), values));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates an HTML table representation of the grid data.
        /// </summary>
        /// <param name="includeHeaders">Whether to include table header row.</param>
        /// <param name="selectedOnly">Whether to include only selected rows/cells.</param>
        /// <returns>A string containing an HTML table with the grid data.</returns>
        private string GenerateHTML(bool includeHeaders, bool selectedOnly)
        {
            var sb = new StringBuilder();
            var columns = GetVisibleColumns();

            sb.AppendLine("<table border='1'>");

            // Headers
            if (includeHeaders)
            {
                sb.AppendLine("  <thead>");
                sb.AppendLine("    <tr>");
                foreach (var column in columns)
                {
                    sb.AppendLine($"      <th>{HtmlEncode(column.HeaderText)}</th>");
                }
                sb.AppendLine("    </tr>");
                sb.AppendLine("  </thead>");
            }

            // Body
            sb.AppendLine("  <tbody>");
            foreach (DataGridViewRow row in GetRowsToProcess(selectedOnly))
            {
                sb.AppendLine("    <tr>");
                foreach (var column in columns)
                {
                    var cellValue = row.Cells[column.Index].Value;
                    string value = cellValue == null ? "" : cellValue.ToString();
                    sb.AppendLine($"      <td>{HtmlEncode(value)}</td>");
                }
                sb.AppendLine("    </tr>");
            }
            sb.AppendLine("  </tbody>");

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a JSON array representation of the grid data.
        /// </summary>
        /// <param name="includeHeaders">Whether headers should be considered (not used for JSON items names since headers are used as property names).</param>
        /// <param name="selectedOnly">Whether to include only selected rows/cells.</param>
        /// <returns>A string containing a JSON array where each element is an object representing a row.</returns>
        private string GenerateJSON(bool includeHeaders, bool selectedOnly)
        {
            var sb = new StringBuilder();
            var columns = GetVisibleColumns();

            sb.AppendLine("[");

            bool firstRow = true;
            foreach (DataGridViewRow row in GetRowsToProcess(selectedOnly))
            {
                if (!firstRow)
                    sb.AppendLine(",");

                sb.AppendLine("  {");

                bool firstCol = true;
                foreach (var column in columns)
                {
                    if (!firstCol)
                        sb.AppendLine(",");

                    var cellValue = row.Cells[column.Index].Value;
                    string value = cellValue == null ? "null" : JsonEncode(cellValue);

                    sb.Append($"    {JsonEncode(column.HeaderText)}: {value}");

                    firstCol = false;
                }

                sb.AppendLine();
                sb.Append("  }");

                firstRow = false;
            }

            sb.AppendLine();
            sb.AppendLine("]");

            return sb.ToString();
        }

        /// <summary>
        /// Generates SQL INSERT statements for the grid data.
        /// </summary>
        /// <param name="selectedOnly">Whether to include only selected rows/cells.</param>
        /// <returns>A string containing one or more SQL INSERT statements for the selected or all rows.</returns>
        private string GenerateSQL(bool selectedOnly)
        {
            var sb = new StringBuilder();
            var columns = GetVisibleColumns();
            string tableName = txtTableName.Text.Trim();

            if (string.IsNullOrEmpty(tableName))
                tableName = "MyTable";

            foreach (DataGridViewRow row in GetRowsToProcess(selectedOnly))
            {
                sb.Append($"INSERT INTO {tableName} (");
                sb.Append(string.Join(", ", columns.Select(c => c.Name)));
                sb.Append(") VALUES (");

                var values = new List<string>();
                foreach (var column in columns)
                {
                    var cellValue = row.Cells[column.Index].Value;

                    if (cellValue == null || cellValue == DBNull.Value)
                    {
                        values.Add("NULL");
                    }
                    else if (cellValue is string || cellValue is DateTime)
                    {
                        values.Add(SqlEscape(cellValue.ToString()));
                    }
                    else if (cellValue is bool)
                    {
                        values.Add((bool)cellValue ? "1" : "0");
                    }
                    else
                    {
                        values.Add(cellValue.ToString());
                    }
                }

                sb.Append(string.Join(", ", values));
                sb.AppendLine(");");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the visible columns from the grid ordered by display index.
        /// </summary>
        /// <returns>A list of visible <see cref="DataGridViewColumn"/> objects ordered by their DisplayIndex.</returns>
        private List<DataGridViewColumn> GetVisibleColumns()
        {
            return _grid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();
        }

        /// <summary>
        /// Enumerates the rows to process based on whether only selected cells should be included.
        /// </summary>
        /// <param name="selectedOnly">
        /// If true, only rows that contain selected cells are returned.
        /// If false, all non-new rows in the grid are returned.
        /// </param>
        /// <returns>An enumerable of <see cref="DataGridViewRow"/> for processing.</returns>
        private IEnumerable<DataGridViewRow> GetRowsToProcess(bool selectedOnly)
        {
            if (selectedOnly && _grid.SelectedCells.Count > 0)
            {
                // Get unique rows from selected cells
                var selectedRows = new HashSet<int>();
                foreach (DataGridViewCell cell in _grid.SelectedCells)
                {
                    if (!cell.OwningRow.IsNewRow)
                        selectedRows.Add(cell.RowIndex);
                }

                foreach (var rowIndex in selectedRows.OrderBy(i => i))
                {
                    yield return _grid.Rows[rowIndex];
                }
            }
            else
            {
                // All rows
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (!row.IsNewRow)
                        yield return row;
                }
            }
        }

        /// <summary>
        /// Encodes text for safe inclusion in HTML by replacing special characters with HTML entities.
        /// </summary>
        /// <param name="text">The input text to encode. May be null or empty.</param>
        /// <returns>The HTML-encoded string. If input is null or empty, the same value is returned.</returns>
        private string HtmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Replace("&", "&amp;")
                       .Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;")
                       .Replace("'", "&#39;");
        }

        /// <summary>
        /// Encodes an object value into a JSON-friendly string representation.
        /// </summary>
        /// <param name="value">The value to encode (may be null, string, bool, DateTime, or other).</param>
        /// <returns>
        /// A JSON literal as a string. For strings the result is quoted and escaped; for booleans it's "true"/"false";
        /// for DateTime it's an ISO-like quoted string; for null returns "null"; otherwise uses ToString().
        /// </returns>
        private string JsonEncode(object value)
        {
            if (value == null)
                return "null";

            if (value is string)
            {
                string str = value.ToString();
                str = str.Replace("\\", "\\\\")
                         .Replace("\"", "\\\"")
                         .Replace("\n", "\\n")
                         .Replace("\r", "\\r")
                         .Replace("\t", "\\t")
                         .Replace("\b", "\\b")
                         .Replace("\f", "\\f");
                return "\"" + str + "\"";
            }

            if (value is bool)
                return value.ToString().ToLower();

            if (value is DateTime)
                return "\"" + ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss") + "\"";

            return value.ToString();
        }

        /// <summary>
        /// Escapes a string value for safe inclusion in CSV format.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>
        /// The escaped string. If the value contains comma, quote, newline, or carriage return,
        /// it will be wrapped in quotes and any internal quotes will be doubled.
        /// </returns>
        private string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Need to wrap in quotes if contains comma, quote, newline, or carriage return
            bool needsQuoting = value.Contains(",") || value.Contains("\"") ||
                                value.Contains("\n") || value.Contains("\r");

            if (needsQuoting)
            {
                // Double any quotes and wrap in quotes
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        /// <summary>
        /// Escapes a string value for safe inclusion in SQL INSERT statements.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>
        /// A SQL-escaped string wrapped in single quotes. Single quotes are doubled,
        /// and newlines are replaced with CHAR(13)+CHAR(10) or CHAR(10) depending on the line ending type.
        /// </returns>
        private string SqlEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "''";

            // Escape single quotes by doubling them
            string escaped = value.Replace("'", "''");

            // Replace newlines with SQL CHAR functions
            // Handle Windows-style line endings (CRLF)
            if (escaped.Contains("\r\n"))
            {
                escaped = escaped.Replace("\r\n", "' + CHAR(13) + CHAR(10) + '");
            }
            // Handle Unix-style line endings (LF)
            else if (escaped.Contains("\n"))
            {
                escaped = escaped.Replace("\n", "' + CHAR(10) + '");
            }
            // Handle Mac-style line endings (CR)
            else if (escaped.Contains("\r"))
            {
                escaped = escaped.Replace("\r", "' + CHAR(13) + '");
            }

            // Clean up any empty string concatenations that might have been created
            escaped = escaped.Replace("'' + ", "").Replace(" + ''", "");

            return "'" + escaped + "'";
        }
    }
}
