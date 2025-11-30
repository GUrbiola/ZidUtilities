using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    /// <summary>
    /// Collection of ZidGridMenuItem objects for designer support.
    /// </summary>
    [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public class ZidGridMenuItemCollection : Collection<ZidGridMenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the ZidGridMenuItemCollection class.
        /// </summary>
        public ZidGridMenuItemCollection()
        {
        }
    }
}
