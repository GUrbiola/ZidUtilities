using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.DataAccess;
using ZidUtilities.CommonCode.Win;
using ZidUtilities.CommonCode.Win.Controls.Grid;

namespace ZidUtilities.TesterWin
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
            foreach (var item in Enum.GetValues(typeof(ZidThemes)))
                cmbThemes.Items.Add(new ComboBoxItem() { Text = item.ToString(), Theme = (ZidThemes)item });


            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.DataExportPlugin());
            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.ColumnVisibilityPlugin());
            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.CopySpecialPlugin());
            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.FreezeColumnsPlugin());
            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.QuickFilterPlugin());

            foreach (var option in zidGrid1.GetDefaultMenuOptions())
                zidGrid1.CustomMenuItems.Add(option);

            cmbThemes.SelectedIndex = 0;
            cmbThemes.SelectedValueChanged += new EventHandler(cmbThemes_SelectedValueChanged);

            GridThemeHelper.ApplyTheme(zidGrid1, ZidThemes.Professional);

            
        }

        private void cmbThemes_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedTheme = cmbThemes.SelectedItem as ComboBoxItem;
            if (selectedTheme != null)
                zidGrid1.Theme = selectedTheme.Theme;
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            zidGrid1.EnableAlternatingRows = checkBox1.Checked;
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            //SqlConnector conx = new SqlConnector("Data Source=.\\SQLSERVER;Initial Catalog=CompensationDB;Integrated Security=True");
            //SqlResponse<DataTable> data = conx.ExecuteTable("SELECT * FROM dbo.Employee");

            SqlConnector conx = new SqlConnector("Data Source=Main\\SQLSERVER;Initial Catalog=Northwind;Integrated Security=False;Persist Security Info=True;User ID=SqlAdmin;Password=SqlAdmin123");
            SqlResponse<DataTable> data = conx.ExecuteTable("SELECT * FROM dbo.Employees");

            //SqlConnector conx = new SqlConnector("Data Source=.\\SQLSERVER;Initial Catalog=SupplierPortal;Integrated Security=True");
            //SqlResponse<DataTable> data = conx.ExecuteTable("SELECT * FROM dbo.AppUsers");

            if (data.IsOK)
                zidGrid1.DataSource = (DataTable)data.Result;
            else
                ZidUtilities.CommonCode.Win.Forms.MessageBoxDialog.Show("An error occured while loading the data from the Data base", "Loading Data on Grid", MessageBoxButtons.OK, MessageBoxIcon.Error, ZidThemes.Error);
        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public ZidThemes Theme { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
