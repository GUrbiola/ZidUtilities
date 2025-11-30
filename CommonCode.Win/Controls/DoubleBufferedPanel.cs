using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls
{
    /// <summary>
    /// A Windows Forms `Panel` derivative that enables double-buffered painting to reduce flicker.
    /// </summary>
    /// <remarks>
    /// This control sets `ControlStyles.AllPaintingInWmPaint` and
    /// `ControlStyles.OptimizedDoubleBuffer` to true and calls `UpdateStyles()`
    /// so that custom painting operations render to an off-screen buffer before being
    /// presented on-screen. Use this control when performing custom drawing to reduce
    /// visible flicker on .NET Framework WinForms applications.
    /// </remarks>
    public class DoubleBufferedPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the `DoubleBufferedPanel` class.
        /// </summary>
        /// <remarks>
        /// The constructor configures the control for double-buffered painting:
        /// - Calls `SetStyle(ControlStyles.AllPaintingInWmPaint, true)` to ensure painting is done in WM_PAINT.
        /// - Calls `SetStyle(ControlStyles.OptimizedDoubleBuffer, true)` to enable an off-screen buffer.
        /// - Calls `UpdateStyles()` to apply the style changes immediately.
        ///
        /// Parameters: none.
        /// Return value: none (constructors do not return a value).
        /// </remarks>
        public DoubleBufferedPanel()
        {

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

        }
    }
}
