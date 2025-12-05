using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A generic dialog form for selecting a single item from a list with filtering support.
    /// Supports DataTable and generic List&lt;T&gt; as data sources.
    /// </summary>
    public partial class SingleSelectionDialog : Form
    {
        #region Fields

        private bool _required = false;
        private ZidThemes _theme = ZidThemes.Default;
        private DataTable _dataSource = null;
        private string _displayMember = null;
        private string _valueMember = null;
        private object _selectedValue = null;

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
        /// Gets or sets whether a selection is required.
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
        /// Gets the selected value (key value).
        /// </summary>
        public object SelectedValue
        {
            get { return _selectedValue; }
        }

        /// <summary>
        /// Gets the selected display text.
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (listBoxItems.SelectedItem != null)
                    return listBoxItems.SelectedItem.ToString();
                return null;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of SingleSelectionDialog.
        /// </summary>
        public SingleSelectionDialog()
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
        /// Loads items from the data source into the list box.
        /// </summary>
        private void LoadItems()
        {
            ApplyFilter();
        }

        /// <summary>
        /// Applies the current filter to the list box.
        /// </summary>
        private void ApplyFilter()
        {
            if (_dataSource == null)
                return;

            listBoxItems.Items.Clear();

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
                listBoxItems.Items.Add(new ListBoxItem(row[_displayMember].ToString(), row[_valueMember]));
            }

            // Update label
            lblItemCount.Text = string.Format("{0} item(s)", listBoxItems.Items.Count);
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
        /// Handles OK button click.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_required && listBoxItems.SelectedItem == null)
            {
                MessageBox.Show("Please select an item.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listBoxItems.SelectedItem != null)
            {
                ListBoxItem item = (ListBoxItem)listBoxItems.SelectedItem;
                _selectedValue = item.Value;
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

        /// <summary>
        /// Handles double-click on list item (same as OK).
        /// </summary>
        private void listBoxItems_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxItems.SelectedItem != null)
            {
                btnOK_Click(sender, e);
            }
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Internal class to hold display text and value for list box items.
        /// </summary>
        private class ListBoxItem
        {
            public string Display { get; set; }
            public object Value { get; set; }

            public ListBoxItem(string display, object value)
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
        /// Shows a single selection dialog with a string list.
        /// </summary>
        public static object ShowDialog(string title, string message, List<string> items,
            ZidThemes theme = ZidThemes.Default, bool required = false,
            Image image = null, IWin32Window owner = null)
        {
            using (SingleSelectionDialog dialog = new SingleSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(items);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValue;

                return null;
            }
        }

        /// <summary>
        /// Shows a single selection dialog with a DataTable.
        /// </summary>
        public static object ShowDialog(string title, string message, DataTable dataTable,
            string displayMember, string valueMember, ZidThemes theme = ZidThemes.Default,
            bool required = false, Image image = null, IWin32Window owner = null)
        {
            using (SingleSelectionDialog dialog = new SingleSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(dataTable, displayMember, valueMember);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValue;

                return null;
            }
        }

        /// <summary>
        /// Shows a single selection dialog with a generic list.
        /// </summary>
        public static object ShowDialog<T>(string title, string message, List<T> items,
            Func<T, string> displaySelector, Func<T, object> valueSelector,
            ZidThemes theme = ZidThemes.Default, bool required = false,
            Image image = null, IWin32Window owner = null)
        {
            using (SingleSelectionDialog dialog = new SingleSelectionDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Theme = theme;
                dialog.Required = required;
                dialog.DialogImage = image;
                dialog.SetDataSource(items, displaySelector, valueSelector);

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.SelectedValue;

                return null;
            }
        }

        #endregion
    }
}
