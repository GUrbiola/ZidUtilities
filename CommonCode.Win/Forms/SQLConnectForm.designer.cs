namespace ZidUtilities.CommonCode.Win.Forms
{
    partial class SqlConnectForm
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.picBanner = new System.Windows.Forms.PictureBox();
            this.panelEx1 = new System.Windows.Forms.Panel();
            this.CBDs = new System.Windows.Forms.ComboBox();
            this.CAut = new System.Windows.Forms.ComboBox();
            this.CServers = new System.Windows.Forms.ComboBox();
            this.TPass = new System.Windows.Forms.TextBox();
            this.TUser = new System.Windows.Forms.TextBox();
            this.labelX5 = new System.Windows.Forms.Label();
            this.labelX4 = new System.Windows.Forms.Label();
            this.labelX3 = new System.Windows.Forms.Label();
            this.labelX2 = new System.Windows.Forms.Label();
            this.labelX1 = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.Cancelar = new System.Windows.Forms.Button();
            this.Conectar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).BeginInit();
            this.panelEx1.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlHeader.Size = new System.Drawing.Size(494, 50);
            this.pnlHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(464, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Connect to Microsoft SQL Server";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlBanner
            //
            this.pnlBanner.Controls.Add(this.picBanner);
            this.pnlBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBanner.Location = new System.Drawing.Point(0, 50);
            this.pnlBanner.Name = "pnlBanner";
            this.pnlBanner.Size = new System.Drawing.Size(494, 100);
            this.pnlBanner.TabIndex = 1;
            this.pnlBanner.Visible = false;
            //
            // picBanner
            //
            this.picBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBanner.Location = new System.Drawing.Point(0, 0);
            this.picBanner.Name = "picBanner";
            this.picBanner.Size = new System.Drawing.Size(494, 100);
            this.picBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBanner.TabIndex = 0;
            this.picBanner.TabStop = false;
            //
            // panelEx1
            //
            this.panelEx1.BackColor = System.Drawing.Color.White;
            this.panelEx1.Controls.Add(this.CBDs);
            this.panelEx1.Controls.Add(this.CAut);
            this.panelEx1.Controls.Add(this.CServers);
            this.panelEx1.Controls.Add(this.TPass);
            this.panelEx1.Controls.Add(this.TUser);
            this.panelEx1.Controls.Add(this.labelX5);
            this.panelEx1.Controls.Add(this.labelX4);
            this.panelEx1.Controls.Add(this.labelX3);
            this.panelEx1.Controls.Add(this.labelX2);
            this.panelEx1.Controls.Add(this.labelX1);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 50);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Padding = new System.Windows.Forms.Padding(20);
            this.panelEx1.Size = new System.Drawing.Size(494, 191);
            this.panelEx1.TabIndex = 2;
            //
            // CBDs
            //
            this.CBDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CBDs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBDs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBDs.FormattingEnabled = true;
            this.CBDs.Location = new System.Drawing.Point(130, 157);
            this.CBDs.Name = "CBDs";
            this.CBDs.Size = new System.Drawing.Size(344, 23);
            this.CBDs.TabIndex = 13;
            this.CBDs.DropDown += new System.EventHandler(this.CBDs_DropDown);
            //
            // CAut
            //
            this.CAut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CAut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CAut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAut.FormattingEnabled = true;
            this.CAut.Items.AddRange(new object[] {
            "Windows authentication",
            "SQL server authentication"});
            this.CAut.Location = new System.Drawing.Point(130, 52);
            this.CAut.Name = "CAut";
            this.CAut.Size = new System.Drawing.Size(344, 23);
            this.CAut.TabIndex = 12;
            this.CAut.SelectedIndexChanged += new System.EventHandler(this.CAut_SelectedIndexChanged);
            //
            // CServers
            //
            this.CServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CServers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CServers.FormattingEnabled = true;
            this.CServers.Location = new System.Drawing.Point(130, 17);
            this.CServers.Name = "CServers";
            this.CServers.Size = new System.Drawing.Size(344, 23);
            this.CServers.TabIndex = 11;
            this.CServers.DropDown += new System.EventHandler(this.CServers_DropDown);
            this.CServers.SelectedIndexChanged += new System.EventHandler(this.CServers_SelectedIndexChanged);
            //
            // TPass
            //
            this.TPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TPass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TPass.Location = new System.Drawing.Point(130, 122);
            this.TPass.Name = "TPass";
            this.TPass.PasswordChar = '*';
            this.TPass.Size = new System.Drawing.Size(344, 23);
            this.TPass.TabIndex = 9;
            //
            // TUser
            //
            this.TUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TUser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TUser.Location = new System.Drawing.Point(130, 87);
            this.TUser.Name = "TUser";
            this.TUser.Size = new System.Drawing.Size(344, 23);
            this.TUser.TabIndex = 8;
            //
            // labelX5
            //
            this.labelX5.AutoSize = true;
            this.labelX5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX5.Location = new System.Drawing.Point(20, 160);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(58, 15);
            this.labelX5.TabIndex = 5;
            this.labelX5.Text = "Database:";
            //
            // labelX4
            //
            this.labelX4.AutoSize = true;
            this.labelX4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX4.Location = new System.Drawing.Point(20, 125);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(60, 15);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "Password:";
            //
            // labelX3
            //
            this.labelX3.AutoSize = true;
            this.labelX3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX3.Location = new System.Drawing.Point(20, 90);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(33, 15);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "User:";
            //
            // labelX2
            //
            this.labelX2.AutoSize = true;
            this.labelX2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX2.Location = new System.Drawing.Point(20, 55);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(90, 15);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "Authentication:";
            //
            // labelX1
            //
            this.labelX1.AutoSize = true;
            this.labelX1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX1.Location = new System.Drawing.Point(20, 20);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(79, 15);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "Server Name:";
            //
            // pnlButtons
            //
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlButtons.Controls.Add(this.Cancelar);
            this.pnlButtons.Controls.Add(this.Conectar);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 241);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlButtons.Size = new System.Drawing.Size(494, 50);
            this.pnlButtons.TabIndex = 3;
            //
            // Cancelar
            //
            this.Cancelar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Cancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.Cancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Cancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelar.FlatAppearance.BorderSize = 0;
            this.Cancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancelar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancelar.ForeColor = System.Drawing.Color.White;
            this.Cancelar.Location = new System.Drawing.Point(384, 10);
            this.Cancelar.Name = "Cancelar";
            this.Cancelar.Size = new System.Drawing.Size(95, 30);
            this.Cancelar.TabIndex = 1;
            this.Cancelar.Text = "Cancel";
            this.Cancelar.UseVisualStyleBackColor = false;
            //
            // Conectar
            //
            this.Conectar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Conectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Conectar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.Conectar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Conectar.FlatAppearance.BorderSize = 0;
            this.Conectar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Conectar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Conectar.ForeColor = System.Drawing.Color.White;
            this.Conectar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Conectar.Location = new System.Drawing.Point(283, 10);
            this.Conectar.Name = "Conectar";
            this.Conectar.Size = new System.Drawing.Size(95, 30);
            this.Conectar.TabIndex = 0;
            this.Conectar.Text = "Connect";
            this.Conectar.UseVisualStyleBackColor = false;
            this.Conectar.Click += new System.EventHandler(this.Connect_Click);
            //
            // SqlConnectForm
            //
            this.AcceptButton = this.Conectar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancelar;
            this.ClientSize = new System.Drawing.Size(494, 291);
            this.ControlBox = false;
            this.Controls.Add(this.panelEx1);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SqlConnectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQL Server Connection";
            this.Shown += new System.EventHandler(this.SQLConnectForm_Shown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlBanner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).EndInit();
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.PictureBox picBanner;
        private System.Windows.Forms.Panel panelEx1;
        private System.Windows.Forms.Label labelX5;
        private System.Windows.Forms.Label labelX4;
        private System.Windows.Forms.Label labelX3;
        private System.Windows.Forms.Label labelX2;
        private System.Windows.Forms.Label labelX1;
        private System.Windows.Forms.TextBox TPass;
        private System.Windows.Forms.TextBox TUser;
        private System.Windows.Forms.Button Conectar;
        private System.Windows.Forms.Button Cancelar;
        private System.Windows.Forms.ComboBox CBDs;
        private System.Windows.Forms.ComboBox CAut;
        private System.Windows.Forms.ComboBox CServers;
        private System.Windows.Forms.Panel pnlButtons;
    }
}
