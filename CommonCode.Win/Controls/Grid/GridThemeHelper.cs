using System;
using System.Drawing;

namespace ZidUtilities.CommonCode.Win.Controls.Grid
{
    /// <summary>
    /// Helper class to extract color information from ZidThemes for use in plugin dialogs and forms.
    /// </summary>
    public static class GridThemeHelper
    {
        /// <summary>
        /// Represents the color scheme of a theme.
        /// </summary>
        public class ThemeColors
        {
            public Color HeaderBackColor { get; set; }
            public Color HeaderForeColor { get; set; }
            public Color DefaultBackColor { get; set; }
            public Color DefaultForeColor { get; set; }
            public Color AlternatingBackColor { get; set; }
            public Color GridColor { get; set; }
            public Color BackgroundColor { get; set; }
            public Color AccentColor { get; set; }
            public Font HeaderFont { get; set; }
            public Font CellFont { get; set; }
        }

        /// <summary>
        /// Gets the color scheme for a specified theme.
        /// </summary>
        public static ThemeColors GetThemeColors(ZidThemes theme)
        {
            var colors = new ThemeColors();

            switch (theme)
            {
                case ZidThemes.None:
                    colors.HeaderBackColor = SystemColors.Control;
                    colors.HeaderForeColor = SystemColors.ControlText;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.White;
                    colors.GridColor = SystemColors.ControlDark;
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = SystemColors.Highlight;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Default:
                    colors.HeaderBackColor = Color.FromArgb(52, 152, 219);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(236, 240, 241);
                    colors.GridColor = Color.FromArgb(52, 152, 219);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(52, 152, 219);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Information:
                    colors.HeaderBackColor = Color.FromArgb(41, 128, 185);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(212, 230, 241);
                    colors.GridColor = Color.FromArgb(52, 152, 219);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(52, 152, 219);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Success:
                    colors.HeaderBackColor = Color.FromArgb(39, 174, 96);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(212, 239, 223);
                    colors.GridColor = Color.FromArgb(46, 204, 113);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(46, 204, 113);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Warning:
                    colors.HeaderBackColor = Color.FromArgb(243, 156, 18);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(252, 243, 207);
                    colors.GridColor = Color.FromArgb(241, 196, 15);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(241, 196, 15);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Error:
                    colors.HeaderBackColor = Color.FromArgb(192, 57, 43);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(242, 215, 213);
                    colors.GridColor = Color.FromArgb(231, 76, 60);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(231, 76, 60);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Professional:
                    colors.HeaderBackColor = Color.FromArgb(52, 73, 94);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(236, 240, 241);
                    colors.GridColor = Color.FromArgb(149, 165, 166);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.FromArgb(149, 165, 166);
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.CodeProject:
                    colors.HeaderBackColor = Color.FromArgb(247, 150, 70);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.White;
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 245, 238);
                    colors.GridColor = Color.Orange;
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.Orange;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.BlackAndWhite:
                    colors.HeaderBackColor = Color.Black;
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(216, 216, 216);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.White;
                    colors.GridColor = Color.Black;
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = Color.Black;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Blue:
                    colors.HeaderBackColor = Color.FromArgb(79, 129, 189);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(158, 183, 229);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(211, 226, 255);
                    colors.GridColor = Color.FromArgb(96, 142, 197);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Violet:
                    colors.HeaderBackColor = Color.FromArgb(128, 100, 162);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(192, 176, 213);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(232, 223, 245);
                    colors.GridColor = Color.FromArgb(152, 128, 181);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Greenish:
                    colors.HeaderBackColor = Color.FromArgb(75, 172, 198);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(162, 217, 232);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(213, 245, 255);
                    colors.GridColor = Color.FromArgb(107, 189, 212);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.DarkMode:
                    colors.HeaderBackColor = Color.FromArgb(30, 30, 30);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(37, 37, 38);
                    colors.DefaultForeColor = Color.White;
                    colors.AlternatingBackColor = Color.FromArgb(45, 45, 48);
                    colors.GridColor = Color.FromArgb(60, 60, 60);
                    colors.BackgroundColor = Color.FromArgb(30, 30, 30);
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Ocean:
                    colors.HeaderBackColor = Color.FromArgb(0, 105, 148);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(135, 206, 250);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(176, 224, 230);
                    colors.GridColor = Color.FromArgb(70, 130, 180);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Sunset:
                    colors.HeaderBackColor = Color.FromArgb(220, 20, 60);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(255, 182, 193);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 228, 225);
                    colors.GridColor = Color.FromArgb(255, 99, 71);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Forest:
                    colors.HeaderBackColor = Color.FromArgb(34, 139, 34);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(144, 238, 144);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(240, 255, 240);
                    colors.GridColor = Color.FromArgb(60, 179, 113);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Rose:
                    colors.HeaderBackColor = Color.FromArgb(199, 21, 133);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(255, 182, 193);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 240, 245);
                    colors.GridColor = Color.FromArgb(219, 112, 147);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Slate:
                    colors.HeaderBackColor = Color.FromArgb(47, 79, 79);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(176, 196, 222);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(230, 230, 250);
                    colors.GridColor = Color.FromArgb(112, 128, 144);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Teal:
                    colors.HeaderBackColor = Color.FromArgb(0, 128, 128);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(175, 238, 238);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(224, 255, 255);
                    colors.GridColor = Color.FromArgb(64, 224, 208);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Amber:
                    colors.HeaderBackColor = Color.FromArgb(255, 191, 0);
                    colors.HeaderForeColor = Color.Black;
                    colors.DefaultBackColor = Color.FromArgb(255, 239, 213);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 248, 220);
                    colors.GridColor = Color.FromArgb(255, 215, 0);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Crimson:
                    colors.HeaderBackColor = Color.FromArgb(139, 0, 0);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(240, 128, 128);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 228, 225);
                    colors.GridColor = Color.FromArgb(178, 34, 34);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Indigo:
                    colors.HeaderBackColor = Color.FromArgb(75, 0, 130);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(216, 191, 216);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(230, 230, 250);
                    colors.GridColor = Color.FromArgb(138, 43, 226);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Emerald:
                    colors.HeaderBackColor = Color.FromArgb(0, 128, 0);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(152, 251, 152);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(240, 255, 240);
                    colors.GridColor = Color.FromArgb(46, 139, 87);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Lavender:
                    colors.HeaderBackColor = Color.FromArgb(153, 102, 204);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(230, 230, 250);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(248, 248, 255);
                    colors.GridColor = Color.FromArgb(186, 85, 211);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Bronze:
                    colors.HeaderBackColor = Color.FromArgb(205, 127, 50);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(250, 235, 215);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 248, 220);
                    colors.GridColor = Color.FromArgb(218, 165, 32);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Navy:
                    colors.HeaderBackColor = Color.FromArgb(0, 0, 128);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(173, 216, 230);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(240, 248, 255);
                    colors.GridColor = Color.FromArgb(25, 25, 112);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Mint:
                    colors.HeaderBackColor = Color.FromArgb(60, 179, 113);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(189, 252, 201);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(245, 255, 250);
                    colors.GridColor = Color.FromArgb(102, 205, 170);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Coral:
                    colors.HeaderBackColor = Color.FromArgb(255, 127, 80);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(255, 228, 196);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 245, 238);
                    colors.GridColor = Color.FromArgb(255, 160, 122);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Steel:
                    colors.HeaderBackColor = Color.FromArgb(70, 130, 180);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(176, 196, 222);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(230, 230, 250);
                    colors.GridColor = Color.FromArgb(100, 149, 237);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Gold:
                    colors.HeaderBackColor = Color.FromArgb(255, 215, 0);
                    colors.HeaderForeColor = Color.Black;
                    colors.DefaultBackColor = Color.FromArgb(255, 250, 205);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 255, 224);
                    colors.GridColor = Color.FromArgb(255, 223, 0);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Plum:
                    colors.HeaderBackColor = Color.FromArgb(142, 69, 133);
                    colors.HeaderForeColor = Color.White;
                    colors.DefaultBackColor = Color.FromArgb(221, 160, 221);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(255, 240, 245);
                    colors.GridColor = Color.FromArgb(186, 85, 211);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                case ZidThemes.Aqua:
                    colors.HeaderBackColor = Color.FromArgb(0, 255, 255);
                    colors.HeaderForeColor = Color.Black;
                    colors.DefaultBackColor = Color.FromArgb(224, 255, 255);
                    colors.DefaultForeColor = Color.Black;
                    colors.AlternatingBackColor = Color.FromArgb(240, 255, 255);
                    colors.GridColor = Color.FromArgb(127, 255, 212);
                    colors.BackgroundColor = Color.White;
                    colors.AccentColor = colors.GridColor;
                    colors.HeaderFont = new Font("Verdana", 9.25f, FontStyle.Bold);
                    colors.CellFont = new Font("Verdana", 9f);
                    break;

                default:
                    goto case ZidThemes.None;
            }

            return colors;
        }
        /// <summary>
        /// Directly applies a theme to a ZidGrid control.
        /// </summary>
        /// <param name="grid">ZidGrid control to which the theme will be applied</param>
        /// <param name="theme">Theme to apply</param>
        public static void ApplyTheme(ZidGrid grid, ZidThemes theme)
        {
            grid.Theme = theme;
            grid.Refresh();
        }
    }
}
