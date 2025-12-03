using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win;
using ZidUtilities.CommonCode.Win.Controls;

namespace ZidUtilities.TesterWin
{
    public partial class FormThemeManagerTest : Form
    {
        private ThemeManager themeManager;

        public FormThemeManagerTest()
        {
            InitializeComponent();

            // Initialize theme manager
            themeManager = new ThemeManager();
            themeManager.ParentForm = this;
        }

        private void FormThemeManagerTest_Load(object sender, EventArgs e)
        {
            BuildTestForm();
        }

        private void BuildTestForm()
        {
            this.SuspendLayout();

            // MenuStrip
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("&New", null, (s, ev) => MessageBox.Show("New clicked"));
            fileMenu.DropDownItems.Add("&Open", null, (s, ev) => MessageBox.Show("Open clicked"));
            fileMenu.DropDownItems.Add("-");
            fileMenu.DropDownItems.Add("E&xit", null, (s, ev) => this.Close());

            ToolStripMenuItem editMenu = new ToolStripMenuItem("&Edit");
            editMenu.DropDownItems.Add("&Copy", null, (s, ev) => MessageBox.Show("Copy clicked"));
            editMenu.DropDownItems.Add("&Paste", null, (s, ev) => MessageBox.Show("Paste clicked"));

            ToolStripMenuItem viewMenu = new ToolStripMenuItem("&View");
            viewMenu.DropDownItems.Add("&Refresh", null, (s, ev) => MessageBox.Show("Refresh clicked"));

            ToolStripMenuItem helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("&About", null, (s, ev) => MessageBox.Show("Theme Manager Test v1.0", "About"));

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, viewMenu, helpMenu });
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            // ToolStrip
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Items.Add(new ToolStripButton("New", null, (s, ev) => MessageBox.Show("New clicked")));
            toolStrip.Items.Add(new ToolStripButton("Open", null, (s, ev) => MessageBox.Show("Open clicked")));
            toolStrip.Items.Add(new ToolStripButton("Save", null, (s, ev) => MessageBox.Show("Save clicked")));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Copy", null, (s, ev) => MessageBox.Show("Copy clicked")));
            toolStrip.Items.Add(new ToolStripButton("Paste", null, (s, ev) => MessageBox.Show("Paste clicked")));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Zoom:"));
            ToolStripComboBox zoomCombo = new ToolStripComboBox();
            zoomCombo.Items.AddRange(new object[] { "50%", "75%", "100%", "125%", "150%" });
            zoomCombo.SelectedIndex = 2;
            toolStrip.Items.Add(zoomCombo);
            this.Controls.Add(toolStrip);

            // StatusStrip
            StatusStrip statusStrip = new StatusStrip();
            statusStrip.Items.Add(new ToolStripStatusLabel("Ready"));
            statusStrip.Items.Add(new ToolStripStatusLabel("Status: OK") { Spring = true, TextAlign = ContentAlignment.MiddleLeft });
            statusStrip.Items.Add(new ToolStripStatusLabel("Line: 1, Col: 1"));
            statusStrip.Items.Add(new ToolStripProgressBar() { Value = 50, Width = 100 });
            this.Controls.Add(statusStrip);

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60
            };

            Label headerLabel = new Label
            {
                Text = "Theme Manager Demonstration",
                Font = new Font("Verdana", 14F, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Add ContextMenuStrip to header label
            ContextMenuStrip headerContextMenu = new ContextMenuStrip();
            headerContextMenu.Items.Add("Copy Title", null, (s, ev) => MessageBox.Show("Title copied!"));
            headerContextMenu.Items.Add("Change Title", null, (s, ev) => headerLabel.Text = "New Title");
            headerContextMenu.Items.Add("-");
            headerContextMenu.Items.Add("Reset Title", null, (s, ev) => headerLabel.Text = "Theme Manager Demonstration");
            headerLabel.ContextMenuStrip = headerContextMenu;

            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // Theme Selection Panel
            Panel themeSelectionPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(860, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label themeLabel = new Label
            {
                Text = "Select Theme:",
                Font = new Font("Verdana", 10F, FontStyle.Bold),
                Location = new Point(10, 15),
                AutoSize = true
            };

            ComboBox cmbTheme = new ComboBox
            {
                Location = new Point(130, 12),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Populate themes
            List<ZidThemes> themesList = new List<ZidThemes>();
            foreach (ZidThemes theme in Enum.GetValues(typeof(ZidThemes)))
                themesList.Add(theme);
            themesList = themesList.OrderBy(t => t.ToString()).ToList();
            for (int i = 0; i < themesList.Count; i++)
                cmbTheme.Items.Add(themesList[i]);

            cmbTheme.SelectedValue = ZidThemes.None;
            cmbTheme.SelectedIndexChanged += (s, ev) =>
            {
                themeManager.Theme = (ZidThemes)cmbTheme.SelectedItem;
                themeManager.ApplyTheme();
            };

            Button btnApply = new Button
            {
                Text = "Apply Theme",
                Location = new Point(350, 10),
                Size = new Size(120, 30)
            };
            btnApply.Click += (s, ev) =>
            {
                themeManager.ApplyTheme();
                MessageBox.Show("Theme applied successfully!", "Theme Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            themeSelectionPanel.Controls.AddRange(new Control[] { themeLabel, cmbTheme, btnApply });
            this.Controls.Add(themeSelectionPanel);

            // Sample Controls Panel - make it larger to accommodate more controls
            Panel samplesPanel = new Panel
            {
                Location = new Point(20, 200),
                Size = new Size(860, 420),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // GroupBox with various controls
            GroupBox groupBox1 = new GroupBox
            {
                Text = "Text Controls",
                Location = new Point(10, 10),
                Size = new Size(400, 180)
            };

            Label label1 = new Label
            {
                Text = "Name:",
                Location = new Point(10, 30),
                AutoSize = true
            };

            TextBox textBox1 = new TextBox
            {
                Location = new Point(80, 27),
                Size = new Size(300, 25),
                Text = "Sample Text"
            };

            // Add ContextMenuStrip to textbox
            ContextMenuStrip textBoxContextMenu = new ContextMenuStrip();
            textBoxContextMenu.Items.Add("Cut", null, (s, ev) => { if (textBox1.SelectionLength > 0) textBox1.Cut(); });
            textBoxContextMenu.Items.Add("Copy", null, (s, ev) => { if (textBox1.SelectionLength > 0) textBox1.Copy(); });
            textBoxContextMenu.Items.Add("Paste", null, (s, ev) => textBox1.Paste());
            textBoxContextMenu.Items.Add("-");
            textBoxContextMenu.Items.Add("Select All", null, (s, ev) => textBox1.SelectAll());
            textBoxContextMenu.Items.Add("Clear", null, (s, ev) => textBox1.Clear());
            textBox1.ContextMenuStrip = textBoxContextMenu;

            Label label2 = new Label
            {
                Text = "Email:",
                Location = new Point(10, 65),
                AutoSize = true
            };

            TextBox textBox2 = new TextBox
            {
                Location = new Point(80, 62),
                Size = new Size(300, 25),
                Text = "sample@email.com"
            };

            CheckBox checkBox1 = new CheckBox
            {
                Text = "Subscribe to newsletter",
                Location = new Point(10, 100),
                AutoSize = true,
                Checked = true
            };

            RadioButton radio1 = new RadioButton
            {
                Text = "Option 1",
                Location = new Point(10, 130),
                AutoSize = true,
                Checked = true
            };

            RadioButton radio2 = new RadioButton
            {
                Text = "Option 2",
                Location = new Point(100, 130),
                AutoSize = true
            };

            groupBox1.Controls.AddRange(new Control[] { label1, textBox1, label2, textBox2, checkBox1, radio1, radio2 });

            // GroupBox with buttons
            GroupBox groupBox2 = new GroupBox
            {
                Text = "Buttons",
                Location = new Point(430, 10),
                Size = new Size(410, 180)
            };

            Button button1 = new Button
            {
                Text = "Primary Action",
                Location = new Point(10, 30),
                Size = new Size(120, 35)
            };

            Button button2 = new Button
            {
                Text = "Secondary",
                Location = new Point(140, 30),
                Size = new Size(120, 35)
            };

            Button button3 = new Button
            {
                Text = "Tertiary",
                Location = new Point(270, 30),
                Size = new Size(120, 35)
            };

            ComboBox combo1 = new ComboBox
            {
                Location = new Point(10, 80),
                Size = new Size(200, 25)
            };
            combo1.Items.AddRange(new object[] { "Item 1", "Item 2", "Item 3", "Item 4" });
            combo1.SelectedIndex = 0;

            ListBox listBox1 = new ListBox
            {
                Location = new Point(10, 115),
                Size = new Size(200, 55)
            };
            listBox1.Items.AddRange(new object[] { "List Item 1", "List Item 2", "List Item 3" });

            groupBox2.Controls.AddRange(new Control[] { button1, button2, button3, combo1, listBox1 });

            // TabControl
            TabControl tabControl = new TabControl
            {
                Location = new Point(10, 200),
                Size = new Size(830, 200)
            };

            TabPage tab1 = new TabPage("Tab 1");
            Label tabLabel1 = new Label
            {
                Text = "This is content in Tab 1",
                Location = new Point(20, 20),
                AutoSize = true
            };
            tab1.Controls.Add(tabLabel1);

            TabPage tab2 = new TabPage("Tab 2");
            Label tabLabel2 = new Label
            {
                Text = "This is content in Tab 2",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Button tabButton = new Button
            {
                Text = "Tab Button",
                Location = new Point(20, 50),
                Size = new Size(100, 30)
            };
            tab2.Controls.AddRange(new Control[] { tabLabel2, tabButton });

            tabControl.TabPages.AddRange(new TabPage[] { tab1, tab2 });

            // GroupBox with additional controls
            GroupBox groupBox3 = new GroupBox
            {
                Text = "Additional Controls",
                Location = new Point(10, 410),
                Size = new Size(400, 260)
            };

            Label label3 = new Label
            {
                Text = "NumericUpDown:",
                Location = new Point(10, 25),
                AutoSize = true
            };

            NumericUpDown numericUpDown1 = new NumericUpDown
            {
                Location = new Point(130, 22),
                Size = new Size(100, 25),
                Value = 50,
                Minimum = 0,
                Maximum = 100
            };

            Label label4 = new Label
            {
                Text = "TrackBar:",
                Location = new Point(10, 60),
                AutoSize = true
            };

            TrackBar trackBar1 = new TrackBar
            {
                Location = new Point(130, 55),
                Size = new Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                Value = 50
            };

            Label label5 = new Label
            {
                Text = "ProgressBar:",
                Location = new Point(10, 105),
                AutoSize = true
            };

            ProgressBar progressBar1 = new ProgressBar
            {
                Location = new Point(130, 102),
                Size = new Size(200, 25),
                Value = 60
            };

            Label label6 = new Label
            {
                Text = "DateTimePicker:",
                Location = new Point(10, 140),
                AutoSize = true
            };

            DateTimePicker dateTimePicker1 = new DateTimePicker
            {
                Location = new Point(130, 137),
                Size = new Size(200, 25)
            };

            Label label7 = new Label
            {
                Text = "MaskedTextBox:",
                Location = new Point(10, 175),
                AutoSize = true
            };

            MaskedTextBox maskedTextBox1 = new MaskedTextBox
            {
                Location = new Point(130, 172),
                Size = new Size(200, 25),
                Mask = "00/00/0000",
                Text = "12/31/2024"
            };

            CheckedListBox checkedListBox1 = new CheckedListBox
            {
                Location = new Point(130, 205),
                Size = new Size(200, 45)
            };
            checkedListBox1.Items.AddRange(new object[] { "Option A", "Option B", "Option C" });
            checkedListBox1.SetItemChecked(0, true);

            groupBox3.Controls.AddRange(new Control[] { label3, numericUpDown1, label4, trackBar1,
                label5, progressBar1, label6, dateTimePicker1, label7, maskedTextBox1, checkedListBox1 });

            // GroupBox with TreeView and ListView
            GroupBox groupBox4 = new GroupBox
            {
                Text = "Tree & List Views",
                Location = new Point(430, 410),
                Size = new Size(410, 260)
            };

            TreeView treeView1 = new TreeView
            {
                Location = new Point(10, 25),
                Size = new Size(180, 220)
            };
            TreeNode rootNode = new TreeNode("Root");
            rootNode.Nodes.Add("Child 1");
            rootNode.Nodes.Add("Child 2");
            TreeNode subNode = rootNode.Nodes.Add("Child 3");
            subNode.Nodes.Add("Subchild 1");
            treeView1.Nodes.Add(rootNode);
            treeView1.ExpandAll();

            ListView listView1 = new ListView
            {
                Location = new Point(200, 25),
                Size = new Size(200, 220),
                View = View.Details
            };
            listView1.Columns.Add("Name", 100);
            listView1.Columns.Add("Value", 95);
            listView1.Items.Add(new ListViewItem(new[] { "Item 1", "100" }));
            listView1.Items.Add(new ListViewItem(new[] { "Item 2", "200" }));
            listView1.Items.Add(new ListViewItem(new[] { "Item 3", "300" }));

            // Add ContextMenuStrip to ListView
            ContextMenuStrip listViewContextMenu = new ContextMenuStrip();
            listViewContextMenu.Items.Add("Add Item", null, (s, ev) =>
            {
                int newIndex = listView1.Items.Count + 1;
                listView1.Items.Add(new ListViewItem(new[] { $"Item {newIndex}", $"{newIndex * 100}" }));
            });
            listViewContextMenu.Items.Add("Remove Selected", null, (s, ev) =>
            {
                if (listView1.SelectedItems.Count > 0)
                    listView1.Items.Remove(listView1.SelectedItems[0]);
            });
            listViewContextMenu.Items.Add("-");
            listViewContextMenu.Items.Add("Clear All", null, (s, ev) => listView1.Items.Clear());
            listView1.ContextMenuStrip = listViewContextMenu;

            groupBox4.Controls.AddRange(new Control[] { treeView1, listView1 });

            // RichTextBox
            GroupBox groupBox5 = new GroupBox
            {
                Text = "Rich Text Editor",
                Location = new Point(10, 680),
                Size = new Size(830, 120)
            };

            RichTextBox richTextBox1 = new RichTextBox
            {
                Location = new Point(10, 25),
                Size = new Size(810, 85),
                Text = "This is a RichTextBox control that supports rich text formatting.\nYou can have multiple lines and styles."
            };

            groupBox5.Controls.Add(richTextBox1);

            samplesPanel.Controls.AddRange(new Control[] { groupBox1, groupBox2, tabControl,
                groupBox3, groupBox4, groupBox5 });
            this.Controls.Add(samplesPanel);
            themeSelectionPanel.Dock = DockStyle.Top;
            samplesPanel.Dock = DockStyle.Fill;
            samplesPanel.BringToFront();

            this.ResumeLayout(false);
        }
    }
}
