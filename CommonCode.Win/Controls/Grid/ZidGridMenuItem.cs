using System;
using System.ComponentModel;
using System.Drawing;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    /// <summary>
    /// Represents a custom menu item for ZidGrid header context menu.
    /// This class is designer-configurable.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ZidGridMenuItem
    {
        private string _text = "Menu Item";
        private Image _image = null;
        private bool _enabled = true;
        private string _name = "";

        /// <summary>
        /// Gets or sets the display text for the menu item.
        /// </summary>
        [Category("Appearance")]
        [Description("The text to display in the menu item.")]
        [DefaultValue("Menu Item")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets the image/icon for the menu item.
        /// </summary>
        [Category("Appearance")]
        [Description("The image to display in the menu item.")]
        [DefaultValue(null)]
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        /// <summary>
        /// Gets or sets whether the menu item is enabled.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines whether the menu item is enabled.")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// Gets or sets the unique name for this menu item.
        /// Used to identify which menu item was clicked in the event handler.
        /// </summary>
        [Category("Design")]
        [Description("The unique name for this menu item. Used to identify the item in event handlers.")]
        [DefaultValue("")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Event that fires when the menu item is clicked.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the menu item is clicked.")]
        public event EventHandler<ZidGridMenuItemClickEventArgs> Click;

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        internal void OnClick(ZidGridMenuItemClickEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(_name) ? _text : _name;
        }
    }

    /// <summary>
    /// Event arguments for ZidGridMenuItem click events.
    /// </summary>
    public class ZidGridMenuItemClickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the menu item that was clicked.
        /// </summary>
        public ZidGridMenuItem MenuItem { get; internal set; }

        /// <summary>
        /// Gets the column index where the menu was triggered.
        /// </summary>
        public int ColumnIndex { get; internal set; }

        /// <summary>
        /// Gets the column where the menu was triggered.
        /// </summary>
        public System.Windows.Forms.DataGridViewColumn Column { get; internal set; }

        /// <summary>
        /// Gets the DataGridView.
        /// </summary>
        public System.Windows.Forms.DataGridView DataGridView { get; internal set; }

        /// <summary>
        /// Gets the current data source.
        /// </summary>
        public object DataSource { get; internal set; }
    }
}
