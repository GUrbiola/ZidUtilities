using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.Grid;

namespace ZidUtilities.CommonCode.Win.Controls
{
    /// <summary>
    /// Defines the filter behavior for the TokenSelect control.
    /// </summary>
    public enum FilterBehavior
    {
        /// <summary>
        /// Filter results and allow selection from filtered list.
        /// </summary>
        FilterAndSelect,

        /// <summary>
        /// Type to select directly - filters and auto-selects on exact match.
        /// </summary>
        DirectSelection
    }

    /// <summary>
    /// A UserControl that provides autocomplete dropdown functionality with token/tag display.
    /// When items are selected, they appear as visual tokens that can be individually removed.
    /// </summary>
    public partial class TokenSelect : UserControl
    {
        #region Internal Data Classes

        /// <summary>
        /// Internal class to represent a token item with display text and value.
        /// </summary>
        private class TokenItem
        {
            public string Display { get; set; }
            public object Value { get; set; }
        }
        
        private ZidThemes? myTheme;
        #endregion

        #region Fields

        private List<TokenItem> _availableItems;
        private List<TokenItem> _selectedTokens;
        private Dictionary<object, TokenChip> _tokenChipMap;
        private DropdownForm _dropdownForm;
        private int _minimumCharactersForDropdown = 1;
        private bool _allowMultipleSelection = true;
        private int _maxTokens = 0;
        private bool _readOnly = false;
        private string _placeholder = "Type to search...";
        private bool _showingPlaceholder = false;

        private Color _tokenBackColor;
        private Color _tokenForeColor;
        private Color _tokenBorderColor;

        #endregion

        #region Events

        /// <summary>
        /// Event arguments for token operations.
        /// </summary>
        public class TokenEventArgs : EventArgs
        {
            public string DisplayText { get; set; }
            public object Value { get; set; }
        }

        /// <summary>
        /// Raised when a token is added to the control.
        /// </summary>
        public event EventHandler<TokenEventArgs> OnTokenAdded;

        /// <summary>
        /// Raised when a token is removed from the control.
        /// </summary>
        public event EventHandler<TokenEventArgs> OnTokenRemoved;

        /// <summary>
        /// Raised when the selection changes (token added or removed).
        /// </summary>
        public event EventHandler OnSelectionChanged;

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets or sets the theme for the control.
        ///// </summary>
        //[Category("Appearance")]
        //[Description("The theme to apply to the control.")]
        //[DefaultValue(ZidThemes.Default)]
        //public ZidThemes Theme
        //{
        //    get { return _theme; }
        //    set
        //    {
        //        if (_theme != value)
        //        {
        //            _theme = value;
        //            ApplyThemeInternal(value);
        //        }
        //    }
        //}

        /// <summary>
        /// Gets or sets the minimum number of characters required before showing the dropdown.
        /// </summary>
        [Category("Behavior")]
        [Description("Minimum number of characters required before showing the dropdown.")]
        [DefaultValue(1)]
        public int MinimumCharactersForDropdown
        {
            get { return _minimumCharactersForDropdown; }
            set { _minimumCharactersForDropdown = Math.Max(0, value); }
        }

        /// <summary>
        /// Gets or sets whether multiple selections are allowed.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines whether multiple selections are allowed.")]
        [DefaultValue(true)]
        public bool AllowMultipleSelection
        {
            get { return _allowMultipleSelection; }
            set { _allowMultipleSelection = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of tokens allowed (0 = unlimited).
        /// </summary>
        [Category("Behavior")]
        [Description("Maximum number of tokens allowed (0 = unlimited).")]
        [DefaultValue(0)]
        public int MaxTokens
        {
            get { return _maxTokens; }
            set { _maxTokens = Math.Max(0, value); }
        }

        /// <summary>
        /// Gets or sets whether the control is read-only.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines whether the control is read-only.")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                inputBox.ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the placeholder text shown when the input is empty.
        /// </summary>
        [Category("Appearance")]
        [Description("Placeholder text shown when the input is empty.")]
        [DefaultValue("Type to search...")]
        public string Placeholder
        {
            get { return _placeholder; }
            set
            {
                _placeholder = value;
                UpdatePlaceholder();
            }
        }

        /// <summary>
        /// Gets or sets the filter behavior for the control.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines how filtering and selection works.")]
        [DefaultValue(FilterBehavior.FilterAndSelect)]
        public FilterBehavior FilterBehavior { get; set; } = FilterBehavior.FilterAndSelect;

        /// <summary>
        /// Gets the list of selected values.
        /// </summary>
        [Browsable(false)]
        public List<object> SelectedValues
        {
            get
            {
                return _selectedTokens?.Select(t => t.Value).ToList() ?? new List<object>();
            }
        }

        /// <summary>
        /// Gets the list of selected display texts.
        /// </summary>
        [Browsable(false)]
        public List<string> SelectedDisplayTexts
        {
            get
            {
                return _selectedTokens?.Select(t => t.Display).ToList() ?? new List<string>();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TokenSelect control.
        /// </summary>
        public TokenSelect()
        {
            InitializeComponent();

            // Initialize collections
            _availableItems = new List<TokenItem>();
            _selectedTokens = new List<TokenItem>();
            _tokenChipMap = new Dictionary<object, TokenChip>();

            // Initialize token colors with defaults (will be overridden by ApplyTheme)
            _tokenBackColor = SystemColors.Highlight;
            _tokenForeColor = Color.White;
            _tokenBorderColor = SystemColors.Highlight;

            // Wire up events
            inputBox.TextChanged += InputBox_TextChanged;
            inputBox.KeyDown += InputBox_KeyDown;
            inputBox.GotFocus += InputBox_GotFocus;
            inputBox.LostFocus += InputBox_LostFocus;
            inputBox.Click += tokenPanel_Click;

            // Focus handling - click anywhere to focus input
            borderPanel.Click += (s, e) => inputBox.Focus();
            tokenPanel.Click += (s, e) => inputBox.Focus();
            this.Click += (s, e) => inputBox.Focus();

            // Handle control resize to adjust textbox size
            tokenPanel.SizeChanged += TokenPanel_SizeChanged;

            //// Apply initial theme
            //ApplyThemeInternal(_theme);

            // Show placeholder initially (delay to ensure control is loaded)
            this.Load += (s, e) =>
            {
                _showingPlaceholder = true;
                inputBox.Text = _placeholder;
                inputBox.ForeColor = Color.Gray;
            };

            UpdatePlaceholder();


        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the data source using separate display and value lists.
        /// </summary>
        public void SetDataSource(List<string> displayItems, List<object> valueItems)
        {
            if (displayItems == null)
                throw new ArgumentNullException(nameof(displayItems));
            if (valueItems == null)
                throw new ArgumentNullException(nameof(valueItems));
            if (displayItems.Count != valueItems.Count)
                throw new ArgumentException("Display and value lists must have the same length.");

            _availableItems.Clear();

            for (int i = 0; i < displayItems.Count; i++)
            {
                _availableItems.Add(new TokenItem
                {
                    Display = displayItems[i],
                    Value = valueItems[i]
                });
            }
        }

        /// <summary>
        /// Sets the data source using a dictionary.
        /// </summary>
        public void SetDataSource(Dictionary<string, object> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _availableItems.Clear();

            foreach (var kvp in items)
            {
                _availableItems.Add(new TokenItem
                {
                    Display = kvp.Key,
                    Value = kvp.Value
                });
            }
        }

        /// <summary>
        /// Adds a token to the control.
        /// </summary>
        public void AddToken(string display, object value)
        {
            if (string.IsNullOrWhiteSpace(display))
                return;

            // Validate multiple selection mode
            if (!_allowMultipleSelection && _selectedTokens.Count > 0)
            {
                // Remove existing token first
                ClearTokens();
            }

            // Check max tokens
            if (_maxTokens > 0 && _selectedTokens.Count >= _maxTokens)
                return;

            // Check duplicates
            if (_selectedTokens.Any(t => t.Value.Equals(value)))
                return;

            // Create token item
            var tokenItem = new TokenItem { Display = display, Value = value };
            _selectedTokens.Add(tokenItem);

            // Create visual chip
            var chip = new TokenChip(display, value);
            chip.RemoveClicked += TokenChip_RemoveClicked;

            // Add to map and panel
            _tokenChipMap[value] = chip;

            tokenPanel.Controls.Add(chip);

            // Ensure input box is last
            tokenPanel.Controls.SetChildIndex(inputBox, tokenPanel.Controls.Count - 1);

            // Fire events
            OnTokenAdded?.Invoke(this, new TokenEventArgs { DisplayText = display, Value = value });
            OnSelectionChanged?.Invoke(this, EventArgs.Empty);

            // Clear input and prevent text selection
            inputBox.Clear();
            inputBox.SelectionStart = 0;
            inputBox.SelectionLength = 0;
            UpdatePlaceholder();

            // Recalculate textbox size
            TokenPanel_SizeChanged(tokenPanel, EventArgs.Empty);
            if (myTheme.HasValue)
            {
                ThemeManager th = new ThemeManager();
                th.Theme = myTheme.Value;
                th.ApplyThemeTo(tokenPanel);
            }
        }

        /// <summary>
        /// Removes a token from the control by value.
        /// </summary>
        public void RemoveToken(object value)
        {
            if (_readOnly)
                return;

            // Find and remove from selected
            var token = _selectedTokens.FirstOrDefault(t => t.Value.Equals(value));
            if (token == null)
                return;

            _selectedTokens.Remove(token);

            // Remove and dispose chip
            if (_tokenChipMap.TryGetValue(value, out TokenChip chip))
            {
                tokenPanel.Controls.Remove(chip);
                _tokenChipMap.Remove(value);
                chip.Dispose();
            }

            // Fire events
            OnTokenRemoved?.Invoke(this, new TokenEventArgs
            {
                DisplayText = token.Display,
                Value = value
            });
            OnSelectionChanged?.Invoke(this, EventArgs.Empty);

            UpdatePlaceholder();

            // Recalculate textbox size
            TokenPanel_SizeChanged(tokenPanel, EventArgs.Empty);
        }

        /// <summary>
        /// Clears all tokens from the control.
        /// </summary>
        public void ClearTokens()
        {
            if (_readOnly)
                return;

            // Make a copy of the list to avoid modification during iteration
            var tokensToRemove = _selectedTokens.ToList();

            foreach (var token in tokensToRemove)
            {
                RemoveToken(token.Value);
            }
        }

        #endregion

        #region Theme Methods
        internal void SetTheme(ZidThemes theme)
        {
            myTheme = theme;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the filtered list of items based on input text.
        /// </summary>
        private List<TokenItem> GetFilteredItems()
        {
            string filter = inputBox.Text.Trim().ToLower();

            return _availableItems
                // Exclude already selected (no duplicates)
                .Where(item => !_selectedTokens.Any(t => t.Value.Equals(item.Value)))
                // Apply text filter (case-insensitive)
                .Where(item => string.IsNullOrEmpty(filter) ||
                              item.Display.ToLower().Contains(filter))
                .ToList();
        }

        /// <summary>
        /// Shows the dropdown with filtered suggestions.
        /// </summary>
        private void ShowDropdown()
        {
            if (_readOnly)
                return;

            // Create dropdown if needed
            if (_dropdownForm == null)
            {
                _dropdownForm = new DropdownForm();
                _dropdownForm.ItemSelected += DropdownForm_ItemSelected;
            }

            // Get filtered items
            var filtered = GetFilteredItems();
            if (filtered.Count == 0)
            {
                HideDropdown();
                return;
            }

            // Position below control
            Point location = this.PointToScreen(new Point(0, this.Height));

            // Check screen boundaries
            Screen screen = Screen.FromControl(this);
            int maxHeight = screen.WorkingArea.Bottom - location.Y;

            // If not enough space below, position above
            if (maxHeight < 100 && location.Y - this.Height > 100)
            {
                int dropdownHeight = Math.Min(200, filtered.Count * 20 + 4);
                location = this.PointToScreen(new Point(0, -dropdownHeight));
            }

            // Update dropdown
            _dropdownForm.SetItems(filtered);
            _dropdownForm.Location = location;
            _dropdownForm.Width = this.Width;

            // Show dropdown without stealing focus
            if (!_dropdownForm.Visible)
            {
                // Store current focus control
                Control focusedControl = inputBox;

                _dropdownForm.Show(this);
                _dropdownForm.TopMost = true;

                // Restore focus to input box immediately
                if (focusedControl != null && !focusedControl.IsDisposed)
                {
                    focusedControl.Focus();
                }
            }
        }

        /// <summary>
        /// Hides the dropdown.
        /// </summary>
        private void HideDropdown()
        {
            if (_dropdownForm != null && _dropdownForm.Visible)
            {
                _dropdownForm.Hide();
            }
        }

        /// <summary>
        /// Updates the placeholder text visibility.
        /// </summary>
        private void UpdatePlaceholder()
        {
            bool shouldShowPlaceholder = string.IsNullOrEmpty(inputBox.Text) &&
                                        !inputBox.Focused &&
                                        _selectedTokens.Count == 0;

            if (shouldShowPlaceholder && !_showingPlaceholder)
            {
                _showingPlaceholder = true;
                inputBox.Text = _placeholder;
                inputBox.ForeColor = Color.Gray;
            }
            else if (!shouldShowPlaceholder && _showingPlaceholder)
            {
                _showingPlaceholder = false;
                inputBox.Text = string.Empty;
                var colors = GridThemeHelper.GetThemeColors(myTheme ?? ZidThemes.Default);
                inputBox.ForeColor = colors.DefaultForeColor;
            }
        }

        /// <summary>
        /// Adjusts the inputBox size when the token panel size changes.
        /// </summary>
        private void TokenPanel_SizeChanged(object sender, EventArgs e)
        {
            // Calculate available width
            int usedWidth = 0;
            foreach (Control ctrl in tokenPanel.Controls)
            {
                if (ctrl != inputBox)
                {
                    usedWidth += ctrl.Width + ctrl.Margin.Horizontal;
                }
            }

            // Set inputBox width to fill remaining space (with minimum width)
            int availableWidth = tokenPanel.ClientSize.Width - usedWidth - tokenPanel.Padding.Horizontal - 10;
            inputBox.Width = Math.Max(200, availableWidth);
        }

        #endregion

        #region Event Handlers

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            if (_showingPlaceholder)
                return;

            string text = inputBox.Text.Trim();

            // Check minimum characters
            if (text.Length >= _minimumCharactersForDropdown)
            {
                if (FilterBehavior == FilterBehavior.DirectSelection)
                {
                    // DirectSelection mode: Auto-select on exact match
                    var exactMatch = GetFilteredItems()
                        .FirstOrDefault(item => item.Display.Equals(text, StringComparison.OrdinalIgnoreCase));

                    if (exactMatch != null)
                    {
                        AddToken(exactMatch.Display, exactMatch.Value);
                        HideDropdown();
                    }
                    else
                    {
                        ShowDropdown();
                    }
                }
                else
                {
                    // FilterAndSelect mode: Always show dropdown with filtered results
                    ShowDropdown();
                }
            }
            else
            {
                HideDropdown();
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Select first item in dropdown if visible
                if (_dropdownForm != null && _dropdownForm.Visible && _dropdownForm.HasItems)
                {
                    _dropdownForm.SelectCurrentItem();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // Close dropdown
                HideDropdown();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(inputBox.Text))
            {
                // Remove last token on backspace when input is empty
                if (_selectedTokens.Count > 0)
                {
                    var lastToken = _selectedTokens[_selectedTokens.Count - 1];
                    RemoveToken(lastToken.Value);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Navigate dropdown without losing focus
                if (_dropdownForm != null && _dropdownForm.Visible)
                {
                    _dropdownForm.MoveSelectionDown();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                // Navigate dropdown without losing focus
                if (_dropdownForm != null && _dropdownForm.Visible)
                {
                    _dropdownForm.MoveSelectionUp();
                    e.Handled = true;
                }
            }
        }

        private void InputBox_GotFocus(object sender, EventArgs e)
        {
            if (_showingPlaceholder)
            {
                _showingPlaceholder = false;
                inputBox.Text = string.Empty;
                var colors = GridThemeHelper.GetThemeColors(myTheme ?? ZidThemes.Default);
                inputBox.ForeColor = colors.DefaultForeColor;
            }
        }

        private void InputBox_LostFocus(object sender, EventArgs e)
        {
            // Delay to allow dropdown click to register
            this.BeginInvoke(new Action(() =>
            {
                UpdatePlaceholder();
                // Hide dropdown only if neither the input box nor the dropdown has focus
                if (_dropdownForm != null && _dropdownForm.Visible)
                {
                    if (!inputBox.Focused && !_dropdownForm.ContainsFocus)
                    {
                        HideDropdown();
                    }
                }
            }));
        }

        private void DropdownForm_ItemSelected(object sender, TokenItem item)
        {
            AddToken(item.Display, item.Value);
            HideDropdown();  // Auto-close after selection
            inputBox.Focus();  // Return focus to input
        }

        private void TokenChip_RemoveClicked(object sender, EventArgs e)
        {
            if (sender is TokenChip chip)
            {
                RemoveToken(chip.Value);
                inputBox.Focus();
            }
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents a visual token chip with display text and remove button.
        /// </summary>
        private class TokenChip : Panel
        {
            private Label lblText;
            private PictureBox picClose;

            public string DisplayText { get; private set; }
            public object Value { get; private set; }
            public bool IsHovered { get; private set; }

            public event EventHandler RemoveClicked;

            public TokenChip(string displayText, object value)
            {
                DisplayText = displayText;
                Value = value;

                // Panel setup
                this.Height = 22;
                this.Margin = new Padding(2, 3, 2, 3);
                this.Padding = new Padding(10, 3, 10, 3);
                this.BorderStyle = BorderStyle.FixedSingle;
                this.BackColor = SystemColors.Highlight;
                this.AutoSize = false;

                // Label for text
                lblText = new Label
                {
                    Text = displayText,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
                };

                // PictureBox for X button
                picClose = new PictureBox
                {
                    Size = new Size(12, 12),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    SizeMode = PictureBoxSizeMode.CenterImage
                };

                // Create simple X image with white color initially (will be updated by ApplyColors)
                picClose.Image = CreateXImage(Color.Black);
                picClose.Click += PicClose_Click;
                picClose.MouseEnter += (s, e) => { IsHovered = true; this.Invalidate(); };
                picClose.MouseLeave += (s, e) => { IsHovered = false; this.Invalidate(); };

                // Add controls first
                this.Controls.Add(lblText);
                this.Controls.Add(picClose);

                // Use SuspendLayout/ResumeLayout for better layout performance
                this.SuspendLayout();

                // Position label with left padding
                lblText.Location = new Point(10, 3);

                // Wait for label to calculate its size
                lblText.PerformLayout();

                // Position close button to the right of label with generous spacing
                picClose.Location = new Point(lblText.Right + 20, 4);

                // Size panel to fit content (label + close button + right padding)
                // = label.Right + spacing (12) + closeButton.Width (12) + right padding (10)
                this.Width = lblText.Right + 20 + picClose.Width + 10;

                this.ResumeLayout(false);

            }

            private Bitmap CreateXImage(Color color)
            {
                Bitmap bmp = new Bitmap(12, 12);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen pen = new Pen(color, 2))
                    {
                        g.DrawLine(pen, 2, 2, 10, 10);
                        g.DrawLine(pen, 10, 2, 2, 10);
                    }
                }
                return bmp;
            }

            private void PicClose_Click(object sender, EventArgs e)
            {
                RemoveClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Dropdown form that displays filtered suggestions.
        /// </summary>
        private class DropdownForm : Form
        {
            private ListBox listBox;
            private List<TokenItem> _items;
            private const int BORDER_WIDTH = 1;
            private const int WS_EX_NOACTIVATE = 0x08000000;
            private const int WS_EX_TOOLWINDOW = 0x00000080;

            public bool HasItems => listBox.Items.Count > 0;
            public event EventHandler<TokenItem> ItemSelected;

            // Prevent the form from stealing focus when shown
            protected override bool ShowWithoutActivation
            {
                get { return true; }
            }

            // Override CreateParams to prevent activation
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams baseParams = base.CreateParams;
                    baseParams.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                    return baseParams;
                }
            }

            public DropdownForm()
            {
                // Form setup - no border, we'll draw our own
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.BackColor = Color.White;
                this.MinimumSize = new Size(100, 50);
                this.MaximumSize = new Size(800, 200);
                this.Padding = new Padding(BORDER_WIDTH);

                // ListBox - inset by border width
                listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    IntegralHeight = true
                };

                listBox.Click += ListBox_Click;
                listBox.DoubleClick += ListBox_Click;
                listBox.KeyDown += ListBox_KeyDown;
                listBox.MouseMove += ListBox_MouseMove;

                this.Controls.Add(listBox);
                this.Paint += DropdownForm_Paint;
            }

            // Draw custom border
            private void DropdownForm_Paint(object sender, PaintEventArgs e)
            {
                // Draw a subtle border to make it look professional
                using (Pen borderPen = new Pen(Color.FromArgb(180, 180, 180), BORDER_WIDTH))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
                }
            }

            // Highlight item on hover
            private void ListBox_MouseMove(object sender, MouseEventArgs e)
            {
                int index = listBox.IndexFromPoint(e.Location);
                if (index >= 0 && index < listBox.Items.Count)
                {
                    listBox.SelectedIndex = index;
                }
            }

            public void SetItems(List<TokenItem> items)
            {
                _items = items;
                listBox.Items.Clear();

                foreach (var item in items)
                {
                    listBox.Items.Add(item.Display);
                }

                if (listBox.Items.Count > 0)
                {
                    listBox.SelectedIndex = 0;

                    // Auto-size height (max 10 items) + border
                    int itemHeight = listBox.ItemHeight;
                    int visibleItems = Math.Min(10, listBox.Items.Count);
                    this.Height = (visibleItems * itemHeight) + (BORDER_WIDTH * 2) + 2;
                }
            }

            public void SelectCurrentItem()
            {
                if (listBox.Items.Count > 0 && listBox.SelectedIndex >= 0)
                {
                    ListBox_Click(listBox, EventArgs.Empty);
                }
            }

            public void MoveSelectionDown()
            {
                if (listBox.Items.Count > 0)
                {
                    if (listBox.SelectedIndex < listBox.Items.Count - 1)
                    {
                        listBox.SelectedIndex++;
                        // Ensure the selected item is visible
                        listBox.TopIndex = Math.Max(0, listBox.SelectedIndex - 9);
                    }
                }
            }

            public void MoveSelectionUp()
            {
                if (listBox.Items.Count > 0)
                {
                    if (listBox.SelectedIndex > 0)
                    {
                        listBox.SelectedIndex--;
                        // Ensure the selected item is visible
                        listBox.TopIndex = Math.Min(listBox.SelectedIndex, listBox.TopIndex);
                    }
                }
            }

            private void ListBox_Click(object sender, EventArgs e)
            {
                if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex < _items.Count)
                {
                    var selectedItem = _items[listBox.SelectedIndex];
                    ItemSelected?.Invoke(this, selectedItem);
                }
            }

            private void ListBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ListBox_Click(sender, e);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Hide();
                    e.Handled = true;
                }
            }
        }

        #endregion

        private void tokenPanel_Click(object sender, EventArgs e)
        {
            if(MinimumCharactersForDropdown == 0)
                ShowDropdown();
        }
    }
}
