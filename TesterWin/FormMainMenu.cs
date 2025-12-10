using System;
using System.Drawing;
using System.Windows.Forms;
using TesterWin;

namespace ZidUtilities.TesterWin
{
    public partial class FormMainMenu : Form
    {
        private Panel header;
        private Panel content;

        public FormMainMenu()
        {
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.content = new System.Windows.Forms.Panel();
            this.header = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // content
            // 
            this.content.AutoScroll = true;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(0, 100);
            this.content.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(582, 668);
            this.content.TabIndex = 0;
            // 
            // header
            // 
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(582, 100);
            this.header.TabIndex = 1;
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 768);
            this.Controls.Add(this.content);
            this.Controls.Add(this.header);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZidUtilities Test Suite";
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.ResumeLayout(false);

        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            BuildMainMenu();
        }

        private void BuildMainMenu()
        {
            this.SuspendLayout();
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header Panel
            header.Height = 100;
            header.BackColor = Color.FromArgb(52, 152, 219);

            Label titleLabel = new Label
            {
                Text = "ZidUtilities Test Suite",
                Font = new Font("Verdana", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label subtitleLabel = new Label
            {
                Text = "Select a test form to open",
                Font = new Font("Verdana", 10F),
                ForeColor = Color.White,
                Location = new Point(20, 55),
                AutoSize = true
            };

            header.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });

            // Main content panel

            int yPos = 10;

            // Add test form buttons
            AddTestButton(content, "UIGenerator Test (CRUD)", "Comprehensive test of UIGenerator for dynamic CRUD dialogs",
                ref yPos, () => OpenForm(new FormUIGeneratorTest()));

            AddTestButton(content, "Theme Manager Test", "Test the ThemeManager component with various controls",
                ref yPos, () => OpenForm(new FormThemeManagerTest()));

            AddTestButton(content, "ZidGrid Test (Form 9)", "Test the ZidGrid with themes and plugins",
                ref yPos, () => OpenForm(new Form9()));

            AddTestButton(content, "ZidGrid Menu Test", "Test ZidGrid context menu and plugin system",
                ref yPos, () => OpenForm(new FormZidGridMenuTest()));

            AddTestButton(content, "Form 1 (ICSharpTextEditor)", "Test ICSharpTextEditor.ExtendedEditor control",
                ref yPos, () => OpenForm(new Form1()));

            AddTestButton(content, "AvalonEdit Test", "Test AvalonEdit.ExtendedEditor control",
                ref yPos, () => OpenForm(new FormAvalonEditTest()));

            AddTestButton(content, "Form 2", "Original test form 2",
                ref yPos, () => OpenForm(new Form2()));

            AddTestButton(content, "Form 3", "Original test form 3",
                ref yPos, () => OpenForm(new Form3()));

            AddTestButton(content, "Form 4", "Original test form 4",
                ref yPos, () => OpenForm(new Form4()));

            AddTestButton(content, "Form 5", "Original test form 5",
                ref yPos, () => OpenForm(new Form5()));

            AddTestButton(content, "Form 6", "Original test form 6",
                ref yPos, () => OpenForm(new Form6()));

            AddTestButton(content, "Form 7", "Original test form 7",
                ref yPos, () => OpenForm(new Form7()));

            AddTestButton(content, "Form 8", "Original test form 8",
                ref yPos, () => OpenForm(new Form8()));

            this.ResumeLayout(false);
        }

        private void AddTestButton(Panel parent, string title, string description, ref int yPos, Action onClick)
        {
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(580, 70),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Verdana", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(15, 10),
                AutoSize = true
            };

            Label descLabel = new Label
            {
                Text = description,
                Font = new Font("Verdana", 9F),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(15, 35),
                Size = new Size(550, 25)
            };

            buttonPanel.Controls.AddRange(new Control[] { titleLabel, descLabel });

            // Add hover effect
            buttonPanel.MouseEnter += (s, e) =>
            {
                buttonPanel.BackColor = Color.FromArgb(236, 240, 241);
            };

            buttonPanel.MouseLeave += (s, e) =>
            {
                buttonPanel.BackColor = Color.White;
            };

            buttonPanel.Click += (s, e) => onClick();
            titleLabel.Click += (s, e) => onClick();
            descLabel.Click += (s, e) => onClick();

            parent.Controls.Add(buttonPanel);

            yPos += 80;
        }

        private void OpenForm(Form form)
        {
            try
            {
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
