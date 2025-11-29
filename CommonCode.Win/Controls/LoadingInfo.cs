using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ZidUtilities.CommonCode.Win.Controls
{
    public partial class LoadingInfo : UserControl
    {

        [Category("Custom Properties")]
        [Description("Title on the control")]
        public string Title { get { return label1.Text; }  set { label1.Text = value; } }
        [Category("Custom Properties")]
        [Description("Text shown on top of the progressa bar")]
        public string Caption { get { return LDB.Text; } set { LDB.Text = value; } }
        [Category("Custom Properties")]
        [Description("Text below the progress bar")]
        public string Message { get { return LAction.Text; } set { LAction.Text = value; } }
        [Category("Custom Properties")]
        [Description("Image shown on the top left of the control")]
        public Image Icon { get { return ReadingIcon.Image; } set { ReadingIcon.Image = value; } }

        public LoadingInfo()
        {
            InitializeComponent();
            Region = new Region(graphicsPath = CreateRoundRectangle(Width - 1, Height - 1, 6));
        }
        private static GraphicsPath CreateRoundRectangle(int w, int h, int r)
        {
            int d = r << 1;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, d, d), 180, 90);
            path.AddLine(r, 0, w - r, 0);
            path.AddArc(new Rectangle(w - d, 0, d, d), 270, 90);
            path.AddLine(w + 1, r, w + 1, h - r);
            path.AddArc(new Rectangle(w - d, h - d, d, d), 0, 90);
            path.AddLine(w - r, h + 1, r, h + 1);
            path.AddArc(new Rectangle(0, h - d, d, d), 90, 90);
            path.AddLine(0, h - r, 0, r);
            path.CloseFigure();
            return path;
        }
        private GraphicsPath graphicsPath;
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(-1, -1);
            using (Pen p = new Pen(SystemColors.WindowFrame, 2))
            {
                e.Graphics.DrawPath(p, graphicsPath);
            }
            e.Graphics.ResetTransform();
        }
        public void SetInfo(string DB, string Action)
        {
            LDB.Text = DB;
            LAction.Text = Action;
        }
    }
}
