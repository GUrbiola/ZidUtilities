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

        public event PluginExecuted OnPluginExecuted;

        /// <summary>
        /// Executes the plugin by validating the context and showing the quick filter dialog for the selected column.
        /// </summary>
        /// <param name="context">The plugin execution context containing the DataGridView, column, column index and theme.
        /// Must contain a non-null DataGridView and a valid ColumnIndex.</param>
        /// <returns>None. The method shows a modal dialog and returns after it is closed.</returns>
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
                OnPluginExecuted?.Invoke(context, "QuickFilterPlugin");
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

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickFilterDialog"/> class.
        /// Sets internal fields, constructs UI and loads the unique values for the specified column.
        /// </summary>
        /// <param name="grid">The DataGridView to operate on. Must not be null.</param>
        /// <param name="columnIndex">Zero-based index of the column to filter.</param>
        /// <param name="theme">Theme to apply to the dialog visuals.</param>
        public QuickFilterDialog(DataGridView grid, int columnIndex, ZidThemes theme)
        {
            _grid = grid;
            _columnIndex = columnIndex;
            _theme = theme;
            InitializeComponent();
            LoadUniqueValues();
        }

        /// <summary>
        /// Builds and configures all visual controls used by the dialog.
        /// Attaches event handlers to controls (search box, buttons).
        /// </summary>
        /// <remarks>
        /// This method configures sizes, locations, colors and basic styles for controls.
        /// It does not return a value; it mutates the dialog's Controls collection.
        /// </remarks>
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

        /// <summary>
        /// Scans the target column in the grid and populates the checklist with unique string representations
        /// of cell values. Null or DB nulls are represented as "(Blank)". All items are checked by default.
        /// </summary>
        /// <remarks>
        /// This method clears the checklist first and then adds sorted unique values.
        /// It does not return a value.
        /// </remarks>
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

        /// <summary>
        /// Handles the TextChanged event for the search textbox. Filters the checklist items to only show
        /// those that contain the search text (case-insensitive). Preserves checked state of matching items.
        /// </summary>
        /// <param name="sender">The textbox control that raised the event.</param>
        /// <param name="e">Event arguments (unused).</param>
        /// <returns>None. The method updates the checked list box in place.</returns>
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

        /// <summary>
        /// Event handler that marks all items in the checklist as checked.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Click event arguments (unused).</param>
        /// <returns>None. All items in the checklist will be checked after this call.</returns>
        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkValues.Items.Count; i++)
            {
                chkValues.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// Event handler that marks all items in the checklist as unchecked.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Click event arguments (unused).</param>
        /// <returns>None. All items in the checklist will be unchecked after this call.</returns>
        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkValues.Items.Count; i++)
            {
                chkValues.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// Event handler that applies the selected value filter to the DataGridView.
        /// Collects selected values from the checklist and hides rows whose cell value does not match any selected value.
        /// Shows informational or error messages as appropriate.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Click event arguments (unused).</param>
        /// <returns>None. The grid row visibility is modified; dialog is closed with DialogResult.OK on success.</returns>
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

                // Determine a safe current cell: first row that will remain visible
                int safeRowIndex = -1;
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string displayValue = GetDisplayValueForRow(row);
                    if (selectedValues.Contains(displayValue))
                    {
                        safeRowIndex = row.Index;
                        break;
                    }
                }

                // Try to set a safe current cell before hiding rows to avoid InvalidOperationException
                try
                {
                    if (safeRowIndex >= 0 && _grid.Columns.Count > 0)
                    {
                        // Prefer the same column cell to remain current if possible
                        if (_grid.Rows[safeRowIndex].Cells.Count > _columnIndex)
                        {
                            _grid.CurrentCell = _grid.Rows[safeRowIndex].Cells[_columnIndex];
                        }
                        else if (_grid.Rows[safeRowIndex].Cells.Count > 0)
                        {
                            _grid.CurrentCell = _grid.Rows[safeRowIndex].Cells[0];
                        }
                    }
                    else
                    {
                        // No row will remain visible; try to clear CurrentCell to allow hiding rows
                        try
                        {
                            _grid.CurrentCell = null;
                        }
                        catch
                        {
                            // ignore; some bindings may not allow null CurrentCell
                        }
                    }
                }
                catch
                {
                    // If moving CurrentCell fails for some reason, proceed but be defensive when hiding rows
                }

                // Apply visibility changes safely
                _grid.SuspendLayout();
                _grid.ClearSelection();

                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    try
                    {
                        string displayValue = GetDisplayValueForRow(row);
                        bool shouldBeVisible = selectedValues.Contains(displayValue);

                        // If the row is the current row and we're trying to hide it, skip or attempt to move current cell again
                        if (!shouldBeVisible && _grid.CurrentCell != null && row.Index == _grid.CurrentCell.RowIndex)
                        {
                            // Try to move current cell to the safeRowIndex if it's still valid, otherwise try to clear it
                            try
                            {
                                if (safeRowIndex >= 0 && safeRowIndex != row.Index && safeRowIndex < _grid.Rows.Count)
                                {
                                    if (_grid.Rows[safeRowIndex].Cells.Count > _columnIndex)
                                        _grid.CurrentCell = _grid.Rows[safeRowIndex].Cells[_columnIndex];
                                    else if (_grid.Rows[safeRowIndex].Cells.Count > 0)
                                        _grid.CurrentCell = _grid.Rows[safeRowIndex].Cells[0];
                                }
                                else
                                {
                                    _grid.CurrentCell = null;
                                }
                            }
                            catch
                            {
                                // If unable to move CurrentCell, skip hiding this row to avoid exception
                                continue;
                            }
                        }

                        row.Visible = shouldBeVisible;
                    }
                    catch (InvalidOperationException)
                    {
                        // If making the row invisible fails, skip hiding this row to avoid crashing the UI.
                        // This is a safe fallback; the user can try again or use Clear Filter which makes all rows visible.
                        continue;
                    }
                    catch
                    {
                        // Any unexpected exception per-row: skip and continue
                        continue;
                    }
                }

                _grid.ResumeLayout();

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

        /// <summary>
        /// Event handler that clears any applied filter by making all rows visible again.
        /// Shows a confirmation message and closes the dialog with DialogResult.OK on success.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Click event arguments (unused).</param>
        /// <returns>None. All rows in the bound DataGridView are made visible.</returns>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            try
            {
                _grid.SuspendLayout();
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    row.Visible = true;
                }
                _grid.ResumeLayout();

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

        private string GetDisplayValueForRow(DataGridViewRow row)
        {
            var cellValue = row.Cells[_columnIndex].Value;
            return cellValue == null || cellValue == DBNull.Value ? "(Blank)" : cellValue.ToString();
        }
    }
}
