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
using ZidUtilities.CommonCode.Win.Controls.Grid;

namespace ZidUtilities.TesterWin
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
            foreach (var item in Enum.GetValues(typeof(GridThemes)))
                cmbThemes.Items.Add(new ComboBoxItem() { Text = item.ToString(), Theme = (GridThemes)item });


            //cmbThemes.Items.Add(new ComboBoxItem() { Text = "Black and White", Theme = GridThemes.BlackAndWhite});
            //cmbThemes.Items.Add(new ComboBoxItem() { Text = "Blue", Theme = GridThemes.Blue });
            //cmbThemes.Items.Add(new ComboBoxItem() { Text = "Code Project Style", Theme = GridThemes.CodeProject });
            //cmbThemes.Items.Add(new ComboBoxItem() { Text = "Kind of green... maybe?", Theme = GridThemes.Greenish });
            //cmbThemes.Items.Add(new ComboBoxItem() { Text = "Violet... manly violet -_-", Theme = GridThemes.Violet });

            zidGrid1.Plugins.Add(new ZidUtilities.CommonCode.Win.Controls.Grid.Plugins.DataExportPlugin());

            cmbThemes.SelectedIndex = 0;
            cmbThemes.SelectedValueChanged += new EventHandler(cmbThemes_SelectedValueChanged);
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
            SqlConnector conx = new SqlConnector("Data Source=.\\SQLSERVER;Initial Catalog=CompensationDB;Integrated Security=True");
            zidGrid1.DataSource = conx.ExecuteTable("SELECT * FROM dbo.Employee");

            //SqlConnector conx = new SqlConnector("Data Source=.\\SQLSERVER;Initial Catalog=SupplierPortal;Integrated Security=True");
            //zidGrid1.DataSource = conx.ExecuteTable("SELECT * FROM dbo.AppUsers");


        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public GridThemes Theme { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
