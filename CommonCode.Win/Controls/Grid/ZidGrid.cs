using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.Win.Controls.Grid.GridFilter;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    public partial class ZidGrid : UserControl
    {
        private int MaxRecordCount;
        private ZidGridMenuItemCollection _customMenuItems;
        private ZidGridPluginCollection _plugins;



        public ZidGrid()
        {
            InitializeComponent();

            _customMenuItems = new ZidGridMenuItemCollection();
            _plugins = new ZidGridPluginCollection();

            ZidGridMenuItem hideColumn = new ZidGridMenuItem();
            hideColumn.Text = "Hide this column";
            hideColumn.Image = Resources.ColumnHide32;
            hideColumn.Click += HideColumn_Click;

            _customMenuItems.Add(hideColumn);

            ZidGridMenuItem toggleFilter = new ZidGridMenuItem();
            toggleFilter.Text = "Show/Hide Filter Box";
            toggleFilter.Image = Resources.FunnelNew32;
            toggleFilter.Click += ToggleFilter_Click;

            _customMenuItems.Add(toggleFilter);

            ZidGridMenuItem resizeColumn = new ZidGridMenuItem();
            resizeColumn.Text = "Adjust this column's width";
            resizeColumn.Image = Resources.ResizeColumn32;
            resizeColumn.Click += ResizeColumn_Click;

            _customMenuItems.Add(resizeColumn);



            GridView.RowsAdded += new DataGridViewRowsAddedEventHandler(GridView_RowsAdded);
            GridView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(GridView_RowsRemoved);
            //GridView.CellFormatting += new DataGridViewCellFormattingEventHandler(GridView_CellFormatting);//first attempts from Claude to handle binary data columns
            //GridView.DataError += new DataGridViewDataErrorEventHandler(GridView_DataError);//first attempts from Claude to handle binary data columns
            GridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(GridView_DataBindingComplete);
            GridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(GridView_ColumnHeaderMouseClick);
        }

        private void ResizeColumn_Click(object sender, ZidGridMenuItemClickEventArgs e)
        {
            // Step 1: validate
            if (GridView == null)
                return;

            int colIndex = e.ColumnIndex;
            if (colIndex < 0 || colIndex >= GridView.Columns.Count)
                return;

            var column = GridView.Columns[colIndex];
            if (!column.Visible)
                return;

            // Step 3: prepare fonts
            Font headerFont = column.HeaderCell?.InheritedStyle?.Font
                              ?? this.TitleFont
                              ?? GridView.ColumnHeadersDefaultCellStyle?.Font
                              ?? GridView.Font;

            // We'll compute maximum width needed
            int maxWidth = 0;

            // Step 4: measure header text
            string headerText = column.HeaderText ?? string.Empty;
            var headerSize = TextRenderer.MeasureText(headerText, headerFont, new Size(int.MaxValue, int.MaxValue),
                                                     TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            maxWidth = Math.Max(maxWidth, headerSize.Width);

            // Step 5: iterate visible rows and measure cell text
            foreach (DataGridViewRow row in GridView.Rows)
            {
                if (row == null || !row.Visible || row.IsNewRow)
                    continue;

                DataGridViewCell cell = row.Cells[colIndex];
                if (cell == null)
                    continue;

                // prefer formatted value (what user sees); fallback to raw value
                object fv = null;
                try
                {
                    fv = cell.FormattedValue;
                }
                catch
                {
                    // ignore formatting exceptions and use Value
                    fv = cell.Value;
                }
                if (fv == null)
                    fv = cell.Value;

                string text = fv?.ToString() ?? string.Empty;

                Font cellFont = cell.InheritedStyle?.Font
                                ?? column.DefaultCellStyle?.Font
                                ?? this.CellFont
                                ?? GridView.DefaultCellStyle?.Font
                                ?? GridView.Font;

                var textSize = TextRenderer.MeasureText(text, cellFont, new Size(int.MaxValue, int.MaxValue),
                                                       TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                maxWidth = Math.Max(maxWidth, textSize.Width);
            }

            // Step 6: padding (cell padding + some extra room for sort glyphs/borders)
            int padding = 16; // basic breathing room
            var colPadding = column.DefaultCellStyle?.Padding ?? Padding.Empty;
            if (colPadding != Padding.Empty)
                padding += colPadding.Left + colPadding.Right;

            // Add a bit more to account for gridlines / glyphs
            padding += SystemInformation.BorderSize.Width + 6;

            int desiredWidth = maxWidth + padding;

            // Step 7: clamp to available width in grid
            int available = GridView.ClientSize.Width - GridView.RowHeadersWidth - 4;
            if (available < 20) available = 20;
            if (desiredWidth > available)
                desiredWidth = available;

            // Ensure a sensible minimum
            desiredWidth = Math.Max(desiredWidth, 20);

            // Step 8: set width and refresh
            try
            {
                column.Width = desiredWidth;
                GridView.InvalidateColumn(colIndex);
            }
            catch
            {
                // if setting width fails for some reason, ignore silently
            }
        }

        private void ToggleFilter_Click(object sender, ZidGridMenuItemClickEventArgs e)
        {
            this.FilterBoxPosition = this.FilterBoxPosition == FilterPosition.Top ? FilterPosition.Off : FilterPosition.Top;
        }

        private void HideColumn_Click(object sender, ZidGridMenuItemClickEventArgs e)
        {
            GridView.Columns[e.ColumnIndex].Visible = false;
        }

        void GridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetRowCount();
        }

        void GridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SetRowCount();
        }
        
        void GridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Handle binary columns after data binding is complete
            GridView.SuspendLayout();

            try
            {
                // First, collect all binary columns to avoid modifying collection while iterating
                var binaryColumns = new System.Collections.Generic.List<DataGridViewColumn>();

                foreach (DataGridViewColumn column in GridView.Columns)
                {
                    if (column.ValueType != null)
                    {
                        // Check for various binary types
                        if (column.ValueType == typeof(byte[]) ||
                            column.ValueType.FullName == "System.Data.SqlTypes.SqlBinary" ||
                            column.ValueType.FullName == "System.Data.Linq.Binary" ||
                            column.ValueType.Name.Contains("Binary"))
                        {
                            binaryColumns.Add(column);
                        }
                    }
                }

                // Now process each binary column
                foreach (var column in binaryColumns)
                {
                    // Convert the column to a text column to avoid binary display issues
                    int columnIndex = column.Index;
                    string columnName = column.Name;
                    string headerText = column.HeaderText;
                    string dataPropertyName = column.DataPropertyName;

                    // Store original column properties
                    var displayIndex = column.DisplayIndex;
                    var width = column.Width;
                    var visible = column.Visible;

                    // Remove the binary column
                    GridView.Columns.Remove(column);

                    // Create a new text column (unbound - no DataPropertyName)
                    DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                    textColumn.Name = columnName;
                    textColumn.HeaderText = headerText;
                    // Don't set DataPropertyName - we don't want to bind to the binary data
                    textColumn.DataPropertyName = ""; // Empty = unbound column
                    textColumn.DisplayIndex = displayIndex;
                    textColumn.Width = width;
                    textColumn.Visible = visible;
                    textColumn.ReadOnly = true; // Always readonly for binary data
                    textColumn.DefaultCellStyle.NullValue = "<Binary Data>";
                    textColumn.Tag = "BinaryColumn";

                    // Insert at the same position
                    GridView.Columns.Insert(columnIndex, textColumn);

                    // Set all cell values to placeholder text
                    foreach (DataGridViewRow row in GridView.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            row.Cells[columnIndex].Value = "<Binary Data>";
                        }
                    }
                }
            }
            finally
            {
                GridView.ResumeLayout();
            }
        }

        [Category("Custom Property"), Browsable(true), DefaultValue(null)]
        [Description("The IBindingListView which should be initially displayed.")]
        public Object DataSource
        {
            get
            {
                return GridView.DataSource as IBindingListView;
            }
            set
            {
                bool dataBinded = false;
                IBindingListView aux = null;
                DataSet ds;
                DataTable dt;


                if (!dataBinded)
                {
                    try
                    {
                        ds = (DataSet)value;
                        if (ds != null && ds.Tables.Count > 0)
                            aux = ds.Tables[0].DefaultView;
                        dataBinded = true;
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }

                if (!dataBinded)
                {
                    try
                    {
                        dt = (DataTable)value;
                        aux = dt.DefaultView;
                        dataBinded = true;
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }

                if (!dataBinded)
                {
                    try
                    {
                        aux = (IBindingListView)value;
                        dataBinded = true;
                    }
                    catch (Exception)
                    {
                        GridView.DataSource = null;
                        return;
                    }
                }

                filterExtender.BeginInit();
                GridView.DataSource = aux;
                filterExtender.EndInit();
                if (aux != null)
                    MaxRecordCount = aux.Count;
                else
                    MaxRecordCount = 0;
                SetRowCount();
            }
        }

        /// <summary>
        /// Gets the collection of custom menu items for the header context menu.
        /// These items are configurable in the Visual Studio designer.
        /// </summary>
        [Category("Custom Property"), Description("Custom menu items that appear in the header context menu.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ZidGridMenuItemCollection CustomMenuItems
        {
            get { return _customMenuItems; }
        }

        /// <summary>
        /// Gets the collection of plugins for the header context menu.
        /// Plugins appear at the top of the menu.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ZidGridPluginCollection Plugins
        {
            get { return _plugins; }
        }

        private Font _CellFont;
        [Category("Custom Property"), Browsable(true)]
        [Description("The Font that must be used for the values on the grid")]
        public Font CellFont
        {
            get
            {
                if (_CellFont == null)
                    _CellFont = new Font("Calibri", 9f);
                return _CellFont;
            }
            set
            {
                if (value == null)
                    _CellFont = new Font("Calibri", 9f);
                else
                    _CellFont = value;
            }
        }

        private Font _TitleFont;
        [Category("Custom Property"), Browsable(true)]
        [Description("The Font that must be used for the headers on the grid")]
        public Font TitleFont
        {
            get
            {
                if (_TitleFont == null)
                    _TitleFont = new Font("Calibri", 9.25f, FontStyle.Bold);
                return _TitleFont;
            }
            set
            {
                if (value == null)
                    _TitleFont = new Font("Calibri", 9.25f, FontStyle.Bold);
                else
                    _TitleFont = value;
            }
        }

        #region Code for the theme of the DataGridView
        private ZidThemes _Theme;
        [Category("Theme"), Browsable(true), DefaultValue(ZidThemes.None)]
        [Description("Gets or sets the theme of the control.")]
        public ZidThemes Theme
        {
            get { return _Theme; }
            set
            {
                _Theme = value;
                SetGridTheme(this.GridView, _Theme);
            }
        }

        private bool _EnableAlternatingRows = true;
        [Category("Theme"), Browsable(true), DefaultValue(true)]
        [Description("Enable or disable alternating row colors in the grid")]
        public bool EnableAlternatingRows
        {
            get { return _EnableAlternatingRows; }
            set
            {
                _EnableAlternatingRows = value;
                SetGridTheme(this.GridView, _Theme);
            }
        }
        private ToolStripItemImageScaling _ContextMenuImageScaling = ToolStripItemImageScaling.SizeToFit;
        [Category("Theme"), Browsable(true), DefaultValue(ToolStripItemImageScaling.SizeToFit)]
        [Description("Gets or sets how images are scaled in the context menu")]
        public ToolStripItemImageScaling ContextMenuImageScaling
        {
            get { return _ContextMenuImageScaling; }
            set { _ContextMenuImageScaling = value; }
        }

        private Font _ContextMenuFont = null;
        [Category("Theme"), Browsable(true), DefaultValue(null)]
        [Description("Gets or sets the font for the context menu. If null, uses default menu font.")]
        public Font ContextMenuFont
        {
            get { return _ContextMenuFont; }
            set { _ContextMenuFont = value; }
        }

        private Size _ContextMenuImageSize = new Size(32, 32);
        [Category("Theme"), Browsable(true)]
        [Description("Gets or sets the size of images in the context menu")]
        public Size ContextMenuImageSize
        {
            get { return _ContextMenuImageSize; }
            set { _ContextMenuImageSize = value; }
        }

        [Category("Filtering")]
        [Browsable(true), DefaultValue(FilterPosition.Top)]
        [Description("Gets or sets the position of the filter controls")]
        public FilterPosition FilterBoxPosition
        {
            get { return filterExtender.FilterBoxPosition; }
            set
            {
                filterExtender.FilterBoxPosition = value;
                RepositionGrid();
            }
        }
        /// <summary>
        /// Gets and sets the text for the filter label.
        /// </summary>
        [Category("Filtering")]
        [Browsable(true), DefaultValue("Filter")]
        [Description("Text shown on top of the grid")]
        public string FilterText
        {
            get { return filterExtender.FilterText; }
            set { filterExtender.FilterText = value; }
        }

        [Category("Filtering")]
        [Browsable(true), DefaultValue(true)]
        [Description("Determines if the text on the top of the grid will be visible")]
        public bool FilterTextVisible
        {
            get { return filterExtender.FilterTextVisible; }
            set { filterExtender.FilterTextVisible = value; }
        }

        private void SetGridTheme(DataGridView srcGrid, ZidThemes SelectedTheme)
        {
            DataGridView grid = srcGrid;

            if (grid == null)
                return;
            grid.AlternatingRowsDefaultCellStyle = null;
            grid.SuspendLayout();
            switch (SelectedTheme)
            {
                case ZidThemes.None:
                    grid.EnableHeadersVisualStyles = true;
                    grid.AlternatingRowsDefaultCellStyle = null;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.ColumnHeadersDefaultCellStyle = null;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.RowHeadersDefaultCellStyle = null;
                    grid.DefaultCellStyle = null;
                    grid.RowTemplate = null;
                    grid.BorderStyle = BorderStyle.FixedSingle;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.GridColor = SystemColors.ControlDark;
                    grid.RowHeadersVisible = true;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.CodeProject:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 238);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.BorderStyle = BorderStyle.FixedSingle;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 150, 70);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.White;
                    grid.DefaultCellStyle.ForeColor = Color.Black;
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.GridColor = Color.Orange;
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 150, 70);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.BlackAndWhite:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.FixedSingle;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(216, 216, 216);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.GridColor = Color.Black;
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Blue:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(211, 226, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(79, 129, 189);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(158, 183, 229);
                    grid.GridColor = Color.FromArgb(96, 142, 197);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(79, 129, 189);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Violet:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 223, 245);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 100, 162);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(192, 176, 213);
                    grid.GridColor = Color.FromArgb(152, 128, 181);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 100, 162);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Greenish:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(213, 245, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(75, 172, 198);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(162, 217, 232);
                    grid.GridColor = Color.FromArgb(107, 189, 212);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(75, 172, 198);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.DarkMode:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
                    grid.GridColor = Color.FromArgb(60, 60, 60);
                    grid.DefaultCellStyle.ForeColor = Color.White;
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.FromArgb(30, 30, 30);
                    break;
                case ZidThemes.Ocean:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(176, 224, 230);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 105, 148);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(135, 206, 250);
                    grid.GridColor = Color.FromArgb(70, 130, 180);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 105, 148);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Sunset:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 218, 185);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 160, 122);
                    grid.GridColor = Color.FromArgb(255, 127, 80);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Forest:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(193, 225, 193);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 139, 34);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(144, 238, 144);
                    grid.GridColor = Color.FromArgb(60, 179, 113);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 139, 34);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Rose:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 228, 225);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 112, 147);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 182, 193);
                    grid.GridColor = Color.FromArgb(255, 105, 180);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 112, 147);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Slate:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 250);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(112, 128, 144);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(176, 196, 222);
                    grid.GridColor = Color.FromArgb(119, 136, 153);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(112, 128, 144);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Teal:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(175, 238, 238);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 128, 128);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(64, 224, 208);
                    grid.GridColor = Color.FromArgb(32, 178, 170);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 128, 128);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Amber:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 191, 0);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 120);
                    grid.GridColor = Color.FromArgb(255, 193, 37);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 191, 0);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Crimson:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 228, 225);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(250, 128, 114);
                    grid.GridColor = Color.FromArgb(205, 92, 92);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Indigo:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 250);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(75, 0, 130);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(138, 43, 226);
                    grid.GridColor = Color.FromArgb(106, 90, 205);
                    grid.DefaultCellStyle.ForeColor = Color.White;
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(75, 0, 130);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Emerald:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(209, 255, 219);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 155, 119);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(80, 200, 120);
                    grid.GridColor = Color.FromArgb(46, 184, 92);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 155, 119);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Lavender:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 240, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(147, 112, 219);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(216, 191, 216);
                    grid.GridColor = Color.FromArgb(186, 85, 211);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(147, 112, 219);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Bronze:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 239, 213);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(205, 127, 50);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(222, 184, 135);
                    grid.GridColor = Color.FromArgb(210, 105, 30);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(205, 127, 50);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Navy:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 128);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(173, 216, 230);
                    grid.GridColor = Color.FromArgb(65, 105, 225);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 0, 128);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Mint:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 250);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(62, 180, 137);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(152, 251, 152);
                    grid.GridColor = Color.FromArgb(102, 205, 170);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(62, 180, 137);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Coral:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 127, 80);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 160, 122);
                    grid.GridColor = Color.FromArgb(240, 128, 128);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 127, 80);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Steel:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(220, 220, 220);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(176, 196, 222);
                    grid.GridColor = Color.FromArgb(123, 104, 238);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Gold:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 205);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(218, 165, 32);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 215, 0);
                    grid.GridColor = Color.FromArgb(255, 185, 15);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(218, 165, 32);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Plum:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(142, 69, 133);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(221, 160, 221);
                    grid.GridColor = Color.FromArgb(218, 112, 214);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(142, 69, 133);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
                case ZidThemes.Aqua:
                    grid.EnableHeadersVisualStyles = false;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 255, 255);
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                    grid.AlternatingRowsDefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.BorderStyle = BorderStyle.None;
                    grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 206, 209);
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersHeight = 28;
                    grid.DefaultCellStyle.BackColor = Color.FromArgb(127, 255, 212);
                    grid.GridColor = Color.FromArgb(64, 224, 208);
                    grid.DefaultCellStyle.Font = new Font("Verdana", 9f);
                    grid.RowHeadersVisible = true;
                    grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 206, 209);
                    grid.RowHeadersDefaultCellStyle.Font = new Font("Verdana", 9.25f, FontStyle.Bold);
                    grid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowTemplate.Height = 26;
                    grid.BackgroundColor = Color.White;
                    break;
            }
            grid.TopLeftHeaderCell.Style = grid.ColumnHeadersDefaultCellStyle.Clone();
            grid.TopLeftHeaderCell.Style.Font = new Font("Calibri", 9, FontStyle.Regular);

            // Apply or clear alternating row colors based on EnableAlternatingRows property
            if (!_EnableAlternatingRows)
            {
                grid.AlternatingRowsDefaultCellStyle.BackColor = grid.DefaultCellStyle.BackColor;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = grid.DefaultCellStyle.ForeColor;
            }

            grid.ResumeLayout();
        }
        #endregion
        private void SetRowCount()
        {
            if (GridView.DataSource != null)
            {
                int tt = GridView.RowCount;
                GridView.TopLeftHeaderCell.Value = tt.ToString() + (tt < MaxRecordCount ? "*" : "");
                if (tt == MaxRecordCount)
                {
                    GridView.TopLeftHeaderCell.Style.ForeColor = GridView.ColumnHeadersDefaultCellStyle.ForeColor;
                }
                else
                {
                    GridView.TopLeftHeaderCell.Style.ForeColor = Color.Red;
                }
            }
        }

        public int RecordCount
        {
            get { return GridView == null ? 0 : GridView.Rows.Count; }
        }
        public int FieldCount
        {
            get { return GridView == null ? 0 : GridView.Columns.Count; }
        }
        /// <summary>
        /// Repositions the grid to match the new size
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RepositionGrid();
        }
        private void RepositionGrid()
        {
            int newTop = GridView.Top;
            int newHeight = GridView.Height;
            int newLeft = 0;
            int newWidth = this.Width;
            switch (filterExtender.FilterBoxPosition)
            {
                case FilterPosition.Off:
                    newTop = 0;
                    newHeight = this.Height;
                    break;
                case FilterPosition.Top:
                    newTop = filterExtender.NeededControlHeight + 1;
                    newHeight = this.Height - newTop - 1;
                    break;
                case FilterPosition.Bottom:
                    newTop = 0;
                    newHeight = this.Height - filterExtender.NeededControlHeight - 1;
                    break;
            }

            GridView.SetBounds(newLeft, newTop, newWidth, newHeight, BoundsSpecified.All);
        }
        #region Header Context Menu

        /// <summary>
        /// Handles column header mouse click to show context menu on right-click.
        /// </summary>
        private void GridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0)
            {
                ShowHeaderContextMenu(e.ColumnIndex, Cursor.Position);
            }
        }

        /// <summary>
        /// Builds and shows the header context menu.
        /// </summary>
        private void ShowHeaderContextMenu(int columnIndex, Point location)
        {
            // Check if there are any items to show
            bool hasPlugins = _plugins != null && _plugins.Count > 0;
            bool hasCustomItems = _customMenuItems != null && _customMenuItems.Count > 0;

            if (!hasPlugins && !hasCustomItems)
            {
                // No items to show, don't display the menu
                return;
            }

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Apply custom font if specified
            if (_ContextMenuFont != null)
            {
                contextMenu.Font = _ContextMenuFont;
            }

            // Set image scaling size for better rendering
            contextMenu.ImageScalingSize = _ContextMenuImageSize;

            // Add plugin menu items (top section)
            if (hasPlugins)
            {
                foreach (var plugin in _plugins)
                {
                    ToolStripMenuItem pluginItem = new ToolStripMenuItem();
                    pluginItem.Text = plugin.MenuText;
                    pluginItem.Image = plugin.MenuImage;
                    pluginItem.ImageScaling = _ContextMenuImageScaling;
                    pluginItem.Enabled = plugin.Enabled;
                    pluginItem.Tag = plugin;

                    pluginItem.Click += (s, e) =>
                    {
                        var clickedPlugin = (IZidGridPlugin)((ToolStripMenuItem)s).Tag;
                        var context = new ZidGridPluginContext
                        {
                            DataGridView = this.GridView,
                            DataSource = this.DataSource,
                            ColumnIndex = columnIndex,
                            Column = columnIndex >= 0 && columnIndex < GridView.Columns.Count
                                ? GridView.Columns[columnIndex]
                                : null,
                            Theme = this.Theme
                        };
                        clickedPlugin.Execute(context);
                    };

                    contextMenu.Items.Add(pluginItem);
                }
            }

            // Add separator if both sections have items
            if (hasPlugins && hasCustomItems)
            {
                contextMenu.Items.Add(new ToolStripSeparator());
            }

            // Add custom menu items (bottom section)
            if (hasCustomItems)
            {
                foreach (var customItem in _customMenuItems)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem();
                    menuItem.Text = customItem.Text;
                    menuItem.Image = customItem.Image;
                    menuItem.ImageScaling = _ContextMenuImageScaling;
                    menuItem.Enabled = customItem.Enabled;
                    menuItem.Tag = customItem;

                    menuItem.Click += (s, e) =>
                    {
                        var clickedItem = (ZidGridMenuItem)((ToolStripMenuItem)s).Tag;
                        var eventArgs = new ZidGridMenuItemClickEventArgs
                        {
                            MenuItem = clickedItem,
                            ColumnIndex = columnIndex,
                            Column = columnIndex >= 0 && columnIndex < GridView.Columns.Count
                                ? GridView.Columns[columnIndex]
                                : null,
                            DataGridView = this.GridView,
                            DataSource = this.DataSource
                        };
                        clickedItem.OnClick(eventArgs);
                    };

                    contextMenu.Items.Add(menuItem);
                }
            }

            // Show the context menu
            contextMenu.Show(location);
        }

        #endregion

        
    }
}
