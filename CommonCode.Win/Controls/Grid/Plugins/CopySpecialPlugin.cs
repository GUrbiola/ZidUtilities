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
        public string MenuText => "Copy Special...";

        public Image MenuImage => Resources.Copy32;

        public bool Enabled => true;

        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null)
                return;

            // Show copy special dialog
            using (var copyDialog = new CopySpecialDialog(context.DataGridView, context.Theme))
            {
                copyDialog.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Copy format options
    /// </summary>
    public enum CopyFormat
    {
        PlainText,
        CSV,
        HTML,
        JSON,
        SQL
    }

    /// <summary>
    /// Dialog for selecting copy format
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

        public CopySpecialDialog(DataGridView grid, ZidThemes theme)
        {
            _grid = grid;
            _theme = theme;
            InitializeComponent();
        }

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

        private void FormatChanged(object sender, EventArgs e)
        {
            // Show/hide table name for SQL format
            bool isSql = rbSQL.Checked;
            lblTableName.Visible = isSql;
            txtTableName.Visible = isSql;
        }

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

        private CopyFormat GetSelectedFormat()
        {
            if (rbPlainText.Checked) return CopyFormat.PlainText;
            if (rbCSV.Checked) return CopyFormat.CSV;
            if (rbHTML.Checked) return CopyFormat.HTML;
            if (rbJSON.Checked) return CopyFormat.JSON;
            if (rbSQL.Checked) return CopyFormat.SQL;
            return CopyFormat.PlainText;
        }

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

        private string GeneratePlainText(bool includeHeaders, bool selectedOnly, char delimiter)
        {
            var sb = new StringBuilder();
            var columns = GetVisibleColumns();

            // Headers
            if (includeHeaders)
            {
                sb.AppendLine(string.Join(delimiter.ToString(), columns.Select(c => c.HeaderText)));
            }

            // Rows
            foreach (DataGridViewRow row in GetRowsToProcess(selectedOnly))
            {
                var values = new List<string>();
                foreach (var column in columns)
                {
                    var cellValue = row.Cells[column.Index].Value;
                    string value = cellValue == null ? "" : cellValue.ToString();

                    // Escape if needed
                    if (delimiter == ',' && value.Contains(','))
                    {
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    }

                    values.Add(value);
                }
                sb.AppendLine(string.Join(delimiter.ToString(), values));
            }

            return sb.ToString();
        }

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
                        values.Add("'" + cellValue.ToString().Replace("'", "''") + "'");
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

        private List<DataGridViewColumn> GetVisibleColumns()
        {
            return _grid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();
        }

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

        private string JsonEncode(object value)
        {
            if (value == null)
                return "null";

            if (value is string)
                return "\"" + value.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

            if (value is bool)
                return value.ToString().ToLower();

            if (value is DateTime)
                return "\"" + ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss") + "\"";

            return value.ToString();
        }
    }
}
