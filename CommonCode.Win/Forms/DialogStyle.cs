using System;
using System.Drawing;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// Defines color schemes for generic dialog forms.
    /// Each style provides a distinct visual appearance with coordinated colors.
    /// </summary>
    public enum DialogStyle
    {
        /// <summary>
        /// Default style with neutral blue tones
        /// </summary>
        Default,

        /// <summary>
        /// Information style with blue tones
        /// </summary>
        Information,

        /// <summary>
        /// Success style with green tones
        /// </summary>
        Success,

        /// <summary>
        /// Warning style with orange/yellow tones
        /// </summary>
        Warning,

        /// <summary>
        /// Error style with red tones
        /// </summary>
        Error,

        /// <summary>
        /// Professional style with dark gray tones
        /// </summary>
        Professional
    }

    /// <summary>
    /// Helper class to provide color schemes based on DialogStyle.
    /// </summary>
    internal static class DialogStyleHelper
    {
        /// <summary>
        /// Gets the header background color for the specified style.
        /// </summary>
        public static Color GetHeaderColor(DialogStyle style)
        {
            switch (style)
            {
                case DialogStyle.Information:
                    return Color.FromArgb(41, 128, 185);
                case DialogStyle.Success:
                    return Color.FromArgb(39, 174, 96);
                case DialogStyle.Warning:
                    return Color.FromArgb(243, 156, 18);
                case DialogStyle.Error:
                    return Color.FromArgb(192, 57, 43);
                case DialogStyle.Professional:
                    return Color.FromArgb(52, 73, 94);
                case DialogStyle.Default:
                default:
                    return Color.FromArgb(52, 152, 219);
            }
        }

        /// <summary>
        /// Gets the header text color for the specified style.
        /// </summary>
        public static Color GetHeaderTextColor(DialogStyle style)
        {
            return Color.White;
        }

        /// <summary>
        /// Gets the accent color for the specified style.
        /// </summary>
        public static Color GetAccentColor(DialogStyle style)
        {
            switch (style)
            {
                case DialogStyle.Information:
                    return Color.FromArgb(52, 152, 219);
                case DialogStyle.Success:
                    return Color.FromArgb(46, 204, 113);
                case DialogStyle.Warning:
                    return Color.FromArgb(241, 196, 15);
                case DialogStyle.Error:
                    return Color.FromArgb(231, 76, 60);
                case DialogStyle.Professional:
                    return Color.FromArgb(149, 165, 166);
                case DialogStyle.Default:
                default:
                    return Color.FromArgb(52, 152, 219);
            }
        }
    }
}
