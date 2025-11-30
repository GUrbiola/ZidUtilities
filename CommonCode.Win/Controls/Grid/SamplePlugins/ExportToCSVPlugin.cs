using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.SamplePlugins
{
    /// <summary>
    /// Sample plugin that exports the grid data to CSV format.
    /// This is a demonstration of how to implement IZidGridPlugin.
    /// </summary>
    public class ExportToCSVPlugin : IZidGridPlugin
    {
        public string MenuText => "Export to CSV";

        public Image MenuImage => null; // You can set an icon here

        public bool Enabled => true;

        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null || context.DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export to CSV",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Show save file dialog
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.FileName = "GridData.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder csv = new StringBuilder();

                    // Add headers
                    for (int i = 0; i < context.DataGridView.Columns.Count; i++)
                    {
                        if (context.DataGridView.Columns[i].Visible)
                        {
                            csv.Append(EscapeCSV(context.DataGridView.Columns[i].HeaderText));
                            if (i < context.DataGridView.Columns.Count - 1)
                                csv.Append(",");
                        }
                    }
                    csv.AppendLine();

                    // Add rows
                    foreach (DataGridViewRow row in context.DataGridView.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < context.DataGridView.Columns.Count; i++)
                            {
                                if (context.DataGridView.Columns[i].Visible)
                                {
                                    object cellValue = row.Cells[i].Value;
                                    string value = cellValue != null ? cellValue.ToString() : "";
                                    csv.Append(EscapeCSV(value));
                                    if (i < context.DataGridView.Columns.Count - 1)
                                        csv.Append(",");
                                }
                            }
                            csv.AppendLine();
                        }
                    }

                    File.WriteAllText(saveDialog.FileName, csv.ToString());
                    MessageBox.Show($"Data exported successfully to {saveDialog.FileName}",
                        "Export to CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Export to CSV",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // If the value contains comma, quote, or newline, wrap it in quotes
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // Escape quotes by doubling them
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}
