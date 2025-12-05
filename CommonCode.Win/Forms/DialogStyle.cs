using System;
using System.Drawing;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// DEPRECATED: This enum has been replaced by ZidThemes for unified theming across all ZidUtilities controls and forms.
    /// Please use ZidThemes instead for consistent styling.
    /// This enum is kept for backward compatibility but is no longer actively used.
    /// </summary>
    [Obsolete("DialogStyle has been deprecated. Please use ZidThemes instead for unified theming.", false)]
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
    /// Helper class to provide color schemes based on ZidThemes.
    /// Provides backward compatibility for DialogStyle (deprecated).
    /// </summary>
    internal static class DialogStyleHelper
    {
        /// <summary>
        /// Gets the header background color for the specified style.
        /// DEPRECATED: Use GetHeaderColor(ZidThemes) instead.
        /// </summary>
        [Obsolete("Use GetHeaderColor(ZidThemes) instead.", false)]
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
        /// Gets the header background color for the specified theme.
        /// </summary>
        public static Color GetHeaderColor(ZidThemes theme)
        {
            var colors = Controls.Grid.GridThemeHelper.GetThemeColors(theme);
            return colors.HeaderBackColor;
        }

        /// <summary>
        /// Gets the header text color for the specified style.
        /// DEPRECATED: Use GetHeaderTextColor(ZidThemes) instead.
        /// </summary>
        [Obsolete("Use GetHeaderTextColor(ZidThemes) instead.", false)]
        public static Color GetHeaderTextColor(DialogStyle style)
        {
            return Color.White;
        }

        /// <summary>
        /// Gets the header text color for the specified theme.
        /// </summary>
        public static Color GetHeaderTextColor(ZidThemes theme)
        {
            var colors = Controls.Grid.GridThemeHelper.GetThemeColors(theme);
            return colors.HeaderForeColor;
        }

        /// <summary>
        /// Gets the accent color for the specified style.
        /// DEPRECATED: Use GetAccentColor(ZidThemes) instead.
        /// </summary>
        [Obsolete("Use GetAccentColor(ZidThemes) instead.", false)]
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

        /// <summary>
        /// Gets the accent color for the specified theme.
        /// </summary>
        public static Color GetAccentColor(ZidThemes theme)
        {
            var colors = Controls.Grid.GridThemeHelper.GetThemeColors(theme);
            return colors.AccentColor;
        }
    }
}
