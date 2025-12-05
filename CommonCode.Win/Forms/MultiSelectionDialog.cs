using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A generic dialog form for selecting multiple items from a list with filtering support.
    /// Supports DataTable and generic List&lt;T&gt; as data sources.
    /// </summary>
    public partial class MultiSelectionDialog : Form
    {
        #region Fields

        private bool _required = false;
        private ZidThemes _theme = ZidThemes.Default;
        private DataTable _dataSource = null;
        private string _displayMember = null;
        private string _valueMember = null;
        private List<object> _selectedValues = new List<object>();

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
        /// Gets the selected values (key values).
        /// </summary>
        public List<object> SelectedValues
        {
            get { return _selectedValues; }
        }

        /// <summary>
        /// Gets the selected display texts.
        /// </summary>
        public List<string> SelectedTexts
        {
            get
            {
                List<string> texts = new List<string>();
                foreach (object item in checkedListBoxItems.CheckedItems)
                {
                    texts.Add(item.ToString());
                }
                return texts;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MultiSelectionDialog.
        /// </summary>
        public MultiSelectionDialog()
        {
            InitializeComponent();
            ApplyTheme();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the data source from a DataTable.
        /// </summary>
        public void SetDataSource(DataTable dataTable, string displayMember, string valueMember)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            _dataSource = dataTable.Copy();
            _displayMember = displayMember;
            _valueMember = valueMember;

            LoadItems();
        }

        /// <summary>
        /// Sets the data source from a generic list.
        /// </summary>
        public void SetDataSource<T>(List<T> items, Func<T, string> displaySelector, Func<T, object> valueSelector)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            // Convert to DataTable
            _dataSource = new DataTable();
            _dataSource.Columns.Add("Display", typeof(string));
            _dataSource.Columns.Add("Value", typeof(object));

            foreach (T item in items)
            {
                DataRow row = _dataSource.NewRow();
                row["Display"] = displaySelector(item);
                row["Value"] = valueSelector(item);
                _dataSource.Rows.Add(row);
            }

            _displayMember = "Display";
            _valueMember = "Value";

            LoadItems();
        }

        /// <summary>
        /// Sets the data source from a simple string list (display and value are the same).
        /// </summary>
        public void SetDataSource(List<string> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _dataSource = new DataTable();
            _dataSource.Columns.Add("Display", typeof(string));
            _dataSource.Columns.Add("Value", typeof(string));

            foreach (string item in items)
            {
                DataRow row = _dataSource.NewRow();
                row["Display"] = item;
                row["Value"] = item;
                _dataSource.Rows.Add(row);
            }

            _displayMember = "Display";
            _valueMember = "Value";

            LoadItems();
        }

        /// <summary>
        /// Sets the data source from a dictionary.
        /// </summary>
        public void SetDataSource(Dictionary<string, object> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _dataSource = new DataTable();
            _dataSource.Columns.Add("Display", typeof(string));
            _dataSource.Columns.Add("Value", typeof(object));

            foreach (var kvp in items)
            {
                DataRow row = _dataSource.NewRow();
                row["Display"] = kvp.Key;
                row["Value"] = kvp.Value;
                _dataSource.Rows.Add(row);
            }

            _displayMember = "Display";
            _valueMember = "Value";

            LoadItems();
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
        /// Loads items from the data source into the checked list box.
        /// </summary>
        private void LoadItems()
        {
            ApplyFilter();
        }

        /// <summary>
        /// Applies the current filter to the checked list box.
        /// </summary>
        private void ApplyFilter()
        {
            if (_dataSource == null)
                return;

            // Store currently checked items
            List<object> checkedValues = new List<object>();
            foreach (CheckBoxItem item in checkedListBoxItems.CheckedItems)
            {
                checkedValues.Add(item.Value);
            }

            checkedListBoxItems.Items.Clear();

            string filter = txtFilter.Text.Trim();
            DataRow[] rows;

            if (string.IsNullOrWhiteSpace(filter))
            {
                rows = _dataSource.Select();
            }
            else
            {
                // Escape single quotes in the filter
                string escapedFilter = filter.Replace("'", "''");
                string filterExpression = string.Format("{0} LIKE '%{1}%'", _displayMember, escapedFilter);
                rows = _dataSource.Select(filterExpression);
            }

            foreach (DataRow row in rows)
            {
                CheckBoxItem item = new CheckBoxItem(row[_displayMember].ToString(), row[_valueMember]);
                int index = checkedListBoxItems.Items.Add(item);

                // Restore checked state if this item was previously checked
                if (checkedValues.Contains(item.Value))
                {
                    checkedListBoxItems.SetItemChecked(index, true);
                }
            }

            // Update labels
            lblItemCount.Text = string.Format("{0} item(s)", checkedListBoxItems.Items.Count);
            UpdateSelectedCount();
        }

        /// <summary>
        /// Updates the selected count label.
        /// </summary>
        private void UpdateSelectedCount()
        {
            lblSelectedCount.Text = string.Format("{0} selected", checkedListBoxItems.CheckedItems.Count);
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
        /// Handles checked list box item check changed.
        /// </summary>
        private void checkedListBoxItems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Update count after the check state changes
            this.BeginInvoke(new Action(() => UpdateSelectedCount()));
        }

        /// <summary>
        /// Handles Select All button click.
        /// </summary>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxItems.Items.Count; i++)
            {
                checkedListBoxItems.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// Handles Deselect All button click.
        /// </summary>
        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxItems.Items.Count; i++)
            {
                checkedListBoxItems.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// Handles OK button click.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_required && checkedListBoxItems.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedValues.Clear();
            foreach (CheckBoxItem item in checkedListBoxItems.CheckedItems)
            {
                _selectedValues.Add(item.Value);
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

        #region Helper Classes

        /// <summary>
        /// Internal class to hold display text and value for checked list box items.
        /// </summary>
        private class CheckBoxItem
        {
            public string Display { get; set; }
            public object Value { get; set; }

            public CheckBoxItem(string display, object value)
            {
                Display = display;
                Value = value;
            }

            public override string ToString()
            {
                return Display;
            }
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Shows a multi-selection dialog with a string list.
        /// </summary>
        public static List<object> ShowDialog(string title, string message, List<string> items,
            ZidThemes theme = ZidThemes.Default, bool required = false,
            Image image = null, IWin32Window owner = null)
        {
            using (MultiSelectionDialog dialog = new MultiSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(items);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValues;

                return null;
            }
        }

        /// <summary>
        /// Shows a multi-selection dialog with a DataTable.
        /// </summary>
        public static List<object> ShowDialog(string title, string message, DataTable dataTable,
            string displayMember, string valueMember, ZidThemes theme = ZidThemes.Default,
            bool required = false, Image image = null, IWin32Window owner = null)
        {
            using (MultiSelectionDialog dialog = new MultiSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(dataTable, displayMember, valueMember);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValues;

                return null;
            }
        }

        /// <summary>
        /// Shows a multi-selection dialog with a generic list.
        /// </summary>
        public static List<object> ShowDialog<T>(string title, string message, List<T> items,
            Func<T, string> displaySelector, Func<T, object> valueSelector,
            ZidThemes theme = ZidThemes.Default, bool required = false,
            Image image = null, IWin32Window owner = null)
        {
            using (MultiSelectionDialog dialog = new MultiSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(items, displaySelector, valueSelector);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValues;

                return null;
            }
        }

        #endregion
    }
}
