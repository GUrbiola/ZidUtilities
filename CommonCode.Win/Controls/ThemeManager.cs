using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.Grid;

namespace ZidUtilities.CommonCode.Win.Controls
{
    [ToolboxBitmap(@"D:\Just For Fun\ZidUtilities\CommonCode.Win\Controls\ThemeManager.ico")]
    /// <summary>
    /// Component that manages and applies themes to Windows Forms and their controls.
    /// </summary>
    public class ThemeManager : Component
    {
        private ZidThemes _theme = ZidThemes.None;
        private Form _parentForm = null;
        private GridThemeHelper.ThemeColors _themeColors;

        /// <summary>
        /// Gets or sets the theme to apply to the form and controls.
        /// </summary>
        [Category("Appearance")]
        [Description("The theme to apply to the form and its controls.")]
        [DefaultValue(ZidThemes.None)]
        public ZidThemes Theme
        {
            get { return _theme; }
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    _themeColors = GridThemeHelper.GetThemeColors(_theme);

                    // Auto-apply theme if in design mode or if explicitly set
                    if (_parentForm != null && !IsInDesignMode())
                    {
                        ApplyTheme();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent form to which the theme will be applied.
        /// </summary>
        [Category("Behavior")]
        [Description("The parent form to which the theme will be applied.")]
        [DefaultValue(null)]
        public Form ParentForm
        {
            get { return _parentForm; }
            set
            {
                if (_parentForm != value)
                {
                    _parentForm = value;

                    // Update theme colors when parent changes
                    if (_parentForm != null && _theme != ZidThemes.None)
                    {
                        _themeColors = GridThemeHelper.GetThemeColors(_theme);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ThemeManager component.
        /// </summary>
        public ThemeManager()
        {
            _themeColors = GridThemeHelper.GetThemeColors(_theme);
        }

        /// <summary>
        /// Initializes a new instance of the ThemeManager component with a container.
        /// </summary>
        public ThemeManager(IContainer container) : this()
        {
            if (container != null)
            {
                container.Add(this);
            }
        }

        /// <summary>
        /// Applies the selected theme to the parent form and all its controls.
        /// </summary>
        public void ApplyTheme()
        {
            if (_parentForm == null)
            {
                throw new InvalidOperationException("ParentForm must be set before applying theme.");
            }

            if (_theme == ZidThemes.None)
            {
                // Reset to default appearance
                ResetToDefault(_parentForm);
                ResetControlsToDefault(_parentForm.Controls);
                return;
            }

            // Ensure we have theme colors
            if (_themeColors == null)
            {
                _themeColors = GridThemeHelper.GetThemeColors(_theme);
            }

            // Apply theme to the form itself
            ApplyThemeToForm(_parentForm);

            // Apply theme to all controls recursively
            ApplyThemeToControls(_parentForm.Controls);
        }

        /// <summary>
        /// Applies the theme to the form.
        /// </summary>
        private void ApplyThemeToForm(Form form)
        {
            form.BackColor = _themeColors.BackgroundColor;
            form.ForeColor = _themeColors.DefaultForeColor;

            // Apply font if available
            if (_themeColors.CellFont != null)
            {
                form.Font = _themeColors.CellFont;
            }
        }

        /// <summary>
        /// Recursively applies the theme to all controls.
        /// </summary>
        private void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control);

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        /// <summary>
        /// Applies the theme to a specific control based on its type.
        /// </summary>
        private void ApplyThemeToControl(Control control)
        {
            // Skip certain controls that manage their own themes
            if (control is Grid.ZidGrid)
            {
                // ZidGrid manages its own theme
                var grid = control as Grid.ZidGrid;
                grid.Theme = _theme;
                return;
            }

            // Apply base styling
            control.BackColor = _themeColors.DefaultBackColor;
            control.ForeColor = _themeColors.DefaultForeColor;
            control.Font = _themeColors.CellFont;

            // Specific control type handling
            if (control is Button)
            {
                ApplyThemeToButton((Button)control);
            }
            else if (control is TextBox)
            {
                ApplyThemeToTextBox((TextBox)control);
            }
            else if (control is ComboBox)
            {
                ApplyThemeToComboBox((ComboBox)control);
            }
            else if (control is Label)
            {
                ApplyThemeToLabel((Label)control);
            }
            else if (control is Panel)
            {
                ApplyThemeToPanel((Panel)control);
            }
            else if (control is GroupBox)
            {
                ApplyThemeToGroupBox((GroupBox)control);
            }
            else if (control is CheckBox || control is RadioButton)
            {
                ApplyThemeToCheckable(control);
            }
            else if (control is ListBox)
            {
                ApplyThemeToListBox((ListBox)control);
            }
            else if (control is ListView)
            {
                ApplyThemeToListView((ListView)control);
            }
            else if (control is TreeView)
            {
                ApplyThemeToTreeView((TreeView)control);
            }
            else if (control is DataGridView)
            {
                ApplyThemeToDataGridView((DataGridView)control);
            }
            else if (control is TabControl)
            {
                ApplyThemeToTabControl((TabControl)control);
            }
            else if (control is MenuStrip || control is ToolStrip)
            {
                ApplyThemeToToolStrip((ToolStrip)control);
            }
            else if (control is NumericUpDown)
            {
                ApplyThemeToNumericUpDown((NumericUpDown)control);
            }
            else if (control is TrackBar)
            {
                ApplyThemeToTrackBar((TrackBar)control);
            }
            else if (control is ProgressBar)
            {
                ApplyThemeToProgressBar((ProgressBar)control);
            }
            else if (control is DateTimePicker)
            {
                ApplyThemeToDateTimePicker((DateTimePicker)control);
            }
            else if (control is MonthCalendar)
            {
                ApplyThemeToMonthCalendar((MonthCalendar)control);
            }
            else if (control is RichTextBox)
            {
                ApplyThemeToRichTextBox((RichTextBox)control);
            }
            else if (control is MaskedTextBox)
            {
                ApplyThemeToMaskedTextBox((MaskedTextBox)control);
            }
            else if (control is CheckedListBox)
            {
                ApplyThemeToCheckedListBox((CheckedListBox)control);
            }
            else if (control is PictureBox)
            {
                ApplyThemeToPictureBox((PictureBox)control);
            }
            else if (control is SplitContainer)
            {
                ApplyThemeToSplitContainer((SplitContainer)control);
            }
            else if (control is PropertyGrid)
            {
                ApplyThemeToPropertyGrid((PropertyGrid)control);
            }
        }

        private void ApplyThemeToButton(Button button)
        {
            button.BackColor = _themeColors.HeaderBackColor;
            button.ForeColor = _themeColors.HeaderForeColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = _themeColors.AccentColor;
            button.Font = new Font(_themeColors.HeaderFont.FontFamily, 9f, FontStyle.Bold);
        }

        private void ApplyThemeToTextBox(TextBox textBox)
        {
            textBox.BackColor = Color.White;
            textBox.ForeColor = Color.Black; // Use dark text on white background
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ApplyThemeToComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = Color.Black; // Use dark text on white background
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private void ApplyThemeToLabel(Label label)
        {
            // Check if this is a header-style label (larger or bold font)
            if (label.Font.Size > 10 || label.Font.Bold)
            {
                label.ForeColor = _themeColors.HeaderBackColor;
                label.Font = new Font(_themeColors.HeaderFont.FontFamily, label.Font.Size, FontStyle.Bold);
            }
            else
            {
                label.ForeColor = _themeColors.DefaultForeColor;
            }

            // Labels typically use transparent background
            if (label.BackColor != Color.Transparent)
            {
                label.BackColor = _themeColors.BackgroundColor;
            }
        }

        private void ApplyThemeToPanel(Panel panel)
        {
            // Check if this looks like a header panel (docked at top)
            if (panel.Dock == DockStyle.Top && panel.Height < 100)
            {
                panel.BackColor = _themeColors.HeaderBackColor;
                // Update child controls in header panel
                foreach (Control child in panel.Controls)
                {
                    if (child is Label)
                    {
                        child.ForeColor = _themeColors.HeaderForeColor;
                        child.BackColor = Color.Transparent;
                    }
                }
            }
            else
            {
                panel.BackColor = _themeColors.DefaultBackColor;
            }
        }

        private void ApplyThemeToGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = _themeColors.HeaderBackColor;
            groupBox.Font = new Font(_themeColors.CellFont.FontFamily, _themeColors.CellFont.Size, FontStyle.Bold);
        }

        private void ApplyThemeToCheckable(Control control)
        {
            control.BackColor = Color.Transparent;
            control.ForeColor = _themeColors.DefaultForeColor;
        }

        private void ApplyThemeToListBox(ListBox listBox)
        {
            listBox.BackColor = Color.White;
            listBox.ForeColor = Color.Black; // Use dark text on white background
        }

        private void ApplyThemeToListView(ListView listView)
        {
            listView.BackColor = Color.White;
            listView.ForeColor = Color.Black; // Use dark text on white background

            if (listView.View == View.Details)
            {
                listView.GridLines = true;
            }
        }

        private void ApplyThemeToTreeView(TreeView treeView)
        {
            treeView.BackColor = Color.White;
            treeView.ForeColor = Color.Black; // Use dark text on white background
            treeView.LineColor = _themeColors.GridColor;
        }

        private void ApplyThemeToDataGridView(DataGridView dataGridView)
        {
            // Apply basic theme - ZidGrid handles its own
            dataGridView.BackgroundColor = _themeColors.BackgroundColor;
            dataGridView.DefaultCellStyle.BackColor = _themeColors.DefaultBackColor;
            dataGridView.DefaultCellStyle.ForeColor = _themeColors.DefaultForeColor;
            dataGridView.DefaultCellStyle.Font = _themeColors.CellFont;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = _themeColors.AlternatingBackColor;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = _themeColors.HeaderBackColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _themeColors.HeaderForeColor;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = _themeColors.HeaderFont;
            dataGridView.GridColor = _themeColors.GridColor;
            dataGridView.EnableHeadersVisualStyles = false;
        }

        private void ApplyThemeToTabControl(TabControl tabControl)
        {
            tabControl.Font = _themeColors.CellFont;

            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = _themeColors.BackgroundColor;
                page.ForeColor = _themeColors.DefaultForeColor;
            }
        }

        private void ApplyThemeToToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = _themeColors.HeaderBackColor;
            toolStrip.ForeColor = _themeColors.HeaderForeColor;

            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.BackColor = _themeColors.HeaderBackColor;
                item.ForeColor = _themeColors.HeaderForeColor;
            }
        }

        private void ApplyThemeToNumericUpDown(NumericUpDown numericUpDown)
        {
            numericUpDown.BackColor = Color.White;
            numericUpDown.ForeColor = Color.Black; // Use dark text on white background
            numericUpDown.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ApplyThemeToTrackBar(TrackBar trackBar)
        {
            trackBar.BackColor = _themeColors.DefaultBackColor;
            trackBar.ForeColor = _themeColors.AccentColor;
        }

        private void ApplyThemeToProgressBar(ProgressBar progressBar)
        {
            progressBar.BackColor = _themeColors.DefaultBackColor;
            progressBar.ForeColor = _themeColors.AccentColor;
        }

        private void ApplyThemeToDateTimePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.BackColor = Color.White;
            dateTimePicker.ForeColor = Color.Black; // Use dark text on white background
            dateTimePicker.CalendarForeColor = Color.Black;
            dateTimePicker.CalendarMonthBackground = Color.White;
            dateTimePicker.CalendarTitleBackColor = _themeColors.HeaderBackColor;
            dateTimePicker.CalendarTitleForeColor = _themeColors.HeaderForeColor;
            dateTimePicker.CalendarTrailingForeColor = Color.Gray;
        }

        private void ApplyThemeToMonthCalendar(MonthCalendar monthCalendar)
        {
            monthCalendar.BackColor = Color.White;
            monthCalendar.ForeColor = Color.Black; // Use dark text on white background
            monthCalendar.TitleBackColor = _themeColors.HeaderBackColor;
            monthCalendar.TitleForeColor = _themeColors.HeaderForeColor;
            monthCalendar.TrailingForeColor = Color.Gray;
        }

        private void ApplyThemeToRichTextBox(RichTextBox richTextBox)
        {
            richTextBox.BackColor = Color.White;
            richTextBox.ForeColor = Color.Black; // Use dark text on white background
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ApplyThemeToMaskedTextBox(MaskedTextBox maskedTextBox)
        {
            maskedTextBox.BackColor = Color.White;
            maskedTextBox.ForeColor = Color.Black; // Use dark text on white background
            maskedTextBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ApplyThemeToCheckedListBox(CheckedListBox checkedListBox)
        {
            checkedListBox.BackColor = Color.White;
            checkedListBox.ForeColor = Color.Black; // Use dark text on white background
        }

        private void ApplyThemeToPictureBox(PictureBox pictureBox)
        {
            pictureBox.BackColor = _themeColors.DefaultBackColor;
        }

        private void ApplyThemeToSplitContainer(SplitContainer splitContainer)
        {
            splitContainer.BackColor = _themeColors.GridColor;
            splitContainer.Panel1.BackColor = _themeColors.DefaultBackColor;
            splitContainer.Panel2.BackColor = _themeColors.DefaultBackColor;
        }

        private void ApplyThemeToPropertyGrid(PropertyGrid propertyGrid)
        {
            propertyGrid.BackColor = _themeColors.DefaultBackColor;
            propertyGrid.ViewBackColor = Color.White;
            propertyGrid.ViewForeColor = Color.Black; // Use dark text on white background
            propertyGrid.CategoryForeColor = _themeColors.HeaderForeColor;
            propertyGrid.CategorySplitterColor = _themeColors.GridColor;
            propertyGrid.HelpBackColor = _themeColors.DefaultBackColor;
            propertyGrid.HelpForeColor = _themeColors.DefaultForeColor;
            propertyGrid.LineColor = _themeColors.GridColor;
            propertyGrid.ViewBorderColor = _themeColors.AccentColor;
        }

        /// <summary>
        /// Resets the form to default Windows Forms appearance.
        /// </summary>
        private void ResetToDefault(Form form)
        {
            form.BackColor = SystemColors.Control;
            form.ForeColor = SystemColors.ControlText;
            form.Font = SystemFonts.DefaultFont;
        }

        /// <summary>
        /// Recursively resets all controls to default Windows Forms appearance.
        /// </summary>
        private void ResetControlsToDefault(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ResetControlToDefault(control);

                // Recursively reset child controls
                if (control.HasChildren)
                {
                    ResetControlsToDefault(control.Controls);
                }
            }
        }

        /// <summary>
        /// Resets a specific control to default Windows Forms appearance based on its type.
        /// </summary>
        private void ResetControlToDefault(Control control)
        {
            // Skip ZidGrid - let it manage its own theme reset
            if (control is Grid.ZidGrid)
            {
                var grid = control as Grid.ZidGrid;
                grid.Theme = ZidThemes.None;
                return;
            }

            // Reset base properties to defaults
            control.BackColor = SystemColors.Control;
            control.ForeColor = SystemColors.ControlText;
            control.Font = SystemFonts.DefaultFont;

            // Specific control type handling
            if (control is Button)
            {
                ResetButton((Button)control);
            }
            else if (control is TextBox)
            {
                ResetTextBox((TextBox)control);
            }
            else if (control is ComboBox)
            {
                ResetComboBox((ComboBox)control);
            }
            else if (control is Label)
            {
                ResetLabel((Label)control);
            }
            else if (control is Panel)
            {
                ResetPanel((Panel)control);
            }
            else if (control is GroupBox)
            {
                ResetGroupBox((GroupBox)control);
            }
            else if (control is CheckBox || control is RadioButton)
            {
                ResetCheckable(control);
            }
            else if (control is ListBox)
            {
                ResetListBox((ListBox)control);
            }
            else if (control is ListView)
            {
                ResetListView((ListView)control);
            }
            else if (control is TreeView)
            {
                ResetTreeView((TreeView)control);
            }
            else if (control is DataGridView)
            {
                ResetDataGridView((DataGridView)control);
            }
            else if (control is TabControl)
            {
                ResetTabControl((TabControl)control);
            }
            else if (control is MenuStrip || control is ToolStrip)
            {
                ResetToolStrip((ToolStrip)control);
            }
            else if (control is NumericUpDown)
            {
                ResetNumericUpDown((NumericUpDown)control);
            }
            else if (control is TrackBar)
            {
                ResetTrackBar((TrackBar)control);
            }
            else if (control is ProgressBar)
            {
                ResetProgressBar((ProgressBar)control);
            }
            else if (control is DateTimePicker)
            {
                ResetDateTimePicker((DateTimePicker)control);
            }
            else if (control is MonthCalendar)
            {
                ResetMonthCalendar((MonthCalendar)control);
            }
            else if (control is RichTextBox)
            {
                ResetRichTextBox((RichTextBox)control);
            }
            else if (control is MaskedTextBox)
            {
                ResetMaskedTextBox((MaskedTextBox)control);
            }
            else if (control is CheckedListBox)
            {
                ResetCheckedListBox((CheckedListBox)control);
            }
            else if (control is PictureBox)
            {
                ResetPictureBox((PictureBox)control);
            }
            else if (control is SplitContainer)
            {
                ResetSplitContainer((SplitContainer)control);
            }
            else if (control is PropertyGrid)
            {
                ResetPropertyGrid((PropertyGrid)control);
            }
        }

        private void ResetButton(Button button)
        {
            button.BackColor = SystemColors.Control;
            button.ForeColor = SystemColors.ControlText;
            button.FlatStyle = FlatStyle.Standard;
            button.Font = SystemFonts.DefaultFont;
        }

        private void ResetTextBox(TextBox textBox)
        {
            textBox.BackColor = SystemColors.Window;
            textBox.ForeColor = SystemColors.WindowText;
            textBox.BorderStyle = BorderStyle.Fixed3D;
        }

        private void ResetComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = SystemColors.Window;
            comboBox.ForeColor = SystemColors.WindowText;
            comboBox.FlatStyle = FlatStyle.Standard;
        }

        private void ResetLabel(Label label)
        {
            label.ForeColor = SystemColors.ControlText;
            if (label.BackColor != Color.Transparent)
            {
                label.BackColor = Color.Transparent;
            }
        }

        private void ResetPanel(Panel panel)
        {
            panel.BackColor = SystemColors.Control;
        }

        private void ResetGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = SystemColors.ControlText;
            groupBox.Font = SystemFonts.DefaultFont;
        }

        private void ResetCheckable(Control control)
        {
            control.BackColor = Color.Transparent;
            control.ForeColor = SystemColors.ControlText;
        }

        private void ResetListBox(ListBox listBox)
        {
            listBox.BackColor = SystemColors.Window;
            listBox.ForeColor = SystemColors.WindowText;
        }

        private void ResetListView(ListView listView)
        {
            listView.BackColor = SystemColors.Window;
            listView.ForeColor = SystemColors.WindowText;
        }

        private void ResetTreeView(TreeView treeView)
        {
            treeView.BackColor = SystemColors.Window;
            treeView.ForeColor = SystemColors.WindowText;
            treeView.LineColor = Color.Black;
        }

        private void ResetDataGridView(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = SystemColors.AppWorkspace;
            dataGridView.DefaultCellStyle.BackColor = SystemColors.Window;
            dataGridView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            dataGridView.DefaultCellStyle.Font = SystemFonts.DefaultFont;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = SystemFonts.DefaultFont;
            dataGridView.GridColor = SystemColors.ControlDark;
            dataGridView.EnableHeadersVisualStyles = true;
        }

        private void ResetTabControl(TabControl tabControl)
        {
            tabControl.Font = SystemFonts.DefaultFont;

            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = SystemColors.Control;
                page.ForeColor = SystemColors.ControlText;
            }
        }

        private void ResetToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = SystemColors.Control;
            toolStrip.ForeColor = SystemColors.ControlText;

            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.BackColor = SystemColors.Control;
                item.ForeColor = SystemColors.ControlText;
            }
        }

        private void ResetNumericUpDown(NumericUpDown numericUpDown)
        {
            numericUpDown.BackColor = SystemColors.Window;
            numericUpDown.ForeColor = SystemColors.WindowText;
        }

        private void ResetTrackBar(TrackBar trackBar)
        {
            trackBar.BackColor = SystemColors.Control;
        }

        private void ResetProgressBar(ProgressBar progressBar)
        {
            progressBar.BackColor = SystemColors.Control;
            progressBar.ForeColor = SystemColors.Highlight;
        }

        private void ResetDateTimePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.BackColor = SystemColors.Window;
            dateTimePicker.ForeColor = SystemColors.WindowText;
        }

        private void ResetMonthCalendar(MonthCalendar monthCalendar)
        {
            monthCalendar.BackColor = SystemColors.Window;
            monthCalendar.ForeColor = SystemColors.WindowText;
            monthCalendar.TitleBackColor = SystemColors.ActiveCaption;
            monthCalendar.TitleForeColor = SystemColors.ActiveCaptionText;
            monthCalendar.TrailingForeColor = SystemColors.GrayText;
        }

        private void ResetRichTextBox(RichTextBox richTextBox)
        {
            richTextBox.BackColor = SystemColors.Window;
            richTextBox.ForeColor = SystemColors.WindowText;
            richTextBox.BorderStyle = BorderStyle.Fixed3D;
        }

        private void ResetMaskedTextBox(MaskedTextBox maskedTextBox)
        {
            maskedTextBox.BackColor = SystemColors.Window;
            maskedTextBox.ForeColor = SystemColors.WindowText;
            maskedTextBox.BorderStyle = BorderStyle.Fixed3D;
        }

        private void ResetCheckedListBox(CheckedListBox checkedListBox)
        {
            checkedListBox.BackColor = SystemColors.Window;
            checkedListBox.ForeColor = SystemColors.WindowText;
        }

        private void ResetPictureBox(PictureBox pictureBox)
        {
            pictureBox.BackColor = SystemColors.Control;
        }

        private void ResetSplitContainer(SplitContainer splitContainer)
        {
            splitContainer.BackColor = SystemColors.Control;
            splitContainer.Panel1.BackColor = SystemColors.Control;
            splitContainer.Panel2.BackColor = SystemColors.Control;
        }

        private void ResetPropertyGrid(PropertyGrid propertyGrid)
        {
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.ViewBackColor = SystemColors.Window;
            propertyGrid.ViewForeColor = SystemColors.WindowText;
            propertyGrid.CategoryForeColor = SystemColors.ControlText;
            propertyGrid.CategorySplitterColor = SystemColors.Control;
            propertyGrid.HelpBackColor = SystemColors.Control;
            propertyGrid.HelpForeColor = SystemColors.ControlText;
            propertyGrid.LineColor = SystemColors.InactiveBorder;
        }

        /// <summary>
        /// Checks if the component is running in design mode.
        /// </summary>
        private bool IsInDesignMode()
        {
            return (LicenseManager.UsageMode == LicenseUsageMode.Designtime) ||
                   (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }
    }
}
