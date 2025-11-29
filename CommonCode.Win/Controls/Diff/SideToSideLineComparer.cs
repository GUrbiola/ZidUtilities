using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ZidUtilities.CommonCode.DifferenceEngine.Implementations;
using ZidUtilities.CommonCode.DifferenceEngine.Engine;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;
using ZidUtilities.CommonCode.ICSharpTextEditor;
using System.Runtime;
using ZidUtilities.CommonCode.Win.Controls.Diff;

namespace ZidUtilities.CommonCode.Win.Controls.Diff
{
    public partial class SideToSideLineComparer : UserControl
    {
        [Category("Custom Properties")]
        [Description("Defines the syntax highlighting that the control will have.")]
        public SyntaxHighlighting Highlighting { get { return Line1.Syntax; } set { Line1.Syntax = Line2.Syntax = value; } }
        [Category("Custom Properties")]
        [Description("Text on the upper text editor.")]
        public string UpperText { get { return Line1.EditorText; } set { Line1.Text = value; } }
        [Category("Custom Properties")]
        [Description("Text on the lower text editor.")]
        public string LowerText { get { return Line2.EditorText; } set { Line2.Text = value; } }
        [Category("Custom Properties")]
        [Description("Sets the control to read only mode.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool IsReadOnly { get { return Line1.IsReadOnly; } set { Line1.IsReadOnly = Line2.IsReadOnly = value; } }

        public SideToSideLineComparer()
        {
            InitializeComponent();

            Line1.Text = "";
            Line2.Text = "";

            Line1.Editor.ActiveTextAreaControl.VScrollBar.Hide();
            Line2.Editor.ActiveTextAreaControl.VScrollBar.Hide();

            Line1.Editor.IsReadOnly = true;
            Line2.Editor.IsReadOnly = true;

            Line1.Editor.ActiveTextAreaControl.HScrollBar.ValueChanged += new EventHandler(Line1HorizontalScrollChanged);
            Line2.Editor.ActiveTextAreaControl.HScrollBar.ValueChanged += new EventHandler(Line2HorizontalScrollChanged);
        }
        
        private void Line1HorizontalScrollChanged(object sender, EventArgs e)
        {
            Line2.Editor.ActiveTextAreaControl.HScrollBar.Value = Line1.Editor.ActiveTextAreaControl.HScrollBar.Value;
            Line2.Editor.ActiveTextAreaControl.Refresh();
        }

        private void Line2HorizontalScrollChanged(object sender, EventArgs e)
        {
            Line1.Editor.ActiveTextAreaControl.HScrollBar.Value = Line2.Editor.ActiveTextAreaControl.HScrollBar.Value;
            Line1.Editor.ActiveTextAreaControl.Refresh();
        }

        public void LoadTexts(string txt1, string txt2)
        {
            Line1.Text = "";
            Line2.Text = "";

            DiffListString t1, t2;

            List<Tuple<char, DiffHighlight>> finalT1 = new List<Tuple<char, DiffHighlight>>();
            List<Tuple<char, DiffHighlight>> finalT2 = new List<Tuple<char, DiffHighlight>>();

            t1 = new DiffListString(txt1);
            t2 = new DiffListString(txt2);

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
                            string h1 = t1.GetByIndex(drs.SourceIndex + i).ToString();
                            string h2 = " ";
                            finalT1.Add(new Tuple<char, DiffHighlight>(h1[0], DiffHighlight.Remove));
                            finalT2.Add(new Tuple<char, DiffHighlight>(h2[0], DiffHighlight.Missing));
                        }
                        break;
                    case DiffResultSpanStatus.NoChange:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            string h1 = t1.GetByIndex(drs.SourceIndex + i).ToString();
                            string h2 = t2.GetByIndex(drs.DestIndex + i).ToString();

                            finalT1.Add(new Tuple<char, DiffHighlight>(h1[0], DiffHighlight.None));
                            finalT2.Add(new Tuple<char, DiffHighlight>(h2[0], DiffHighlight.None));
                        }
                        break;
                    case DiffResultSpanStatus.AddDestination:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            string h1 = " ";
                            string h2 = t2.GetByIndex(drs.DestIndex + i).ToString();
                            finalT1.Add(new Tuple<char, DiffHighlight>(h1[0], DiffHighlight.Missing));
                            finalT2.Add(new Tuple<char, DiffHighlight>(h2[0], DiffHighlight.Add));
                        }

                        break;
                    case DiffResultSpanStatus.Replace:
                        for (int i = 0; i < drs.Length; i++)
                        {
                            string h1 = t1.GetByIndex(drs.SourceIndex + i).ToString();
                            string h2 = t2.GetByIndex(drs.DestIndex + i).ToString();
                            finalT1.Add(new Tuple<char, DiffHighlight>(h1[0], DiffHighlight.Update));
                            finalT2.Add(new Tuple<char, DiffHighlight>(h2[0], DiffHighlight.Update));
                        }
                        break;
                }
            }

            LoadDiffResults(Line1, finalT1);
            LoadDiffResults(Line2, finalT2);

            Line1.Refresh();
            Line2.Refresh();

        }

        private void LoadDiffResults(ExtendedEditor txtEditor, List<Tuple<char, DiffHighlight>> diffResults)
        {
            StringBuilder buff = new StringBuilder();
            foreach (Tuple<char, DiffHighlight> t in diffResults)
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
                        txtEditor.Editor.SetMarker(i, 1, Color.Khaki);
                        break;
                    case DiffHighlight.Missing:
                        txtEditor.Editor.SetMarker(i, 1, Color.Gainsboro);
                        break;
                    case DiffHighlight.None:
                    default:
                        break;
                }
            }
        }

        public void Clean()
        {
            Line1.Text = "";
            Line2.Text = "";

            Line1.Refresh();
            Line2.Refresh();
        }

        private void SideToSideLineComparer_SizeChanged(object sender, EventArgs e)
        {
            Line1.Size = new Size(this.Width, (this.Height - 6) / 2);
            Line2.Size = new Size(this.Width, (this.Height - 6) / 2);
        }
    }
}
