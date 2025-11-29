using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.ICSharpTextEditor
{
    public static class ICSharpTextEditorExtensions
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



        public static void SetSelectionByLine(this TextEditorControl TxtEditor, int lineNumber)
        {
            LineSegment line = TxtEditor.Document.GetLineSegment(lineNumber);
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(
                new ICSharpCode.TextEditor.TextLocation(0, line.LineNumber),
                new ICSharpCode.TextEditor.TextLocation(line.Length, line.LineNumber)
            );
        }

        public static void SetSelectionByOffset(this TextEditorControl TxtEditor, int StartOffset, int EndOffset)
        {
            TxtEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(TxtEditor.Document.OffsetToPosition(StartOffset), TxtEditor.Document.OffsetToPosition(EndOffset));
        }

        /// <summary>
        /// Sets the first visible physical line in the text editor
        /// </summary>
        public static void SetFirstPhysicalLineVisible(this TextEditorControl editor, int lineNumber)
        {
            try
            {
                var textArea = editor.ActiveTextAreaControl.TextArea;
                var caret = editor.ActiveTextAreaControl.Caret;

                // Ensure the line number is within valid bounds
                int maxLine = Math.Max(0, editor.Document.TotalNumberOfLines - 1);
                lineNumber = Math.Max(0, Math.Min(lineNumber, maxLine));

                //make sure you are scrolling from the top
                textArea.ScrollTo(1);

                // Calculate the line to center the target line in the view
                int visibleLines = textArea.TextView.VisibleLineCount;
                int targetLine = Math.Max(0, lineNumber + visibleLines);

                // Scroll to the calculated target line
                textArea.ScrollTo(targetLine);

                // Set caret position to the desired line
                caret.Line = lineNumber;
                caret.Column = 0;

                // Update the caret position visually
                caret.UpdateCaretPosition();

                // Force refresh
                editor.ActiveTextAreaControl.TextArea.Invalidate();
                editor.Refresh();
            }
            catch
            {
                // Silently fail if scrolling is not possible
            }
        }

        /// <summary>
        /// Scrolls the editor to make the specified line visible and optionally centered
        /// </summary>
        public static void ScrollToLine(this TextEditorControl editor, int lineNumber, bool centerInView = true)
        {
            try
            {
                var textArea = editor.ActiveTextAreaControl.TextArea;
                var caret = editor.ActiveTextAreaControl.Caret;

                // Ensure the line number is within valid bounds
                int maxLine = Math.Max(0, editor.Document.TotalNumberOfLines - 1);
                lineNumber = Math.Max(0, Math.Min(lineNumber, maxLine));

                if (centerInView)
                {
                    textArea.ScrollTo(1);

                    // Calculate the line to center the target line in the view
                    int visibleLines = textArea.TextView.VisibleLineCount;
                    int targetLine = Math.Max(0, lineNumber - (visibleLines / 2));

                    // Scroll to the calculated target line
                    textArea.ScrollTo(targetLine);
                }
                else
                {
                    textArea.ScrollTo(lineNumber);
                }

                // Set caret position to the desired line
                caret.Line = lineNumber;
                caret.Column = 0;

                // Update the caret position visually
                caret.UpdateCaretPosition();

                // Force refresh
                editor.ActiveTextAreaControl.TextArea.Invalidate();
                editor.Refresh();
            }
            catch
            {
                // Silently fail
            }
        }

        public static bool TryGetCurrentWord(this TextEditorControl TxtEditor, out int offset, out int length)
        {
            offset = TxtEditor.CurrentOffset();
            length = 0;

            try
            {
                var doc = TxtEditor.Document;
                int textLength = doc.TextLength; // total number of characters in document

                // If caret is at or beyond end of doc or invalid offset, nothing to do
                if (offset < 0 || offset >= textLength)
                    return false;

                List<char> wordBreakers = new List<char>() 
                { ' ', '\t', '\r', '\n', '.', ';', ':', '+', '-', '*', '/', '(', ')', '{', '}', '[', ']', '>', '<', '=' };


                char currentChar = doc.GetCharAt(offset);
                // If current char is a word breaker, consider there is no word at caret
                if (wordBreakers.Contains(currentChar))
                    return false;

                // Find start of word
                int start = offset;
                while (start > 0)
                {
                    char prev = doc.GetCharAt(start - 1);
                    if (wordBreakers.Contains(prev))
                        break;
                    start--;
                }

                // Find end of word
                int end = offset;
                while (end < textLength - 1)
                {
                    char next = doc.GetCharAt(end + 1);
                    if (wordBreakers.Contains(next))
                        break;
                    end++;
                }

                offset = start;
                length = end - start + 1;
                return true;
            }
            catch
            {
                // On error, return false and leave offset/length as set (length = 0)
                length = 0;
                return false;
            }
        }

    }
}
