using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A generic dialog form for selecting one or more complex objects from a grid with filtering support.
    /// Supports DataTable and generic List&lt;T&gt; as data sources with multiple display columns and key fields.
    /// </summary>
    public partial class ComplexObjectSelectionDialog : Form
    {
        #region Fields

        private bool _required = false;
        private bool _allowMultipleSelection = false;
        private ZidThemes _theme = ZidThemes.Default;
        private DataTable _dataSource = null;
        private List<string> _keyMembers = new List<string>();
        private List<object> _selectedKeyValues = new List<object>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string DialogTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        /// <summary>
        /// Gets or sets the message displayed to the user.
        /// </summary>
        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        /// <summary>
        /// Gets or sets whether at least one selection is required.
        /// </summary>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        /// <summary>
        /// Gets or sets whether multiple row selection is allowed.
        /// </summary>
        public bool AllowMultipleSelection
        {
            get { return _allowMultipleSelection; }
            set
            {
                _allowMultipleSelection = value;
                dataGridView.MultiSelect = value;
                dataGridView.SelectionMode = value
                    ? DataGridViewSelectionMode.FullRowSelect
                    : DataGridViewSelectionMode.FullRowSelect;
            }
        }

        /// <summary>
        /// Gets or sets the dialog theme (color scheme).
        /// </summary>
        public ZidThemes Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                ApplyTheme();
            }
        }

        /// <summary>
        /// Gets or sets the image displayed in the dialog.
        /// </summary>
        public Image DialogImage
        {
            get { return pictureBox.Image; }
            set
            {
                pictureBox.Image = value;
                pictureBox.Visible = (value != null);
            }
        }

        /// <summary>
        /// Gets the selected key values.
        /// For single key field: List of single values.
        /// For multiple key fields: List of object arrays containing key values.
        /// </summary>
        public List<object> SelectedKeyValues
        {
            get { return _selectedKeyValues; }
        }

        /// <summary>
        /// Gets the selected rows as a list of DataRow objects.
        /// </summary>
        public List<DataRow> SelectedRows
        {
            get
            {
                List<DataRow> rows = new List<DataRow>();
                foreach (DataGridViewRow gridRow in dataGridView.SelectedRows)
                {
                    if (gridRow.Index >= 0 && gridRow.Index < _dataSource.Rows.Count)
                    {
                        rows.Add(_dataSource.Rows[gridRow.Index]);
                    }
                }
                return rows;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of ComplexObjectSelectionDialog.
        /// </summary>
        public ComplexObjectSelectionDialog()
        {
            InitializeComponent();
            ApplyTheme();
            AllowMultipleSelection = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the data source from a DataTable.
        /// </summary>
        /// <param name="dataTable">The data source.</param>
        /// <param name="displayMembers">List of column names to display.</param>
        /// <param name="keyMembers">List of column names that make up the key.</param>
        /// <param name="hideKeyColumns">Whether to hide key columns if they're not in display members.</param>
        public void SetDataSource(DataTable dataTable, List<string> displayMembers,
            List<string> keyMembers, bool hideKeyColumns = false)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));
            if (displayMembers == null || displayMembers.Count == 0)
                throw new ArgumentException("At least one display member is required.");
            if (keyMembers == null || keyMembers.Count == 0)
                throw new ArgumentException("At least one key member is required.");

            _dataSource = dataTable.Copy();
            _keyMembers = new List<string>(keyMembers);

            // Set up data grid
            dataGridView.DataSource = _dataSource;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.Clear();

            // Add display columns
            foreach (string displayMember in displayMembers)
            {
                if (_dataSource.Columns.Contains(displayMember))
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.DataPropertyName = displayMember;
                    column.HeaderText = displayMember;
                    column.Name = displayMember;
                    column.ReadOnly = true;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView.Columns.Add(column);
                }
            }

            // Add key columns if not already in display members
            if (!hideKeyColumns)
            {
                foreach (string keyMember in keyMembers)
                {
                    if (!displayMembers.Contains(keyMember) && _dataSource.Columns.Contains(keyMember))
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.DataPropertyName = keyMember;
                        column.HeaderText = keyMember;
                        column.Name = keyMember;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dataGridView.Columns.Add(column);
                    }
                }
            }

            UpdateRowCount();
        }

        /// <summary>
        /// Sets the data source from a generic list.
        /// </summary>
        public void SetDataSource<T>(List<T> items, List<string> displayProperties,
            List<string> keyProperties, bool hideKeyColumns = false)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (displayProperties == null || displayProperties.Count == 0)
                throw new ArgumentException("At least one display property is required.");
            if (keyProperties == null || keyProperties.Count == 0)
                throw new ArgumentException("At least one key property is required.");

            // Convert to DataTable
            _dataSource = new DataTable();
            Type type = typeof(T);

            // Get all properties we'll need
            HashSet<string> allProperties = new HashSet<string>(displayProperties);
            foreach (string key in keyProperties)
                allProperties.Add(key);

            // Create columns
            foreach (string propName in allProperties)
            {
                PropertyInfo prop = type.GetProperty(propName);
                if (prop != null)
                {
                    Type columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    _dataSource.Columns.Add(propName, columnType);
                }
            }

            // Add rows
            foreach (T item in items)
            {
                DataRow row = _dataSource.NewRow();
                foreach (string propName in allProperties)
                {
                    PropertyInfo prop = type.GetProperty(propName);
                    if (prop != null)
                    {
                        object value = prop.GetValue(item);
                        row[propName] = value ?? DBNull.Value;
                    }
                }
                _dataSource.Rows.Add(row);
            }

            SetDataSource(_dataSource, displayProperties, keyProperties, hideKeyColumns);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the selected theme to the dialog.
        /// </summary>
        private void ApplyTheme()
        {
            Color headerColor = DialogStyleHelper.GetHeaderColor(_theme);
            Color headerTextColor = DialogStyleHelper.GetHeaderTextColor(_theme);
            Color accentColor = DialogStyleHelper.GetAccentColor(_theme);

            pnlHeader.BackColor = headerColor;
            lblMessage.ForeColor = headerTextColor;
            btnOK.BackColor = accentColor;
            btnOK.ForeColor = Color.White;
        }

        /// <summary>
        /// Updates the row count label.
        /// </summary>
        private void UpdateRowCount()
        {
            if (_dataSource != null)
            {
                lblRowCount.Text = string.Format("{0} row(s)", dataGridView.Rows.Count);
            }
        }

        /// <summary>
        /// Updates the selected count label.
        /// </summary>
        private void UpdateSelectedCount()
        {
            lblSelectedCount.Text = string.Format("{0} selected", dataGridView.SelectedRows.Count);
        }

        /// <summary>
        /// Applies filter to the data grid.
        /// </summary>
        private void ApplyFilter()
        {
            if (_dataSource == null)
                return;

            string filter = txtFilter.Text.Trim();

            if (string.IsNullOrWhiteSpace(filter))
            {
                (_dataSource as DataTable).DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // Build filter expression for all string columns
                List<string> filterParts = new List<string>();
                string escapedFilter = filter.Replace("'", "''");

                foreach (DataColumn column in _dataSource.Columns)
                {
                    if (column.DataType == typeof(string))
                    {
                        filterParts.Add(string.Format("{0} LIKE '%{1}%'", column.ColumnName, escapedFilter));
                    }
                }

                if (filterParts.Count > 0)
                {
                    (_dataSource as DataTable).DefaultView.RowFilter = string.Join(" OR ", filterParts);
                }
            }

            UpdateRowCount();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles filter text change.
        /// </summary>
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Handles selection changed in data grid.
        /// </summary>
        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedCount();
        }

        /// <summary>
        /// Handles double-click on grid row (same as OK for single selection).
        /// </summary>
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_allowMultipleSelection && e.RowIndex >= 0)
            {
                btnOK_Click(sender, e);
            }
        }

        /// <summary>
        /// Handles OK button click.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_required && dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one item.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extract key values from selected rows
            _selectedKeyValues.Clear();
            foreach (DataGridViewRow gridRow in dataGridView.SelectedRows)
            {
                if (gridRow.Index >= 0 && gridRow.Index < _dataSource.Rows.Count)
                {
                    DataRow dataRow = _dataSource.Rows[gridRow.Index];

                    if (_keyMembers.Count == 1)
                    {
                        // Single key field
                        _selectedKeyValues.Add(dataRow[_keyMembers[0]]);
                    }
                    else
                    {
                        // Multiple key fields - return as object array
                        object[] keyValues = new object[_keyMembers.Count];
                        for (int i = 0; i < _keyMembers.Count; i++)
                        {
                            keyValues[i] = dataRow[_keyMembers[i]];
                        }
                        _selectedKeyValues.Add(keyValues);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles Cancel button click.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Shows a complex object selection dialog with a DataTable.
        /// </summary>
        public static List<object> ShowDialog(string title, string message, DataTable dataTable,
            List<string> displayMembers, List<string> keyMembers, bool allowMultiple = false,
            ZidThemes theme = ZidThemes.Default, bool required = false, bool hideKeyColumns = false,
            Image image = null, IWin32Window owner = null)
        {
            using (ComplexObjectSelectionDialog dialog = new ComplexObjectSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.AllowMultipleSelection = allowMultiple;
                dialog.DialogImage = image;
                dialog.SetDataSource(dataTable, displayMembers, keyMembers, hideKeyColumns);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedKeyValues;

                return null;
            }
        }

        /// <summary>
        /// Shows a complex object selection dialog with a generic list.
        /// </summary>
        public static List<object> ShowDialog<T>(string title, string message, List<T> items,
            List<string> displayProperties, List<string> keyProperties, bool allowMultiple = false,
            ZidThemes theme = ZidThemes.Default, bool required = false, bool hideKeyColumns = false,
            Image image = null, IWin32Window owner = null)
        {
            using (ComplexObjectSelectionDialog dialog = new ComplexObjectSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.AllowMultipleSelection = allowMultiple;
                dialog.DialogImage = image;
                dialog.SetDataSource(items, displayProperties, keyProperties, hideKeyColumns);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedKeyValues;

                return null;
            }
        }

        #endregion
    }
}
