using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.ICSharpTextEditor
{
    public static class Extensions
    {

        public static int CurrentLineNumber(this TextEditorControl TxtEditor)
        {
            return TxtEditor.Document.GetLineSegmentForOffset(TxtEditor.CurrentOffset()).LineNumber;
        }

        public static int CurrentOffset(this TextEditorControl TxtEditor)
        {
            return TxtEditor.Document.PositionToOffset(TxtEditor.ActiveTextAreaControl.Caret.Position);
        }

        public static string GetLineText(this TextEditorControl TxtEditor, int LineNumber)
        {
            LineSegment line;
            ICSharpCode.TextEditor.TextLocation Start;
            string lineText = "";

            if (LineNumber <= TxtEditor.Document.TotalNumberOfLines && LineNumber >= 0)
            {
                line = TxtEditor.Document.GetLineSegment(LineNumber);
                Start = new ICSharpCode.TextEditor.TextLocation(0, line.LineNumber);
                lineText = TxtEditor.Document.GetText(line.Offset, line.Length);
            }

            return lineText;
        }

        public static void InsertString(this TextEditorControl TxtEditor, string InsStr, int Position = -1, bool DoRefreshAfter = true)
        {
            int SelectionLength = 0;
            if (String.IsNullOrEmpty(InsStr))
                return;


            if (Position == -1)
            {
                if (TxtEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {
                    SelectionLength = TxtEditor.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText.Length;
                    Position = TxtEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].Offset;
                    TxtEditor.ActiveTextAreaControl.TextArea.Caret.Position = TxtEditor.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition;
                    TxtEditor.ActiveTextAreaControl.TextArea.SelectionManager.RemoveSelectedText();
                }
                else
                {
                    Position = TxtEditor.CurrentOffset();
                }
            }
            TxtEditor.Document.Insert(Position, InsStr);
            TxtEditor.ActiveTextAreaControl.Caret.Column += InsStr.Length;

            if (DoRefreshAfter)
                TxtEditor.Refresh();
        }

        public static void MarkLine(this TextEditorControl TxtEditor, int LineNumber, Color MarkColor, TextMarkerType MarkType = TextMarkerType.SolidBlock)
        {
            LineSegment line = TxtEditor.Document.GetLineSegment(LineNumber);
            TextMarker marker = new TextMarker(line.Offset, line.Length, MarkType, MarkColor);
            TxtEditor.Document.MarkerStrategy.AddMarker(marker);
        }

        public static void SelectLine(this TextEditorControl TxtEditor, int LineNumber)
        {
            LineSegment Line;
            ICSharpCode.TextEditor.TextLocation Start, End;

            if (LineNumber >= TxtEditor.Document.TotalNumberOfLines)
            {
                TxtEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
                return;
            }

            if (LineNumber >= 0)
            {
                Line = TxtEditor.Document.GetLineSegment(LineNumber);
                Start = new ICSharpCode.TextEditor.TextLocation(0, Line.LineNumber);
                End = new ICSharpCode.TextEditor.TextLocation(Line.Length, Line.LineNumber);
                TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(Start, End);
                TxtEditor.ActiveTextAreaControl.ScrollTo(LineNumber);
            }
            else
            {
                TxtEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
            }
        }

        public static void SelectText(this TextEditorControl TxtEditor, int offset, int length)
        {
            TextLocation p1 = TxtEditor.Document.OffsetToPosition(offset);
            TextLocation p2 = TxtEditor.Document.OffsetToPosition(offset + length);
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            TxtEditor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            // Also move the caret to the end of the selection, because when the user
            // presses F3, the caret is where we start searching next time.
            TxtEditor.ActiveTextAreaControl.Caret.Position = TxtEditor.Document.OffsetToPosition(offset + length);
        }

        public static void SetMarker(this TextEditorControl TxtEditor, int Offset, int Length, Color MarkColor, TextMarkerType MarkType = TextMarkerType.SolidBlock)
        {
            TextMarker marker = new TextMarker(Offset, Length, MarkType, MarkColor);
            TxtEditor.Document.MarkerStrategy.AddMarker(marker);
        }

        public static void SetSelectionByOffset(this TextEditorControl TxtEditor, int StartOffset, int EndOffset)
        {
            TxtEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(TxtEditor.Document.OffsetToPosition(StartOffset), TxtEditor.Document.OffsetToPosition(EndOffset));
        }

    }
}
