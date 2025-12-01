using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid.Plugins
{
    /// <summary>
    /// Plugin that allows users to show/hide columns in the ZidGrid.
    /// </summary>
    public class ColumnVisibilityPlugin : IZidGridPlugin
    {
        public string MenuText => "Column Visibility...";

        public Image MenuImage => Resources.ColumnPick32;

        public bool Enabled => true;

        public void Execute(ZidGridPluginContext context)
        {
            if (context.DataGridView == null || context.DataGridView.Columns.Count == 0)
            {
                MessageBox.Show("No columns available.", "Column Visibility",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new ColumnVisibilityDialog(context.DataGridView, context.Theme))
            {
                dialog.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Dialog for selecting which columns to show/hide.
    /// </summary>
    internal class ColumnVisibilityDialog : Form
    {
        private CheckedListBox chkListColumns;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private Button btnOK;
        private Button btnCancel;
        private Panel pnlHeader;
        private Label lblTitle;
        private DataGridView _grid;
        private GridThemeHelper.ThemeColors _themeColors;

        public ColumnVisibilityDialog(DataGridView grid, ZidThemes theme)
        {
            _grid = grid;
            _themeColors = GridThemeHelper.GetThemeColors(theme);

            InitializeComponent();
            ApplyTheme();
            PopulateColumns();
        }

        private void InitializeComponent()
        {
            this.Text = "Column Visibility";
            this.Size = new Size(450, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Header panel
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            lblTitle = new Label();
            lblTitle.Text = "Select columns to display";
            lblTitle.Font = new Font("Verdana", 10f, FontStyle.Bold);
            lblTitle.Location = new Point(15, 15);
            lblTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblTitle);

            // Checked list box
            chkListColumns = new CheckedListBox();
            chkListColumns.CheckOnClick = true;
            chkListColumns.Location = new Point(15, 65);
            chkListColumns.Size = new Size(405, 320);
            chkListColumns.IntegralHeight = false;

            // Select All button
            btnSelectAll = new Button();
            btnSelectAll.Text = "Select All";
            btnSelectAll.Location = new Point(15, 395);
            btnSelectAll.Size = new Size(100, 30);
            btnSelectAll.Click += BtnSelectAll_Click;

            // Deselect All button
            btnDeselectAll = new Button();
            btnDeselectAll.Text = "Deselect All";
            btnDeselectAll.Location = new Point(120, 395);
            btnDeselectAll.Size = new Size(100, 30);
            btnDeselectAll.Click += BtnDeselectAll_Click;

            // OK button
            btnOK = new Button();
            btnOK.Text = "Apply";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(235, 435);
            btnOK.Size = new Size(90, 30);
            btnOK.Click += BtnOK_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(330, 435);
            btnCancel.Size = new Size(90, 30);

            this.Controls.AddRange(new Control[] {
                pnlHeader, chkListColumns,
                btnSelectAll, btnDeselectAll,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void ApplyTheme()
        {
            // Apply theme to header panel
            pnlHeader.BackColor = _themeColors.HeaderBackColor;
            lblTitle.ForeColor = _themeColors.HeaderForeColor;

            // Apply theme to form
            this.BackColor = _themeColors.BackgroundColor;

            // Apply theme to checked list box
            chkListColumns.BackColor = _themeColors.DefaultBackColor;
            chkListColumns.ForeColor = _themeColors.DefaultForeColor;
            chkListColumns.Font = _themeColors.CellFont;

            // Apply theme to buttons
            Color buttonBackColor = _themeColors.DefaultBackColor;
            Color buttonForeColor = _themeColors.DefaultForeColor;

            btnSelectAll.BackColor = buttonBackColor;
            btnSelectAll.ForeColor = buttonForeColor;
            btnSelectAll.FlatStyle = FlatStyle.Flat;

            btnDeselectAll.BackColor = buttonBackColor;
            btnDeselectAll.ForeColor = buttonForeColor;
            btnDeselectAll.FlatStyle = FlatStyle.Flat;

            btnOK.BackColor = _themeColors.HeaderBackColor;
            btnOK.ForeColor = _themeColors.HeaderForeColor;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font(_themeColors.HeaderFont.FontFamily, 9f, FontStyle.Bold);

            btnCancel.BackColor = buttonBackColor;
            btnCancel.ForeColor = buttonForeColor;
            btnCancel.FlatStyle = FlatStyle.Flat;
        }

        private void PopulateColumns()
        {
            chkListColumns.Items.Clear();

            foreach (DataGridViewColumn column in _grid.Columns)
            {
                string columnName = string.IsNullOrWhiteSpace(column.HeaderText)
                    ? column.Name
                    : column.HeaderText;

                var item = new ColumnItem
                {
                    Column = column,
                    DisplayName = columnName
                };

                chkListColumns.Items.Add(item, column.Visible);
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkListColumns.Items.Count; i++)
            {
                chkListColumns.SetItemChecked(i, true);
            }
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkListColumns.Items.Count; i++)
            {
                chkListColumns.SetItemChecked(i, false);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Apply visibility changes
            for (int i = 0; i < chkListColumns.Items.Count; i++)
            {
                var columnItem = (ColumnItem)chkListColumns.Items[i];
                bool isChecked = chkListColumns.GetItemChecked(i);
                columnItem.Column.Visible = isChecked;
            }
        }

        private class ColumnItem
        {
            public DataGridViewColumn Column { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}
