using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.ICSharpTextEditor;
using static System.Windows.Forms.LinkLabel;

namespace ZidUtilities.CommonCode.Win.Controls.Diff
{
    [ToolboxBitmap(@"D:\Just For Fun\ZidUtilities\CommonCode.Win\Controls\Diff\DiffInspector.ico")]
    public partial class DiffInspector : UserControl
    {
        [Category("Custom Properties")]
        [Description("Defines the syntax highlighting that the control will have.")]
        public SyntaxHighlighting Highlighting 
        { 
            get 
            { 
                return TextDiff.Highlighting; 
            } 
            set 
            { 
                TextDiff.Highlighting = value;
                LineDiff.Highlighting = value;
            } 
        }
        [Category("Custom Properties")]
        [Description("Text on the left text editor of the text comparer.")]
        public string TextLeftSide { get { return TextDiff.TextLeftSide; } set { TextDiff.TextLeftSide = value; } }
        [Category("Custom Properties")]
        [Description("Text on the right text editor of the text comparer.")]
        public string TextRightSide { get { return TextDiff.TextRightSide; } set { TextDiff.TextRightSide = value; } }
        [Category("Custom Properties")]
        [Description("Text on the upper text editor of the line comparer.")]
        public string UpperText { get { return LineDiff.UpperText; } set { LineDiff.UpperText = value; } }
        [Category("Custom Properties")]
        [Description("Text on the lower text editor of the line comparer.")]
        public string LowerText { get { return LineDiff.LowerText; } set { LineDiff.LowerText = value; } }


        [Category("Custom Properties")]
        [Description("Text on the lower text editor of the line comparer.")]
        public bool ShowEagleView { get { return locationPanel.Visible; } set { locationPanel.Visible = value; } }

        private List<Rectangle> rects;

        public DiffInspector()
        {
            InitializeComponent();

            

            TextDiff.Clean();
            LineDiff.Clean();

            ShowEagleView = true;
        }

        private void sideToSideTextComparer1_OnLineClicked(string TxtLeft, string TxtRight)
        {
            LineDiff.LoadTexts(TxtLeft, TxtRight);
        }

        public void LoadTexts(string txt1, string txt2)
        {
            TextDiff.LoadTexts(txt1, txt2);
            locationPanel.Refresh();
        }

        private void locationPanel_Paint(object sender, PaintEventArgs e)
        {
            if(ShowEagleView)
            {
                if (TextDiff.LoadedDiffsLeft != null && TextDiff.LoadedDiffsRight != null && (TextDiff.LoadedDiffsLeft.Count > 0 || TextDiff.LoadedDiffsRight.Count > 0))
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    rects = new List<Rectangle>();

                    int totalLines = Math.Max(TextDiff.LoadedDiffsLeft.Count, TextDiff.LoadedDiffsRight.Count);
                    int width = locationPanel.Width - 2;
                    int height = locationPanel.Height - 2;
                    int halfWidth = width / 2;

                    // Draw background
                    g.FillRectangle(Brushes.White, 0, 0, locationPanel.Width, locationPanel.Height);

                    for (int lineNumber = 0; lineNumber < totalLines; lineNumber++)
                    {
                        float yPosition = ((float)lineNumber / totalLines) * height;
                        float barHeight = Math.Min(20, Math.Max(2, height / (float)totalLines * 3)); // At least 2 pixels high and no higher than 10

                        float xPositionLeft = 0, xPositionRight = halfWidth;
                        float barWidth = halfWidth;

                        Color leftSide = TextDiff.LoadedDiffsLeft.Count > lineNumber ? TextDiff.LoadedDiffsLeft[lineNumber] : Color.White;
                        Color rightSide = TextDiff.LoadedDiffsRight.Count > lineNumber ? TextDiff.LoadedDiffsRight[lineNumber] : Color.White;

                        rects.Add(new Rectangle((int)xPositionLeft, (int)yPosition, (int)barWidth*2, (int)barHeight));
                        
                        g.FillRectangle(new SolidBrush(leftSide), xPositionLeft, yPosition, barWidth, barHeight);
                        g.FillRectangle(new SolidBrush(rightSide), xPositionRight, yPosition, barWidth, barHeight);
                    }

                    // Draw center divider line
                    using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                    {
                        g.DrawLine(pen, halfWidth, 0, halfWidth, height);
                    }

                    // Draw viewport indicator (shows currently visible portion) - non-interactive
                    DrawViewportIndicator(g, width, height);

                    // Draw border
                    g.DrawRectangle(Pens.LightGray, 0, 0, locationPanel.Width - 1, locationPanel.Height - 1);
                }
            }
        }

        /// <summary>
        /// Draws the viewport indicator showing the currently visible portion (read-only, non-interactive)
        /// </summary>
        private void DrawViewportIndicator(Graphics g, int width, int height)
        {
            try
            {
                if (TextDiff.LoadedDiffsLeft == null || TextDiff.LoadedDiffsRight == null)
                    return;

                // Get current scroll position and visible line count
                int firstVisibleLine = TextDiff.LeftEditor.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine;
                int visibleLineCount = TextDiff.LeftEditor.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount;
                int totalDocumentLines = TextDiff.LeftEditor.Document.TotalNumberOfLines;

                if (totalDocumentLines == 0)
                    return;

                // Calculate viewport position and size
                float startRatio = (float)firstVisibleLine / totalDocumentLines;
                float endRatio = (float)(firstVisibleLine + visibleLineCount) / totalDocumentLines;

                // Clamp to valid range
                startRatio = Math.Max(0, Math.Min(1, startRatio));
                endRatio = Math.Max(0, Math.Min(1, endRatio));

                float viewportY = startRatio * height;
                float viewportHeight = (endRatio - startRatio) * height;

                // Ensure minimum height for visibility
                viewportHeight = Math.Max(10, viewportHeight);

                // Draw semi-transparent viewport indicator (non-interactive)
                using (var brush = new SolidBrush(Color.FromArgb(60, 100, 150, 255)))
                {
                    g.FillRectangle(brush, 1, viewportY, width, viewportHeight);
                }

                // Draw thin border for viewport indicator
                using (var pen = new Pen(Color.FromArgb(150, 50, 100, 200), 1))
                {
                    g.DrawRectangle(pen, 1, viewportY, width - 1, viewportHeight);
                }
            }
            catch
            {
                // Ignore errors in viewport drawing
            }
        }

        private void locationPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if(rects != null && rects.Count > 0)
            {
                for(int i = 0; i < rects.Count; i++)
                {
                    if(rects[i].Contains(e.Location))
                    {
                        TextDiff.LabelTextLeft = $"Clicked on line {i}";
                        TextDiff.LeftEditor.SetFirstPhysicalLineVisible(i - 1);
                        locationPanel.Refresh();
                        break;
                    }
                }
            }
        }
    }
}
