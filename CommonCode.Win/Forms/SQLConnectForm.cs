using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;


namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// Form that allows the user to browse for SQL Servers, choose authentication,
    /// select a database, and build a connection string.
    /// </summary>
    public partial class SqlConnectForm : Form
    {
        /// <summary>
        /// Builder used to construct and parse SQL connection strings.
        /// </summary>
        SqlConnectionStringBuilder StrBuilder;

        private DialogStyle _style = DialogStyle.Default;

        /// <summary>
        /// Gets the currently selected database name from the connection string builder.
        /// </summary>
        public string SelectedDB { get { return StrBuilder.InitialCatalog; } }

        /// <summary>
        /// Gets or sets the dialog style (color scheme).
        /// </summary>
        public DialogStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string DialogTitle
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>
        /// Gets or sets the banner image displayed at the top of the form.
        /// When set to null, the banner is hidden.
        /// </summary>
        public Image BannerImage
        {
            get { return picBanner.Image; }
            set
            {
                picBanner.Image = value;
                pnlBanner.Visible = (value != null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectForm"/> class.
        /// Sets up the UI components and initializes the internal connection string builder.
        /// </summary>
        public SqlConnectForm()
        {
            InitializeComponent();
            StrBuilder = new SqlConnectionStringBuilder();
            ApplyStyle();
        }

        /// <summary>
        /// Applies the selected style to the dialog.
        /// </summary>
        private void ApplyStyle()
        {
            Color headerColor = DialogStyleHelper.GetHeaderColor(_style);
            Color headerTextColor = DialogStyleHelper.GetHeaderTextColor(_style);
            Color accentColor = DialogStyleHelper.GetAccentColor(_style);

            pnlHeader.BackColor = headerColor;
            lblTitle.ForeColor = headerTextColor;
            Conectar.BackColor = accentColor;
            Conectar.ForeColor = Color.White;
        }

        /// <summary>
        /// Gets or sets whether the user/password controls are active.
        /// When set to false, the username and password textboxes are cleared and disabled.
        /// When set to true, the username and password textboxes are enabled.
        /// </summary>
        private bool Controles_Activos
        {
            get
            {
                return TUser.Enabled;
            }
            set
            {
                if (value)
                {
                    TUser.Enabled = true;
                    TPass.Enabled = true;
                }
                else
                {
                    TUser.Enabled = false;
                    TUser.Text = "";
                    TPass.Enabled = false;
                    TPass.Text = "";
                }
            }
        }

        /// <summary>
        /// Handles changes to the authentication type combo box.
        /// Toggles enabled state of username/password controls according to the selected authentication method.
        /// </summary>
        /// <param name="sender">The combo box that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CAut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CAut.SelectedIndex == 0)
                Controles_Activos = false;
            else
                Controles_Activos = true;
        }

        /// <summary>
        /// Populates the servers combo box when the dropdown is opened.
        /// Uses <see cref="SqlDataSourceEnumerator"/> to discover available SQL Server instances.
        /// </summary>
        /// <param name="sender">The servers combo box.</param>
        /// <param name="e">Event arguments.</param>
        private void CServers_DropDown(object sender, EventArgs e)
        {
            if (CServers.Items.Count > 0)
                return;

            DataTable Aux;
            Aux = SqlDataSourceEnumerator.Instance.GetDataSources();
            for (int i = 0; i < Aux.Rows.Count; i++)
            {
                string txt = Aux.Rows[i][0] + "\\" + Aux.Rows[i][1].ToString();
                CServers.Items.Add(txt);
                CServers.Refresh();
            }
        }

        /// <summary>
        /// Clears the database combo box when the selected server changes.
        /// This forces re-querying the databases for the new server.
        /// </summary>
        /// <param name="sender">The servers combo box.</param>
        /// <param name="e">Event arguments.</param>
        private void CServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            CBDs.Items.Clear();
            CBDs.Text = "";
        }

        /// <summary>
        /// Handler for the form's Shown event.
        /// Ensures a default authentication selection if no connection string is present.
        /// </summary>
        /// <param name="sender">The form that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SQLConnectForm_Shown(object sender, EventArgs e)
        {
            if (StrBuilder == null || StrBuilder.ConnectionString == null || StrBuilder.ConnectionString == "")
            {
                CAut.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Populates the databases combo box when the dropdown is opened.
        /// Builds a temporary connection to the master database using the selected server and authentication,
        /// executes a query to list database names, and fills the combo box with the results.
        /// </summary>
        /// <param name="sender">The databases combo box.</param>
        /// <param name="e">Event arguments.</param>
        private void CBDs_DropDown(object sender, EventArgs e)
        {
            if (CBDs.Items.Count > 0)
                return;
            //create connection string to connect to master database
            string Constr = "";
            if (CAut.SelectedIndex == 0)
            {
                //windows authentication
                Constr = Constr + "Integrated Security=SSPI;";
                Constr = Constr + "Persist Security Info=False;";
                Constr = Constr + "Initial Catalog=master;";
                Constr = Constr + "Data Source=" + CServers.Text;
            }
            else
            {
                Constr = Constr + "Password=" + TPass.Text + ";";
                Constr = Constr + "Persist Security Info=True;";
                Constr = Constr + "User ID=" + TUser.Text + ";";
                Constr = Constr + "Initial Catalog=master;";
                Constr = Constr + "Data Source=" + CServers.Text;
            }
            
            //create query to get database names
            string comando = "select name from sysdatabases order by name";
            
            System.Data.SqlClient.SqlConnection Conexion = null;
            Conexion = new System.Data.SqlClient.SqlConnection();
            Conexion.ConnectionString = Constr;
            //trying to connect
            try
            {
                Conexion.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when trying to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            //Connected, time to execute query.
            System.Data.SqlClient.SqlCommand SqlCommand1;
            SqlCommand1 = new System.Data.SqlClient.SqlCommand();
            SqlCommand1.CommandType = System.Data.CommandType.Text;
            SqlCommand1.Connection = Conexion;
            SqlCommand1.CommandText = comando;
            try
            {
                System.Data.SqlClient.SqlDataReader dr;
                dr = SqlCommand1.ExecuteReader();
                while (dr.Read())
                {
                    CBDs.Items.Add(dr["name"].ToString());
                }
            }
            catch (System.Exception ex)
            {
                //if something dfailed, close connection and show exception
                Conexion.Close();
                MessageBox.Show(ex.Message, "Error when retrieving database names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Conexion.Close();
        }

        /// <summary>
        /// Gets or sets the connection string represented by the form controls.
        /// The getter validates that server and database are selected and returns a constructed connection string.
        /// The setter parses a supplied connection string and sets form controls to reflect it.
        /// </summary>
        /// <remarks>
        /// Getter returns an empty string on validation failure and shows a message box describing the issue.
        /// Setter displays a message box if the supplied connection string cannot be parsed.
        /// </remarks>
        public string ConnectionString
        {
            get
            {
                if (CServers.Text == "")
                {
                    MessageBox.Show("Select/introduce a server name", "Server Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
                if (CBDs.Text == "")
                {
                    MessageBox.Show("Select a database", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }

                StrBuilder = new SqlConnectionStringBuilder();
                StrBuilder.DataSource = CServers.Text;
                if (CAut.SelectedIndex == 0)
                {//Autentificacion de Windows
                    StrBuilder.IntegratedSecurity = true;
                }
                else
                {
                    StrBuilder.IntegratedSecurity = false;
                    StrBuilder.UserID = TUser.Text;
                    StrBuilder.Password = TPass.Text;
                }

                StrBuilder.InitialCatalog = CBDs.Text;
                StrBuilder.PersistSecurityInfo = true;

                return StrBuilder.ConnectionString;
            }
            set
            {
                try
                {
                    StrBuilder = new SqlConnectionStringBuilder(value);
                    CServers.Text = StrBuilder.DataSource;
                    if (StrBuilder.IntegratedSecurity)
                    {//Autentificacion de Windows
                        CAut.SelectedIndex = 0;
                    }
                    else
                    {//Autentificacion de SQL Server
                        CAut.SelectedIndex = 1;
                        TUser.Text = StrBuilder.UserID;
                        TPass.Text = StrBuilder.Password;
                    }
                    CBDs.Text = StrBuilder.InitialCatalog;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// Attempts to open a connection using the connection string built from the form.
        /// If the connection opens successfully, sets the dialog result to OK; otherwise shows an error message.
        /// </summary>
        /// <param name="sender">The connect button that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Connect_Click(object sender, EventArgs e)
        {
            SqlConnection Con;
            if (ConnectionString != "")
                Con = new SqlConnection(ConnectionString);
            else
                return;

            try
            {
                Con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Con.Close();
            DialogResult = DialogResult.OK;
        }

    }
}