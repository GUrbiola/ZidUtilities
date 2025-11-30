using System;
using System.Data;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.Grid;
using ZidUtilities.CommonCode.Win.Controls.Grid.Plugins;
using ZidUtilities.CommonCode.Win.Controls.Grid.SamplePlugins;

namespace ZidUtilities.TesterWin
{
    /// <summary>
    /// Test form to demonstrate ZidGrid's header context menu functionality
    /// with both plugins and custom menu items.
    /// </summary>
    public partial class FormZidGridMenuTest : Form
    {
        public FormZidGridMenuTest()
        {
            InitializeComponent();
            SetupGrid();
        }

        private void SetupGrid()
        {
            // Add plugins to the grid
            zidGrid1.Plugins.Add(new DataExportPlugin());
            zidGrid1.Plugins.Add(new ExportToCSVPlugin());

            // Add custom menu items (these would normally be configured in the designer)
            var sortAscMenuItem = new ZidGridMenuItem
            {
                Name = "sortAsc",
                Text = "Sort Ascending",
                Enabled = true
            };
            sortAscMenuItem.Click += CustomMenuItem_SortAscending;
            zidGrid1.CustomMenuItems.Add(sortAscMenuItem);

            var sortDescMenuItem = new ZidGridMenuItem
            {
                Name = "sortDesc",
                Text = "Sort Descending",
                Enabled = true
            };
            sortDescMenuItem.Click += CustomMenuItem_SortDescending;
            zidGrid1.CustomMenuItems.Add(sortDescMenuItem);

            var hideColumnMenuItem = new ZidGridMenuItem
            {
                Name = "hideColumn",
                Text = "Hide Column",
                Enabled = true
            };
            hideColumnMenuItem.Click += CustomMenuItem_HideColumn;
            zidGrid1.CustomMenuItems.Add(hideColumnMenuItem);

            // Create sample data
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Salary", typeof(decimal));
            dt.Columns.Add("HireDate", typeof(DateTime));

            dt.Rows.Add(1, "John Doe", "IT", 75000, new DateTime(2020, 1, 15));
            dt.Rows.Add(2, "Jane Smith", "HR", 65000, new DateTime(2019, 3, 22));
            dt.Rows.Add(3, "Bob Johnson", "IT", 80000, new DateTime(2018, 7, 10));
            dt.Rows.Add(4, "Alice Williams", "Finance", 70000, new DateTime(2021, 5, 5));
            dt.Rows.Add(5, "Charlie Brown", "IT", 72000, new DateTime(2020, 11, 30));

            zidGrid1.DataSource = dt;
            zidGrid1.Theme = GridThemes.Blue;
        }

        private void CustomMenuItem_SortAscending(object sender, ZidGridMenuItemClickEventArgs e)
        {
            if (e.Column != null)
            {
                e.DataGridView.Sort(e.Column, System.ComponentModel.ListSortDirection.Ascending);
            }
        }

        private void CustomMenuItem_SortDescending(object sender, ZidGridMenuItemClickEventArgs e)
        {
            if (e.Column != null)
            {
                e.DataGridView.Sort(e.Column, System.ComponentModel.ListSortDirection.Descending);
            }
        }

        private void CustomMenuItem_HideColumn(object sender, ZidGridMenuItemClickEventArgs e)
        {
            if (e.Column != null)
            {
                e.Column.Visible = false;
                MessageBox.Show($"Column '{e.Column.HeaderText}' has been hidden.", "Hide Column",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
