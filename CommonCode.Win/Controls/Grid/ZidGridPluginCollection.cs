using System;
using System.Collections.ObjectModel;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    /// <summary>
    /// Collection of IZidGridPlugin objects.
    /// </summary>
    public class ZidGridPluginCollection : Collection<IZidGridPlugin>
    {
        /// <summary>
        /// Initializes a new instance of the ZidGridPluginCollection class.
        /// </summary>
        public ZidGridPluginCollection()
        {
        }
    }
}
