using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.Plugins
{
    /// <summary>
    /// Plugin that allows freezing/unfreezing columns in the grid
    /// </summary>
    public class FreezeColumnsPlugin : IZidGridPlugin
    {
        /// <summary>
        /// Gets the display text for the plugin menu item.
        /// </summary>
        public string MenuText => "Freeze/Unfreeze Columns";

        /// <summary>
        /// Gets the image/icon for the plugin menu item.
        /// </summary>
        public Image MenuImage => Resources.Freeze32;

        /// <summary>
        /// Gets whether the plugin menu item is enabled.
        /// </summary>
        public bool Enabled => true;

        public event PluginExecuted OnPluginExecuted;

        /// <summary>
        /// Executes the plugin functionality by showing a dialog that allows the user
        /// to freeze or unfreeze columns in the provided DataGridView.
        /// </summary>
        /// <param name="context">
        /// The execution context containing the DataGridView to operate on and the current theme.
        /// The method will return immediately if <see cref="ZidGridPluginContext.DataGridView"/> is null.
        /// </param>
        /// <returns>
        /// This method does not return a value. It may show UI to the user and mutate the grid's column Frozen state.
        /// </returns>
        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null)
                return;

            // Show dialog to select freeze option
            using (var freezeDialog = new FreezeColumnsDialog(context.DataGridView, context.Theme))
            {
                freezeDialog.ShowDialog();
                OnPluginExecuted?.Invoke(context, "FreezeColumnsPlugin");
            }
        }
    }

    /// <summary>
    /// Dialog for freezing/unfreezing columns
    /// </summary>
    internal class FreezeColumnsDialog : Form
    {
        private DataGridView _grid;
        private ZidThemes _theme;
        private ListBox lstColumns;
        private Button btnFreeze;
        private Button btnUnfreezeAll;
        private Button btnClose;
        private Label lblInstructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezeColumnsDialog"/> class.
        /// </summary>
        /// <param name="grid">The DataGridView whose columns can be frozen or unfrozen by this dialog.</param>
        /// <param name="theme">The theme to apply to the dialog UI elements.</param>
        public FreezeColumnsDialog(DataGridView grid, ZidThemes theme)
        {
            _grid = grid;
            _theme = theme;
            InitializeComponent();
            LoadColumns();
        }

        /// <summary>
        /// Initializes and lays out UI components used by the dialog.
        /// This sets up labels, list box for columns, and action buttons.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Freeze Columns";
            this.Size = new Size(500, 400);
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
                Text = "Freeze Columns",
                Font = new Font(colors.HeaderFont.FontFamily, 14F, FontStyle.Bold),
                ForeColor = colors.HeaderForeColor,
                Location = new Point(15, 15),
                AutoSize = true
            };
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // Instructions
            lblInstructions = new Label
            {
                Text = "Select columns to freeze to the left. Frozen columns stay visible while scrolling.",
                Location = new Point(15, 70),
                Size = new Size(460, 40),
                Font = new Font(colors.CellFont.FontFamily, 9F)
            };
            this.Controls.Add(lblInstructions);

            // Columns list
            lstColumns = new ListBox
            {
                Location = new Point(15, 115),
                Size = new Size(460, 180),
                SelectionMode = SelectionMode.MultiExtended,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            this.Controls.Add(lstColumns);

            // Buttons
            btnFreeze = new Button
            {
                Text = "Freeze Selected",
                Location = new Point(15, 310),
                Size = new Size(140, 35),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnFreeze.FlatAppearance.BorderColor = colors.AccentColor;
            btnFreeze.Click += BtnFreeze_Click;
            this.Controls.Add(btnFreeze);

            btnUnfreezeAll = new Button
            {
                Text = "Unfreeze All",
                Location = new Point(165, 310),
                Size = new Size(140, 35),
                BackColor = colors.HeaderBackColor,
                ForeColor = colors.HeaderForeColor,
                FlatStyle = FlatStyle.Flat
            };
            btnUnfreezeAll.FlatAppearance.BorderColor = colors.AccentColor;
            btnUnfreezeAll.Click += BtnUnfreezeAll_Click;
            this.Controls.Add(btnUnfreezeAll);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(335, 310),
                Size = new Size(140, 35),
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
        /// Loads visible columns from the grid into the list box and selects any columns that are already frozen.
        /// </summary>
        /// <remarks>
        /// The list displays each visible column using the format "HeaderText (Name)".
        /// The selection indexes correspond to the visible columns order.
        /// </remarks>
        private void LoadColumns()
        {
            lstColumns.Items.Clear();

            foreach (DataGridViewColumn column in _grid.Columns)
            {
                if (column.Visible)
                {
                    lstColumns.Items.Add(column.HeaderText + " (" + column.Name + ")");

                    // Select already frozen columns
                    if (column.Frozen)
                    {
                        lstColumns.SetSelected(lstColumns.Items.Count - 1, true);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event for the Freeze Selected button.
        /// Freezes columns from the left up to and including the rightmost selected visible column.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">Event arguments for the click event.</param>
        /// <returns>
        /// None. On success the dialog sets its DialogResult to OK and closes. Shows message boxes for feedback or errors.
        /// </returns>
        private void BtnFreeze_Click(object sender, EventArgs e)
        {
            if (lstColumns.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select at least one column to freeze.", "Freeze Columns",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // First, unfreeze all columns
                foreach (DataGridViewColumn column in _grid.Columns)
                {
                    column.Frozen = false;
                }

                // Find the rightmost selected column index
                int maxIndex = -1;
                foreach (int selectedIndex in lstColumns.SelectedIndices)
                {
                    int visibleIndex = 0;
                    for (int i = 0; i < _grid.Columns.Count; i++)
                    {
                        if (_grid.Columns[i].Visible)
                        {
                            if (visibleIndex == selectedIndex)
                            {
                                if (i > maxIndex)
                                    maxIndex = i;
                                break;
                            }
                            visibleIndex++;
                        }
                    }
                }

                // Freeze all columns up to and including the rightmost selected
                if (maxIndex >= 0)
                {
                    for (int i = 0; i <= maxIndex; i++)
                    {
                        if (_grid.Columns[i].Visible)
                        {
                            _grid.Columns[i].Frozen = true;
                        }
                    }
                }

                MessageBox.Show("Columns frozen successfully!", "Freeze Columns",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error freezing columns: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event for the Unfreeze All button.
        /// Unfreezes all columns in the associated DataGridView and refreshes the displayed column list.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">Event arguments for the click event.</param>
        private void BtnUnfreezeAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewColumn column in _grid.Columns)
                {
                    column.Frozen = false;
                }

                MessageBox.Show("All columns unfrozen!", "Freeze Columns",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadColumns(); // Refresh the list
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error unfreezing columns: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
