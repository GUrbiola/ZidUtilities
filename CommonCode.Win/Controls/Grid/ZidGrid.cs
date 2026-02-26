using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.Grid.GridFilter;
using System.ComponentModel.Design; // keep for attribute reference if needed

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    /// <summary>
    /// A composite WinForms control that wraps a <see cref="DataGridView"/> and adds:
    /// - theming support,
    /// - filtering via an internal filter extender,
    /// - header context menu with plugins and custom menu items,
    /// - utility options such as hide/resize column and toggle filter box,
    /// - safety around problematic bound columns and binary data rendering.
    /// </summary>
    [ToolboxBitmap(@"D:\Just For Fun\ZidUtilities\CommonCode.Win\Controls\Grid\ZidGrid.ico")]
    public partial class ZidGrid : UserControl
    {
        /// <summary>
        /// Maximum number of records available in the underlying data source
        /// when it was first bound. Used to detect if the current number of
        /// rows is less than the original and to flag it on the top-left
        /// header cell (with an asterisk).
        /// </summary>
        private int MaxRecordCount;

        /// <summary>
        /// Collection of designer-configurable context-menu items that appear
        /// in the column-header context menu below plugin items.
        /// </summary>
        private ZidGridMenuItemCollection _customMenuItems;

        /// <summary>
        /// Collection of header context-menu plugins that can execute
        /// custom logic against the grid and its data.
        /// </summary>
        private ZidGridPluginCollection _plugins;

        /// <summary>
        /// Raised when rows are added to the underlying <see cref="DataGridView"/>.
        /// Consumers can subscribe to react to row additions.
        /// </summary>
        public event DataGridViewRowsAddedEventHandler OnRowsAdded;

        /// <summary>
        /// Raised when rows are removed from the underlying <see cref="DataGridView"/>.
        /// Consumers can subscribe to react to row removals.
        /// </summary>
        public event DataGridViewRowsRemovedEventHandler OnRowsRemoved;

        /// <summary>
        /// Raised when the selection within the underlying <see cref="DataGridView"/>
        /// changes. Provides a simplified hook instead of subscribing directly
        /// to <see cref="DataGridView.SelectionChanged"/>.
        /// </summary>
        public event EventHandler OnSelectionChanged;

        /// <summary>
        /// Raised when data binding on the underlying <see cref="DataGridView"/>
        /// is complete. Mirrors the <see cref="DataGridView.DataBindingComplete"/>
        /// event for consumers of <see cref="ZidGrid"/>.
        /// </summary>
        public event DataGridViewBindingCompleteEventHandler OnDataBindComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZidGrid"/> control,
        /// sets up default collections, and wires up internal event handlers
        /// for grid events such as rows added/removed, formatting, and errors.
        /// </summary>
        public ZidGrid()
        {
            InitializeComponent();

            // keep the collection instance but DO NOT add defaults here
            // adding defaults in the ctor causes duplication when designer rehydrates the control
            _customMenuItems = new ZidGridMenuItemCollection();
            _plugins = new ZidGridPluginCollection();

            GridView.RowsAdded += new DataGridViewRowsAddedEventHandler(GridView_RowsAdded);
            GridView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(GridView_RowsRemoved);
            GridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(GridView_DataBindingComplete);
            GridView.CellFormatting += new DataGridViewCellFormattingEventHandler(GridView_CellFormatting);
            GridView.DataError += new DataGridViewDataErrorEventHandler(GridView_DataError);
            GridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(GridView_ColumnHeaderMouseClick);
        }

        /// <summary>
        /// Creates the default header context-menu options used by this grid.
        /// The default options are:
        /// - Hide this column,
        /// - Show/Hide Filter Box,
        /// - Adjust this column's width.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ZidGridMenuItem"/> objects preconfigured with
        /// text, images, and click handlers.
        /// </returns>
        public List<ZidGridMenuItem> GetDefaultMenuOptions()
        {
            List<ZidGridMenuItem> back = new List<ZidGridMenuItem>();
            ZidGridMenuItem hideColumn = new ZidGridMenuItem();
            hideColumn.Name = "HideColumn"; // stable id so we can wire at runtime
            hideColumn.Text = "Hide this column";
            hideColumn.Image = Resources.ColumnHide32;

            hideColumn.Click += HideColumn_Click;
            back.Add(hideColumn);

            ZidGridMenuItem toggleFilter = new ZidGridMenuItem();
            toggleFilter.Name = "ToggleFilter";
            toggleFilter.Text = "Show/Hide Filter Box";
            toggleFilter.Image = Resources.FunnelNew32;
            toggleFilter.Click += ToggleFilter_Click;
            back.Add(toggleFilter);

            ZidGridMenuItem resizeColumn = new ZidGridMenuItem();
            resizeColumn.Name = "ResizeColumn";
            resizeColumn.Text = "Adjust this column's width";
            resizeColumn.Image = Resources.ResizeColumn32;
            resizeColumn.Click += ResizeColumn_Click;
            back.Add(resizeColumn);

            return back;
        }

        /// <summary>
        /// Handles the click of the "Resize this column" header menu item.
        /// Measures the header and visible cell contents of the clicked column
        /// and adjusts its <see cref="DataGridViewColumn.Width"/> so that
        /// the content fits within reasonable bounds.
        /// </summary>
        /// <param name="sender">
        /// The menu item that initiated the event (a <see cref="ZidGridMenuItem"/>).
        /// </param>
        /// <param name="e">
        /// The click event arguments containing the column index and other context.
        /// </param>
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

        /// <summary>
        /// Handles the click of the "Show/Hide Filter Box" header menu item.
        /// Toggles the <see cref="FilterBoxPosition"/> between
        /// <see cref="FilterPosition.Top"/> and <see cref="FilterPosition.Off"/>.
        /// </summary>
        /// <param name="sender">
        /// The menu item that initiated the click.
        /// </param>
        /// <param name="e">
        /// Click event arguments indicating which column header was clicked.
        /// </param>
        private void ToggleFilter_Click(object sender, ZidGridMenuItemClickEventArgs e)
        {
            this.FilterBoxPosition = this.FilterBoxPosition == FilterPosition.Top ? FilterPosition.Off : FilterPosition.Top;
        }

        /// <summary>
        /// Handles the click of the "Hide this column" header menu item.
        /// Sets the <see cref="DataGridViewColumn.Visible"/> property of the
        /// associated column to <c>false</c>.
        /// </summary>
        /// <param name="sender">
        /// The menu item that initiated the click.
        /// </param>
        /// <param name="e">
        /// Click event arguments containing the target column index.
        /// </param>
        private void HideColumn_Click(object sender, ZidGridMenuItemClickEventArgs e)
        {
            GridView.Columns[e.ColumnIndex].Visible = false;
        }

        /// <summary>
        /// Handles the <see cref="DataGridView.RowsRemoved"/> event.
        /// Updates the row count indicator and forwards the event through
        /// <see cref="OnRowsRemoved"/> if any subscribers are attached.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> where rows were removed.
        /// </param>
        /// <param name="e">
        /// Event arguments describing the removed rows.
        /// </param>
        void GridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetRowCount();
            if (OnRowsRemoved != null)
                OnRowsRemoved(sender, e);
        }

        /// <summary>
        /// Handles the <see cref="DataGridView.RowsAdded"/> event.
        /// Updates the row count indicator and forwards the event through
        /// <see cref="OnRowsAdded"/> if any subscribers are attached.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> where rows were added.
        /// </param>
        /// <param name="e">
        /// Event arguments describing the added rows.
        /// </param>
        void GridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SetRowCount();
            if (OnRowsAdded != null)
                OnRowsAdded(sender, e);

        }

        /// <summary>
        /// Handles <see cref="DataGridView.DataBindingComplete"/> to probe
        /// bound columns for display issues. Columns that throw exceptions when
        /// accessing or formatting their values in the first few rows are
        /// considered problematic and are replaced by unbound text columns
        /// with an error indicator.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> whose data binding completed.
        /// </param>
        /// <param name="e">
        /// Event data describing the data-binding operation.
        /// </param>
        void GridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (GridView.Rows.Count == 0 || GridView.Columns.Count == 0)
                return;

            GridView.SuspendLayout();
            try
            {
                var problematicColumns = new List<DataGridViewColumn>();

                // Test each column by trying to access and format values from the first few rows
                foreach (DataGridViewColumn column in GridView.Columns)
                {
                    bool isProblematic = false;
                    int testRows = Math.Min(5, GridView.Rows.Count); // Test first 5 rows or fewer

                    for (int rowIndex = 0; rowIndex < testRows; rowIndex++)
                    {
                        if (GridView.Rows[rowIndex].IsNewRow)
                            continue;

                        try
                        {
                            var cell = GridView.Rows[rowIndex].Cells[column.Index];
                            var value = cell.Value;

                            // Try to get the formatted value - this is where most errors occur
                            if (value != null && value != DBNull.Value)
                            {
                                var formattedValue = cell.FormattedValue;
                                // If we can access both without error, this column is OK
                            }
                        }
                        catch
                        {
                            // This column causes errors when trying to display
                            isProblematic = true;
                            break;
                        }
                    }

                    if (isProblematic)
                    {
                        problematicColumns.Add(column);
                    }
                }

                // Replace problematic columns with text columns showing error message
                foreach (var column in problematicColumns)
                {
                    int columnIndex = column.Index;
                    string columnName = column.Name;
                    string headerText = column.HeaderText;
                    var displayIndex = column.DisplayIndex;
                    var width = column.Width;
                    var visible = column.Visible;

                    // Remove the problematic column
                    GridView.Columns.Remove(column);

                    // Create a replacement text column (unbound)
                    DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                    textColumn.Name = columnName;
                    textColumn.HeaderText = headerText;
                    textColumn.DataPropertyName = ""; // Unbound
                    textColumn.DisplayIndex = displayIndex;
                    textColumn.Width = width;
                    textColumn.Visible = visible;
                    textColumn.ReadOnly = true;
                    textColumn.DefaultCellStyle.ForeColor = Color.Red;

                    // Insert at the same position
                    GridView.Columns.Insert(columnIndex, textColumn);

                    // Set all cell values to error indicator
                    foreach (DataGridViewRow row in GridView.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            row.Cells[columnIndex].Value = "<Display Error>";
                            row.Cells[columnIndex].ToolTipText = "This column contains data that cannot be displayed";
                        }
                    }
                }
            }
            finally
            {
                GridView.ResumeLayout();
            }
        }

        /// <summary>
        /// Handles <see cref="DataGridView.CellFormatting"/> to provide
        /// graceful display of binary data. Byte arrays that are not likely
        /// images are shown as a placeholder string instead of attempting
        /// to render raw binary content.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> raising the event.
        /// </param>
        /// <param name="e">
        /// Formatting event arguments containing the cell value and styling info.
        /// </param>
        void GridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Skip header rows
            if (e.RowIndex < 0)
                return;

            try
            {
                // If the value is null or DBNull, let default handling occur
                if (e.Value == null || e.Value == DBNull.Value)
                    return;

                // Check if this is a byte array (raw binary data, not images)
                if (e.Value is byte[] byteArray)
                {
                    // Try to determine if this might be an image
                    if (IsLikelyImage(byteArray))
                    {
                        // Let DataGridView try to handle it as an image
                        // Most image types will display fine in DataGridView
                        return;
                    }
                    else
                    {
                        // For non-image binary data, show a placeholder
                        e.Value = $"<Binary: {byteArray.Length} bytes>";
                        e.FormattingApplied = true;
                    }
                }
            }
            catch
            {
                // If formatting fails, show error indicator without modifying cell value
                e.Value = "<Error>";
                e.FormattingApplied = true;
            }
        }

        /// <summary>
        /// Handles <see cref="DataGridView.DataError"/> to suppress exceptions
        /// that occur during display or formatting of cell values. This prevents
        /// the application from crashing due to problematic data.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> where the error occurred.
        /// </param>
        /// <param name="e">
        /// Error event arguments detailing the error and cell context.
        /// </param>
        void GridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress the exception from being thrown to the user
            e.ThrowException = false;

            // Note: We don't modify cell.Value here as that would trigger a repaint loop
            // Instead, problematic columns are handled once in DataBindingComplete
        }

        /// <summary>
        /// Checks a byte array for magic numbers of common image types
        /// (PNG, JPEG, GIF, BMP, ICO) to determine whether it likely
        /// represents an image.
        /// </summary>
        /// <param name="data">
        /// The byte array to test.
        /// </param>
        /// <returns>
        /// <c>true</c> if the data matches a known image signature; otherwise <c>false</c>.
        /// </returns>
        private bool IsLikelyImage(byte[] data)
        {
            if (data == null || data.Length < 4)
                return false;

            // Check common image file signatures
            // PNG: 89 50 4E 47
            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                return true;

            // JPEG: FF D8 FF
            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
                return true;

            // GIF: 47 49 46 38
            if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)
                return true;

            // BMP: 42 4D
            if (data[0] == 0x42 && data[1] == 0x4D)
                return true;

            // ICO: 00 00 01 00
            if (data[0] == 0x00 && data[1] == 0x00 && data[2] == 0x01 && data[3] == 0x00)
                return true;

            return false;
        }

        /// <summary>
        /// Gets or sets the data source that should be displayed in the grid.
        /// The control can accept a <see cref="DataSet"/>, <see cref="DataTable"/>,
        /// or an <see cref="IBindingListView"/>. Internally, the first table's
        /// default view or the view itself is bound to the grid and used for
        /// filtering and record counting.
        /// </summary>
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
        /// Gets the collection of header context-menu plugins.
        /// Plugins are displayed at the top of the header context menu
        /// and can perform custom actions against the grid and its data.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ZidGridPluginCollection Plugins
        {
            get { return _plugins; }
        }

        /// <summary>
        /// Gets the collection of custom header context-menu items.
        /// These items are configurable in the designer and appear
        /// after any plugin-provided items in the header context menu.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ZidGridMenuItemCollection CustomMenuItems { get { return _customMenuItems; } }

        /// <summary>
        /// Backing field for <see cref="CellFont"/>.
        /// </summary>
        private Font _CellFont;

        /// <summary>
        /// Gets or sets the font used to display cell values in the grid.
        /// Defaults to Calibri 9pt if not explicitly set or set to <c>null</c>.
        /// </summary>
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

        /// <summary>
        /// Backing field for <see cref="TitleFont"/>.
        /// </summary>
        private Font _TitleFont;

        /// <summary>
        /// Gets or sets the font used for the grid's column headers.
        /// Defaults to bold Calibri 9.25pt if not explicitly set or set to <c>null</c>.
        /// </summary>
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

        /// <summary>
        /// Backing field for <see cref="Theme"/>.
        /// </summary>
        private ZidThemes _Theme;

        /// <summary>
        /// Gets or sets the visual theme applied to the internal
        /// <see cref="DataGridView"/>. Changing this value updates the grid's
        /// colors, borders, fonts, and header styles via <see cref="SetGridTheme"/>.
        /// </summary>
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

        /// <summary>
        /// Backing field for <see cref="EnableAlternatingRows"/>.
        /// </summary>
        private bool _EnableAlternatingRows = true;

        /// <summary>
        /// Gets or sets a value indicating whether alternating row colors are
        /// enabled. When disabled, the alternating row style is made identical
        /// to the default row style.
        /// </summary>
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

        /// <summary>
        /// Backing field for <see cref="ContextMenuImageScaling"/>.
        /// </summary>
        private ToolStripItemImageScaling _ContextMenuImageScaling = ToolStripItemImageScaling.SizeToFit;

        /// <summary>
        /// Gets or sets how images are scaled within items in the header
        /// context menu (plugins and custom items).
        /// </summary>
        [Category("Theme"), Browsable(true), DefaultValue(ToolStripItemImageScaling.SizeToFit)]
        [Description("Gets or sets how images are scaled in the context menu")]
        public ToolStripItemImageScaling ContextMenuImageScaling
        {
            get { return _ContextMenuImageScaling; }
            set { _ContextMenuImageScaling = value; }
        }

        /// <summary>
        /// Backing field for <see cref="ContextMenuFont"/>.
        /// </summary>
        private Font _ContextMenuFont = null;

        /// <summary>
        /// Gets or sets the font used in the header context menu.
        /// If <c>null</c>, the system default menu font is used.
        /// </summary>
        [Category("Theme"), Browsable(true), DefaultValue(null)]
        [Description("Gets or sets the font for the context menu. If null, uses default menu font.")]
        public Font ContextMenuFont
        {
            get { return _ContextMenuFont; }
            set { _ContextMenuFont = value; }
        }

        /// <summary>
        /// Backing field for <see cref="ContextMenuImageSize"/>.
        /// </summary>
        private Size _ContextMenuImageSize = new Size(32, 32);

        /// <summary>
        /// Gets or sets the image size used in the header context menu.
        /// This determines the <see cref="ContextMenuStrip.ImageScalingSize"/>.
        /// </summary>
        [Category("Theme"), Browsable(true)]
        [Description("Gets or sets the size of images in the context menu")]
        public Size ContextMenuImageSize
        {
            get { return _ContextMenuImageSize; }
            set { _ContextMenuImageSize = value; }
        }

        /// <summary>
        /// Gets or sets the vertical placement of the filter UI relative
        /// to the grid: above, below, or turned off.
        /// Changing this property also triggers repositioning of the grid area.
        /// </summary>
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
        /// Gets or sets the label text associated with the filter controls,
        /// usually shown above the grid when filtering is enabled.
        /// </summary>
        [Category("Filtering")]
        [Browsable(true), DefaultValue("Filter")]
        [Description("Text shown on top of the grid")]
        public string FilterText
        {
            get { return filterExtender.FilterText; }
            set { filterExtender.FilterText = value; }
        }

        /// <summary>
        /// Gets or sets whether the filter label text is visible when
        /// filtering controls are displayed.
        /// </summary>
        [Category("Filtering")]
        [Browsable(true), DefaultValue(true)]
        [Description("Determines if the text on the top of the grid will be visible")]
        public bool FilterTextVisible
        {
            get { return filterExtender.FilterTextVisible; }
            set { filterExtender.FilterTextVisible = value; }
        }

        /// <summary>
        /// Gets the embedded <see cref="DataGridView"/> control that actually
        /// displays the data. Consumers can use this property to access grid
        /// members that are not otherwise surfaced by <see cref="ZidGrid"/>.
        /// </summary>
        [Category("Custom Control")]
        [Browsable(true)]
        [Description("Exposes the embedded DataGridView control")]
        public DataGridView GridControl
        {
            get { return GridView; }
        }

        /// <summary>
        /// Gets the first selected row in the underlying <see cref="DataGridView"/>,
        /// or <c>null</c> if no rows are selected.
        /// </summary>
        [Category("Custom Control")]
        [Browsable(false)]
        [Description("Exposes the selected row from the DataGridView")]
        public DataGridViewRow SelectedRow
        {
            get
            {
                if (GridView.SelectedRows != null && GridView.SelectedRows.Count > 0)
                    return GridView.SelectedRows[0];
                return null;
            }
        }

        /// <summary>
        /// Gets all currently selected rows as a list of <see cref="DataGridViewRow"/>
        /// instances or <c>null</c> if there are no selected rows.
        /// </summary>
        [Category("Custom Control")]
        [Browsable(false)]
        [Description("Exposes the selected rows from the DataGridView")]
        public List<DataGridViewRow> SelectedRows
        {
            get
            {
                if (GridView.SelectedRows != null && GridView.SelectedRows.Count > 0)
                {
                    List<DataGridViewRow> rows = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in GridView.SelectedRows)
                    {
                        rows.Add(row);
                    }
                    return rows;
                }
                return null;
            }
        }

        /// <summary>
        /// Applies the specified <see cref="ZidThemes"/> style to a given
        /// <see cref="DataGridView"/>, configuring colors, fonts, borders,
        /// and row/header appearance according to the theme definition.
        /// This also respects <see cref="EnableAlternatingRows"/>.
        /// </summary>
        /// <param name="srcGrid">
        /// The target <see cref="DataGridView"/> to style. If <c>null</c>,
        /// the method returns without making changes.
        /// </param>
        /// <param name="SelectedTheme">
        /// The theme to apply to the grid.
        /// </param>
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

        /// <summary>
        /// Updates the row count indicator in the top-left header cell of the grid.
        /// Shows the current row count and appends an asterisk (*) if the number
        /// of rows is less than the original <see cref="MaxRecordCount"/>.
        /// Also colors the indicator red in that case.
        /// </summary>
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

        /// <summary>
        /// Gets the number of rows currently in the grid, or 0 if the grid
        /// is not initialized.
        /// </summary>
        public int RecordCount
        {
            get { return GridView == null ? 0 : GridView.Rows.Count; }
        }

        /// <summary>
        /// Gets the number of columns currently in the grid, or 0 if the grid
        /// is not initialized.
        /// </summary>
        public int FieldCount
        {
            get { return GridView == null ? 0 : GridView.Columns.Count; }
        }

        /// <summary>
        /// Handles resizing of the control itself and recalculates the
        /// bounds of the internal <see cref="DataGridView"/> so that it
        /// respects the space taken by the filter UI at the top or bottom.
        /// </summary>
        /// <param name="e">
        /// Event arguments describing the resize.
        /// </param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RepositionGrid();
        }

        /// <summary>
        /// Repositions and resizes the internal <see cref="DataGridView"/>
        /// based on the current <see cref="FilterBoxPosition"/> and the
        /// height required by the filter extender.
        /// </summary>
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
        /// Handles mouse clicks on column headers. On right-click, this method
        /// shows the header context menu at the mouse location for the clicked
        /// column.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> whose header was clicked.
        /// </param>
        /// <param name="e">
        /// Mouse event arguments containing the clicked column index and button.
        /// </param>
        private void GridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0)
            {
                ShowHeaderContextMenu(e.ColumnIndex, Cursor.Position);
            }
        }

        /// <summary>
        /// Builds and displays the column-header context menu at the given
        /// screen location for the specified column.
        /// The menu may contain:
        /// - plugin-provided items at the top,
        /// - a separator (if custom items also exist),
        /// - custom designer-configured items.
        /// </summary>
        /// <param name="columnIndex">
        /// Zero-based index of the column for which the menu is shown.
        /// </param>
        /// <param name="location">
        /// Screen coordinates where the context menu should appear.
        /// </param>
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
                    plugin.OnPluginExecuted += (context, pluginName) =>
                    {
                        SetRowCount();
                    };
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

        /// <summary>
        /// Handles the internal <see cref="DataGridView.SelectionChanged"/> event
        /// and forwards it through the <see cref="OnSelectionChanged"/> event
        /// for external subscribers.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> whose selection changed.
        /// </param>
        /// <param name="e">
        /// Event arguments for the selection change.
        /// </param>
        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            if (OnSelectionChanged != null)
                OnSelectionChanged(sender, e);
        }

        /// <summary>
        /// Secondary handler for <see cref="DataGridView.DataBindingComplete"/>
        /// that simply forwards the event via <see cref="OnDataBindComplete"/>
        /// for external subscribers to react to custom logic.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="DataGridView"/> that completed data binding.
        /// </param>
        /// <param name="e">
        /// Event arguments describing the data binding operation.
        /// </param>
        private void GridView_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (OnDataBindComplete != null)
                OnDataBindComplete(sender, e);
        }
    }
}
