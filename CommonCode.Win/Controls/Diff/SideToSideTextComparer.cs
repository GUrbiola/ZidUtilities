using ICSharpCode.TextEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZidUtilities.CommonCode.DifferenceEngine.Engine;
using ZidUtilities.CommonCode.DifferenceEngine.Implementations;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;
using ZidUtilities.CommonCode.ICSharpTextEditor;
using ZidUtilities.CommonCode.Win.Controls.Diff;
using static System.Windows.Forms.LinkLabel;

namespace Ez_SQL.Custom_Controls
{
    public delegate void LineClicked(string TxtLeft, string TxtRight);
    public partial class SideToSideTextComparer : UserControl
    {
        public event LineClicked OnLineClicked;

        [Category("Custom Properties")]
        [Description("Defines the syntax highlighting that the control will have.")]
        public SyntaxHighlighting Highlighting { get { return LeftText.Syntax; } set { LeftText.Syntax = RightText.Syntax = value; } }
        [Category("Custom Properties")]
        [Description("Text on the upper text editor.")]
        public string TextLeftSide { get { return LeftText.EditorText; } set { LeftText.Text = value; } }
        [Category("Custom Properties")]
        [Description("Text on the lower text editor.")]
        public string TextRightSide { get { return RightText.EditorText; } set { RightText.Text = value; } }
        [Category("Custom Properties")]
        [Description("Label on top of left side text")]
        public string LabelTextLeft
        {
            get { return LabTxt1.Text; }
            set { LabTxt1.Text = value; }
        }
        [Category("Custom Properties")]
        [Description("Label on top of right side text")]
        public string LabelTextRight
        {
            get { return LabTxt2.Text; }
            set { LabTxt2.Text = value; }
        }
        [Category("Custom Properties")]
        [Description("Sets the control to read only mode.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool IsReadOnly { get { return LeftText.IsReadOnly; } set { LeftText.IsReadOnly = RightText.IsReadOnly = value; } }


        public SideToSideTextComparer()
        {
            InitializeComponent();

            LeftText.Text = "";
            RightText.Text = "";

            LeftText.IsReadOnly = true;
            RightText.IsReadOnly = true;

            LeftText.Editor.ActiveTextAreaControl.VScrollBar.ValueChanged += new EventHandler(Txt1VerticalScrollChange);
            RightText.Editor.ActiveTextAreaControl.VScrollBar.ValueChanged += new EventHandler(Txt2VerticalScrollChange);

            LeftText.Editor.ActiveTextAreaControl.HScrollBar.ValueChanged += new EventHandler(Txt1HorizontalScrollChanged);
            RightText.Editor.ActiveTextAreaControl.HScrollBar.ValueChanged += new EventHandler(Txt2HorizontalScrollChanged);

            LeftText.Editor.ActiveTextAreaControl.TextArea.MouseClick += OnMouseClick;
            RightText.Editor.ActiveTextAreaControl.TextArea.MouseClick += OnMouseClick;
            
        }

        private void Txt1HorizontalScrollChanged(object sender, EventArgs e)
        {
            RightText.Editor.ActiveTextAreaControl.HScrollBar.Value = LeftText.Editor.ActiveTextAreaControl.HScrollBar.Value;
            RightText.Editor.ActiveTextAreaControl.Refresh();
        }

        private void Txt2HorizontalScrollChanged(object sender, EventArgs e)
        {
            LeftText.Editor.ActiveTextAreaControl.HScrollBar.Value = RightText.Editor.ActiveTextAreaControl.HScrollBar.Value;
            LeftText.Editor.ActiveTextAreaControl.Refresh();
        }

        private void OnMouseClick(object sender, EventArgs e)
        {
            TextArea ted = sender as TextArea;
            if (ted == null)
                return;
            
            int clickAtLine = ted.Caret.Line;

            LeftText.Editor.SelectLine(clickAtLine);
            RightText.Editor.SelectLine(clickAtLine);

            string txt1 = LeftText.Editor.GetLineText(clickAtLine), txt2 = RightText.Editor.GetLineText(clickAtLine);

            if(OnLineClicked!=null)
                OnLineClicked(txt1, txt2);
        }

        private void Txt1VerticalScrollChange(object sender, EventArgs e)
        {
            RightText.Editor.ActiveTextAreaControl.VScrollBar.Value = LeftText.Editor.ActiveTextAreaControl.VScrollBar.Value;
            RightText.Editor.ActiveTextAreaControl.Refresh();
        }

        private void Txt2VerticalScrollChange(object sender, EventArgs e)
        {
            LeftText.Editor.ActiveTextAreaControl.VScrollBar.Value = RightText.Editor.ActiveTextAreaControl.VScrollBar.Value;
            LeftText.Editor.ActiveTextAreaControl.Refresh();
        }

        public void LoadTexts(string txt1, string txt2)
        {
            DiffListText t1, t2;

            LeftText.Text = "";
            RightText.Text = "";

            List<Tuple<string, DiffHighlight>> finalT1 = new List<Tuple<string, DiffHighlight>>();
            List<Tuple<string, DiffHighlight>> finalT2 = new List<Tuple<string, DiffHighlight>>();

            t1 = new DiffListText(txt1);
            t2 = new DiffListText(txt2);

            DiffEngine Engine = new DiffEngine();
            Engine.ProcessDiff(t1, t2, DiffEngineLevel.SlowPerfect);
            ArrayList rep = Engine.DiffReport();

            foreach (DiffResultSpan drs in rep)
	        {
                switch (drs.Status)
                {
                    case DiffResultSpanStatus.DeleteSource:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            string h1 = (t1.GetByIndex(drs.SourceIndex + i) as TextLine).Line;
                            string h2 = "".PadLeft(h1.Length) + Environment.NewLine;
                            finalT1.Add(new Tuple<string,DiffHighlight>(h1, DiffHighlight.Remove));
                            finalT2.Add(new Tuple<string, DiffHighlight>(h2, DiffHighlight.Missing));
                        }
                        break;
                    case DiffResultSpanStatus.NoChange:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            finalT1.Add(new Tuple<string, DiffHighlight>((t1.GetByIndex(drs.SourceIndex + i) as TextLine).Line, DiffHighlight.None));
                            finalT2.Add(new Tuple<string, DiffHighlight>((t2.GetByIndex(drs.DestIndex + i) as TextLine).Line, DiffHighlight.None));
                        }
                        break;
                    case DiffResultSpanStatus.AddDestination:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            string h1;
                            string h2 = (t2.GetByIndex(drs.DestIndex + i) as TextLine).Line;
                            h1 = "".PadLeft(h2.Length) + Environment.NewLine;
                            finalT1.Add(new Tuple<string, DiffHighlight>(h1, DiffHighlight.Missing));
                            finalT2.Add(new Tuple<string, DiffHighlight>(h2, DiffHighlight.Add));
                        }

                        break;
                    case DiffResultSpanStatus.Replace:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            finalT1.Add(new Tuple<string, DiffHighlight>((t1.GetByIndex(drs.SourceIndex + i) as TextLine).Line, DiffHighlight.Update));
                            finalT2.Add(new Tuple<string, DiffHighlight>((t2.GetByIndex(drs.DestIndex + i) as TextLine).Line, DiffHighlight.Update));
                        }
                        break;
                }
            }

            LoadDiffResults(LeftText, finalT1);
            LoadDiffResults(RightText, finalT2);

            LeftText.Refresh();
            RightText.Refresh();

        }

        private void LoadDiffResults(ExtendedEditor txtEditor, List<Tuple<string, DiffHighlight>> diffResults)
        {
            StringBuilder buff = new StringBuilder();
            foreach (Tuple<string, DiffHighlight> t in diffResults)
            {
                buff.Append(t.Item1);
            }
            txtEditor.Text = buff.ToString();
            for (int i = 0; i < diffResults.Count; i++)
            {
                switch (diffResults[i].Item2)
                {
                    case DiffHighlight.Add:
                    case DiffHighlight.Remove:
                    case DiffHighlight.Update:
                        txtEditor.Editor.MarkLine(i, Color.Khaki);
                        break;
                    case DiffHighlight.Missing:
                        txtEditor.Editor.MarkLine(i, Color.Gainsboro);
                        break;
                    case DiffHighlight.None:
                    default:
                        break;
                }
            }
        }

        public void Clean()
        {
            LeftText.Text = "";
            RightText.Text = "";

            LeftText.Refresh();
            RightText.Refresh();
        }

        private void SideToSideTextComparer_SizeChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = (this.Width - splitContainer1.SplitterWidth) / 2;
        }
    }
    
}
