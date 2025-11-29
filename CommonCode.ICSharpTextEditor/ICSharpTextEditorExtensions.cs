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
    /// <summary>
    /// A collection of extension helper methods for working with an
    /// <see cref="TextEditorControl"/> (ICSharpCode.TextEditor).
    /// Provides utilities to query caret position, manipulate selection, insert text,
    /// add markers and scroll the view.
    /// </summary>
    public static class ICSharpTextEditorExtensions
    {

        /// <summary>
        /// Gets the 0-based line number that contains the current caret position.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to query.</param>
        /// <returns>
        /// The line number (0-based) of the line that contains the caret.
        /// If the document or caret are unavailable this may throw as per underlying API.
        /// </returns>
        public static int CurrentLineNumber(this TextEditorControl TxtEditor)
        {
            return TxtEditor.Document.GetLineSegmentForOffset(TxtEditor.CurrentOffset()).LineNumber;
        }

        /// <summary>
        /// Gets the current caret offset within the document.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to query.</param>
        /// <returns>
        /// The integer offset (0-based) from the start of the document to the caret position.
        /// This uses the editor's caret position converted to a document offset.
        /// </returns>
        public static int CurrentOffset(this TextEditorControl TxtEditor)
        {
            return TxtEditor.Document.PositionToOffset(TxtEditor.ActiveTextAreaControl.Caret.Position);
        }

        /// <summary>
        /// Retrieves the text of a specific line in the document.
        /// </summary>
        /// <param name="TxtEditor">The text editor control containing the document.</param>
        /// <param name="LineNumber">The 0-based line number to read.</param>
        /// <returns>
        /// The text of the requested line. If the <paramref name="LineNumber"/> is out of range,
        /// an empty string is returned.
        /// </returns>
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

        /// <summary>
        /// Inserts a string into the document at a specified position or at the caret.
        /// If there is a selection and <paramref name="Position"/> is -1, the selection is replaced.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to modify.</param>
        /// <param name="InsStr">The string to insert.</param>
        /// <param name="Position">
        /// The document offset at which to insert the string. If -1, the method will use the
        /// current selection start (if there is a selection) or the caret offset.
        /// </param>
        /// <param name="DoRefreshAfter">
        /// If true, the editor is refreshed after insertion to update the visual state.
        /// </param>
        /// <returns>None.</returns>
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

        /// <summary>
        /// Marks a whole line with a visual text marker of a given color and type.
        /// </summary>
        /// <param name="TxtEditor">The text editor control whose document will be marked.</param>
        /// <param name="LineNumber">The 0-based line number to mark.</param>
        /// <param name="MarkColor">The color to use for the marker.</param>
        /// <param name="MarkType">The kind of marker (defaults to <see cref="TextMarkerType.SolidBlock"/>).</param>
        /// <returns>None.</returns>
        public static void MarkLine(this TextEditorControl TxtEditor, int LineNumber, Color MarkColor, TextMarkerType MarkType = TextMarkerType.SolidBlock)
        {
            LineSegment line = TxtEditor.Document.GetLineSegment(LineNumber);
            TextMarker marker = new TextMarker(line.Offset, line.Length, MarkType, MarkColor);
            TxtEditor.Document.MarkerStrategy.AddMarker(marker);
        }

        /// <summary>
        /// Selects the entire specified line in the editor and scrolls to it.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to operate on.</param>
        /// <param name="LineNumber">The 0-based line number to select. If out of range, selection is cleared.</param>
        /// <returns>None.</returns>
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

        /// <summary>
        /// Selects a range of text given by a document offset and length, moves the caret to the end of the selection,
        /// and scrolls the view so the start of the selection is visible.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to operate on.</param>
        /// <param name="offset">The start offset (0-based) of the selection in the document.</param>
        /// <param name="length">The length of the selection in characters.</param>
        /// <returns>None.</returns>
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

        /// <summary>
        /// Adds a text marker covering a specific offset and length in the document.
        /// </summary>
        /// <param name="TxtEditor">The text editor control whose document will receive the marker.</param>
        /// <param name="Offset">Start offset (0-based) in the document where the marker begins.</param>
        /// <param name="Length">Number of characters the marker should cover.</param>
        /// <param name="MarkColor">Color used for the marker.</param>
        /// <param name="MarkType">Type of marker to add (defaults to <see cref="TextMarkerType.SolidBlock"/>).</param>
        /// <returns>None.</returns>
        public static void SetMarker(this TextEditorControl TxtEditor, int Offset, int Length, Color MarkColor, TextMarkerType MarkType = TextMarkerType.SolidBlock)
        {
            TextMarker marker = new TextMarker(Offset, Length, MarkType, MarkColor);
            TxtEditor.Document.MarkerStrategy.AddMarker(marker);
        }

        /// <summary>
        /// Sets the current selection to cover the entire specified 0-based line.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to operate on.</param>
        /// <param name="lineNumber">The 0-based line number to select.</param>
        /// <returns>None.</returns>
        public static void SetSelectionByLine(this TextEditorControl TxtEditor, int lineNumber)
        {
            LineSegment line = TxtEditor.Document.GetLineSegment(lineNumber);
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(
                new ICSharpCode.TextEditor.TextLocation(0, line.LineNumber),
                new ICSharpCode.TextEditor.TextLocation(line.Length, line.LineNumber)
            );
        }

        /// <summary>
        /// Sets the current selection using two document offsets.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to operate on.</param>
        /// <param name="StartOffset">The start offset (0-based) of the selection.</param>
        /// <param name="EndOffset">The end offset (0-based) of the selection.</param>
        /// <returns>None.</returns>
        public static void SetSelectionByOffset(this TextEditorControl TxtEditor, int StartOffset, int EndOffset)
        {
            TxtEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
            TxtEditor.ActiveTextAreaControl.SelectionManager.SetSelection(TxtEditor.Document.OffsetToPosition(StartOffset), TxtEditor.Document.OffsetToPosition(EndOffset));
        }

        /// <summary>
        /// Sets the first visible physical line in the text editor
        /// </summary>
        /// <param name="editor">The text editor control to scroll.</param>
        /// <param name="lineNumber">The 0-based line number that should become the first visible physical line.</param>
        /// <remarks>
        /// This method attempts to bound-check <paramref name="lineNumber"/> and uses the editor's scrolling APIs.
        /// Exceptions are caught and ignored when scrolling is not possible.
        /// </remarks>
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
        /// Scrolls the editor so the specified line is visible. Optionally centers it within the view.
        /// </summary>
        /// <param name="editor">The text editor control to scroll.</param>
        /// <param name="lineNumber">The 0-based line number to make visible.</param>
        /// <param name="centerInView">
        /// If true, attempts to position the line near the center of the visible area; otherwise,
        /// scrolls so the line is visible at its natural position.
        /// </param>
        /// <remarks>
        /// Exceptions are caught and ignored when scrolling is not possible.
        /// </remarks>
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

        /// <summary>
        /// Attempts to identify the word under the caret and returns its starting offset and length.
        /// </summary>
        /// <param name="TxtEditor">The text editor control to inspect.</param>
        /// <param name="offset">
        /// Output parameter that will contain the start offset (0-based) of the word if found.
        /// On entry this is set to the current caret offset and may be updated on success.
        /// </param>
        /// <param name="length">
        /// Output parameter that will contain the length in characters of the detected word.
        /// Set to 0 on failure.
        /// </param>
        /// <returns>
        /// True if a word was found at the caret position (and <paramref name="offset"/> and
        /// <paramref name="length"/> are populated). False if no word is present or an error occurred.
        /// </returns>
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
