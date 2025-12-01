using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.Plugins
{
    /// <summary>
    /// Plugin that provides quick filtering by unique column values
    /// </summary>
    public class QuickFilterPlugin : IZidGridPlugin
    {
        public string MenuText => "Quick Filter (Unique Values)";

        public Image MenuImage => Resources.Flash32;

        public bool Enabled => true;

        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null || context.Column == null)
                return;

            if (context.ColumnIndex < 0 || context.ColumnIndex >= context.DataGridView.Columns.Count)
                return;

            // Show quick filter dialog
            using (var filterDialog = new QuickFilterDialog(context.DataGridView, context.ColumnIndex, context.Theme))
            {
                filterDialog.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Dialog for quick filtering by unique values
    /// </summary>
    internal class QuickFilterDialog : Form
    {
        private DataGridView _grid;
        private int _columnIndex;
        private ZidThemes _theme;
        private CheckedListBox chkValues;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private Button btnApply;
        private Button btnClear;
        private Button btnClose;
        private Label lblColumnName;
        private TextBox txtSearch;

        public QuickFilterDialog(DataGridView grid, int columnIndex, ZidThemes theme)
        {
            _grid = grid;
            _columnIndex = columnIndex;
            _theme = theme;
            InitializeComponent();
            LoadUniqueValues();
        }

        private void InitializeComponent()
        {
            this.Text = "Quick Filter";
            this.Size = new Size(500, 550);
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
                Text = "Quick Filter",
                Font = new Font(colors.HeaderFont.FontFamily, 14F, FontStyle.Bold),
                ForeColor = colors.HeaderForeColor,
                Location = new Point(15, 15),
                AutoSize = true
            };
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // Column name
            lblColumnName = new Label
            {
                Text = "Filter by: " + _grid.Columns[_columnIndex].HeaderText,
                Location = new Point(15, 70),
                Size = new Size(460, 25),
                Font = new Font(colors.CellFont.FontFamily, 10F, FontStyle.Bold)
            };
            this.Controls.Add(lblColumnName);

            // Search box
            Label lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(15, 100),
                AutoSize = true
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(75, 97),
                Size = new Size(400, 25),
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // Values checklist
            chkValues = new CheckedListBox
            {
                Location = new Point(15, 130),
                Size = new Size(460, 280),
                CheckOnClick = true,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            this.Controls.Add(chkValues);

            // Select/Deselect buttons
            btnSelectAll = new Button
            {
                Text = "Select All",
                Location = new Point(15, 420),
                Size = new Size(110, 30),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnSelectAll.FlatAppearance.BorderColor = colors.AccentColor;
            btnSelectAll.Click += BtnSelectAll_Click;
            this.Controls.Add(btnSelectAll);

            btnDeselectAll = new Button
            {
                Text = "Deselect All",
                Location = new Point(135, 420),
                Size = new Size(110, 30),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnDeselectAll.FlatAppearance.BorderColor = colors.AccentColor;
            btnDeselectAll.Click += BtnDeselectAll_Click;
            this.Controls.Add(btnDeselectAll);

            // Action buttons
            btnApply = new Button
            {
                Text = "Apply Filter",
                Location = new Point(15, 460),
                Size = new Size(110, 35),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnApply.FlatAppearance.BorderColor = colors.AccentColor;
            btnApply.Click += BtnApply_Click;
            this.Controls.Add(btnApply);

            btnClear = new Button
            {
                Text = "Clear Filter",
                Location = new Point(135, 460),
                Size = new Size(110, 35),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnClear.FlatAppearance.BorderColor = colors.AccentColor;
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(365, 460),
                Size = new Size(110, 35),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnClose.FlatAppearance.BorderColor = colors.AccentColor;
            this.Controls.Add(btnClose);

            this.CancelButton = btnClose;
        }

        private void LoadUniqueValues()
        {
            chkValues.Items.Clear();

            var uniqueValues = new HashSet<string>();
            string columnName = _grid.Columns[_columnIndex].Name;

            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var cellValue = row.Cells[_columnIndex].Value;
                string displayValue = cellValue == null || cellValue == DBNull.Value
                    ? "(Blank)"
                    : cellValue.ToString();

                uniqueValues.Add(displayValue);
            }

            // Sort and add to checklist
            var sortedValues = uniqueValues.OrderBy(v => v).ToList();
            foreach (var value in sortedValues)
            {
                chkValues.Items.Add(value, true); // All checked by default
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            chkValues.BeginUpdate();

            // Save current checked state
            var checkedItems = new HashSet<string>();
            for (int i = 0; i < chkValues.Items.Count; i++)
            {
                if (chkValues.GetItemChecked(i))
                {
                    checkedItems.Add(chkValues.Items[i].ToString());
                }
            }

            // Reload values and filter
            LoadUniqueValues();

            // Filter items
            for (int i = chkValues.Items.Count - 1; i >= 0; i--)
            {
                string item = chkValues.Items[i].ToString();
                if (!item.ToLower().Contains(searchText))
                {
                    chkValues.Items.RemoveAt(i);
                }
                else
                {
                    // Restore checked state
                    chkValues.SetItemChecked(i, checkedItems.Contains(item));
                }
            }

            chkValues.EndUpdate();
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkValues.Items.Count; i++)
            {
                chkValues.SetItemChecked(i, true);
            }
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkValues.Items.Count; i++)
            {
                chkValues.SetItemChecked(i, false);
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected values
                var selectedValues = new List<string>();
                for (int i = 0; i < chkValues.Items.Count; i++)
                {
                    if (chkValues.GetItemChecked(i))
                    {
                        selectedValues.Add(chkValues.Items[i].ToString());
                    }
                }

                if (selectedValues.Count == 0)
                {
                    MessageBox.Show("Please select at least one value to filter.", "Quick Filter",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hide rows that don't match
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    var cellValue = row.Cells[_columnIndex].Value;
                    string displayValue = cellValue == null || cellValue == DBNull.Value
                        ? "(Blank)"
                        : cellValue.ToString();

                    row.Visible = selectedValues.Contains(displayValue);
                }

                MessageBox.Show($"Filter applied! Showing {selectedValues.Count} value(s).", "Quick Filter",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filter: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // Show all rows
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    row.Visible = true;
                }

                MessageBox.Show("Filter cleared. All rows are now visible.", "Quick Filter",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing filter: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
