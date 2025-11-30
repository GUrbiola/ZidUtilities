namespace TesterWin
{
    partial class Form5
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.addressBar1 = new ZidUtilities.CommonCode.Win.Controls.AddressBar.AddressBar();
            this.rootNode = new ZidUtilities.CommonCode.Win.Controls.AddressBar.GenericNode(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAddNodeToCurrentSelection = new System.Windows.Forms.Button();
            this.btnSwitchToProgrammaticTree = new System.Windows.Forms.Button();
            this.btnSwitchToDesignTimeTree = new System.Windows.Forms.Button();
            this.lblSelectedNode = new System.Windows.Forms.Label();
            this.lblTreeType = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addressBar1
            // 
            this.addressBar1.BackColor = System.Drawing.Color.White;
            this.addressBar1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.addressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressBar1.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressBar1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.addressBar1.Location = new System.Drawing.Point(0, 0);
            this.addressBar1.MinimumSize = new System.Drawing.Size(331, 23);
            this.addressBar1.Name = "addressBar1";
            this.addressBar1.SelectedStyle = ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline)));
            this.addressBar1.Size = new System.Drawing.Size(800, 40);
            this.addressBar1.TabIndex = 0;
            this.addressBar1.SelectionChange += new ZidUtilities.CommonCode.Win.Controls.AddressBar.AddressBar.SelectionChanged(this.addressBar1_SelectionChange);
            // 
            // rootNode
            // 
            this.rootNode.DisplayName = "Root";
            this.rootNode.Parent = null;
            this.rootNode.UniqueID = "Root";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAddNodeToCurrentSelection);
            this.panel1.Controls.Add(this.btnSwitchToProgrammaticTree);
            this.panel1.Controls.Add(this.btnSwitchToDesignTimeTree);
            this.panel1.Controls.Add(this.lblSelectedNode);
            this.panel1.Controls.Add(this.lblTreeType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 40);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(800, 410);
            this.panel1.TabIndex = 1;
            // 
            // btnAddNodeToCurrentSelection
            // 
            this.btnAddNodeToCurrentSelection.Location = new System.Drawing.Point(13, 120);
            this.btnAddNodeToCurrentSelection.Name = "btnAddNodeToCurrentSelection";
            this.btnAddNodeToCurrentSelection.Size = new System.Drawing.Size(250, 30);
            this.btnAddNodeToCurrentSelection.TabIndex = 4;
            this.btnAddNodeToCurrentSelection.Text = "Add Child to Current Selection";
            this.btnAddNodeToCurrentSelection.UseVisualStyleBackColor = true;
            this.btnAddNodeToCurrentSelection.Click += new System.EventHandler(this.btnAddNodeToCurrentSelection_Click);
            // 
            // btnSwitchToProgrammaticTree
            // 
            this.btnSwitchToProgrammaticTree.Location = new System.Drawing.Point(269, 84);
            this.btnSwitchToProgrammaticTree.Name = "btnSwitchToProgrammaticTree";
            this.btnSwitchToProgrammaticTree.Size = new System.Drawing.Size(250, 30);
            this.btnSwitchToProgrammaticTree.TabIndex = 3;
            this.btnSwitchToProgrammaticTree.Text = "Switch to Programmatic Tree";
            this.btnSwitchToProgrammaticTree.UseVisualStyleBackColor = true;
            this.btnSwitchToProgrammaticTree.Click += new System.EventHandler(this.btnSwitchToProgrammaticTree_Click);
            // 
            // btnSwitchToDesignTimeTree
            // 
            this.btnSwitchToDesignTimeTree.Location = new System.Drawing.Point(13, 84);
            this.btnSwitchToDesignTimeTree.Name = "btnSwitchToDesignTimeTree";
            this.btnSwitchToDesignTimeTree.Size = new System.Drawing.Size(250, 30);
            this.btnSwitchToDesignTimeTree.TabIndex = 2;
            this.btnSwitchToDesignTimeTree.Text = "Switch to Design-Time Tree";
            this.btnSwitchToDesignTimeTree.UseVisualStyleBackColor = true;
            this.btnSwitchToDesignTimeTree.Click += new System.EventHandler(this.btnSwitchToDesignTimeTree_Click);
            // 
            // lblSelectedNode
            // 
            this.lblSelectedNode.AutoSize = true;
            this.lblSelectedNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedNode.Location = new System.Drawing.Point(13, 40);
            this.lblSelectedNode.Name = "lblSelectedNode";
            this.lblSelectedNode.Size = new System.Drawing.Size(113, 17);
            this.lblSelectedNode.TabIndex = 1;
            this.lblSelectedNode.Text = "Selected: (none)";
            // 
            // lblTreeType
            // 
            this.lblTreeType.AutoSize = true;
            this.lblTreeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTreeType.Location = new System.Drawing.Point(13, 10);
            this.lblTreeType.Name = "lblTreeType";
            this.lblTreeType.Size = new System.Drawing.Size(420, 20);
            this.lblTreeType.TabIndex = 0;
            this.lblTreeType.Text = "Current Tree: Design-Time Configured (with events)";
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addressBar1);
            this.Name = "Form5";
            this.Text = "GenericNode Demo - AddressBar with Event-Driven Navigation";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ZidUtilities.CommonCode.Win.Controls.AddressBar.AddressBar addressBar1;
        private ZidUtilities.CommonCode.Win.Controls.AddressBar.GenericNode rootNode;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTreeType;
        private System.Windows.Forms.Label lblSelectedNode;
        private System.Windows.Forms.Button btnSwitchToDesignTimeTree;
        private System.Windows.Forms.Button btnSwitchToProgrammaticTree;
        private System.Windows.Forms.Button btnAddNodeToCurrentSelection;
    }
}
