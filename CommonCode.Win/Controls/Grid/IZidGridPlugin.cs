using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    public delegate void PluginExecuted(ZidGridPluginContext context, string PluginName);
    /// <summary>
    /// Interface for ZidGrid plugins that appear as menu options in the header context menu.
    /// </summary>
    public interface IZidGridPlugin
    {
        event PluginExecuted OnPluginExecuted;
        /// <summary>
        /// Gets the display text for the menu item.
        /// </summary>
        string MenuText { get; }

        /// <summary>
        /// Gets the image/icon for the menu item (optional, can be null).
        /// </summary>
        Image MenuImage { get; }

        /// <summary>
        /// Gets whether the menu item is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Executes the plugin functionality.
        /// </summary>
        /// <param name="context">The plugin execution context containing grid and data source references.</param>
        void Execute(ZidGridPluginContext context);
    }

    /// <summary>
    /// Context information passed to plugins during execution.
    /// </summary>
    public class ZidGridPluginContext
    {
        /// <summary>
        /// Gets the DataGridView within the ZidGrid.
        /// </summary>
        public DataGridView DataGridView { get; internal set; }

        /// <summary>
        /// Gets the current data source of the grid.
        /// </summary>
        public object DataSource { get; internal set; }

        /// <summary>
        /// Gets the column index where the context menu was triggered (if applicable).
        /// </summary>
        public int ColumnIndex { get; internal set; }

        /// <summary>
        /// Gets the column header where the context menu was triggered (if applicable).
        /// </summary>
        public DataGridViewColumn Column { get; internal set; }

        /// <summary>
        /// Gets the current grid theme applied to the ZidGrid.
        /// </summary>
        public ZidThemes Theme { get; internal set; }
    }
}
