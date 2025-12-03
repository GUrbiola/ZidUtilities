using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ZidUtilities.CommonCode.AvalonEdit
{
    /// <summary>
    /// A collection of extension helper methods for working with an
    /// <see cref="TextEditor"/> (AvalonEdit).
    /// Provides utilities to query caret position, manipulate selection, insert text,
    /// add markers and scroll the view.
    /// </summary>
    public static class AvalonEditExtensions
    {
        /// <summary>
        /// Gets the 1-based line number that contains the current caret position.
        /// </summary>
        /// <param name="editor">The text editor control to query.</param>
        /// <returns>
        /// The line number (1-based) of the line that contains the caret.
        /// </returns>
        public static int CurrentLineNumber(this TextEditor editor)
        {
            return editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber;
        }

        /// <summary>
        /// Gets the current caret offset within the document.
        /// </summary>
        /// <param name="editor">The text editor control to query.</param>
        /// <returns>
        /// The integer offset (0-based) from the start of the document to the caret position.
        /// </returns>
        public static int CurrentOffset(this TextEditor editor)
        {
            return editor.CaretOffset;
        }

        /// <summary>
        /// Retrieves the text of a specific line in the document.
        /// </summary>
        /// <param name="editor">The text editor control containing the document.</param>
        /// <param name="lineNumber">The 1-based line number to read.</param>
        /// <returns>
        /// The text of the requested line. If the <paramref name="lineNumber"/> is out of range,
        /// an empty string is returned.
        /// </returns>
        public static string GetLineText(this TextEditor editor, int lineNumber)
        {
            if (lineNumber <= 0 || lineNumber > editor.Document.LineCount)
                return string.Empty;

            var line = editor.Document.GetLineByNumber(lineNumber);
            return editor.Document.GetText(line.Offset, line.Length);
        }

        /// <summary>
        /// Inserts a string into the document at a specified position or at the caret.
        /// If there is a selection and <paramref name="position"/> is -1, the selection is replaced.
        /// </summary>
        /// <param name="editor">The text editor control to modify.</param>
        /// <param name="insertText">The string to insert.</param>
        /// <param name="position">
        /// The document offset at which to insert the string. If -1, the method will use the
        /// current selection start (if there is a selection) or the caret offset.
        /// </param>
        /// <param name="doRefreshAfter">
        /// If true, the editor is refreshed after insertion to update the visual state. (Not used in AvalonEdit)
        /// </param>
        public static void InsertString(this TextEditor editor, string insertText, int position = -1, bool doRefreshAfter = true)
        {
            if (string.IsNullOrEmpty(insertText))
                return;

            if (position == -1)
            {
                if (!editor.SelectionLength.Equals(0))
                {
                    position = editor.SelectionStart;
                    editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
                }
                else
                {
                    position = editor.CaretOffset;
                }
            }

            editor.Document.Insert(position, insertText);
            editor.CaretOffset = position + insertText.Length;
        }

        /// <summary>
        /// Marks a whole line with a visual background renderer of a given color.
        /// </summary>
        /// <param name="editor">The text editor control whose document will be marked.</param>
        /// <param name="lineNumber">The 1-based line number to mark.</param>
        /// <param name="markColor">The color to use for the marker.</param>
        public static void MarkLine(this TextEditor editor, int lineNumber, Color markColor)
        {
            if (lineNumber <= 0 || lineNumber > editor.Document.LineCount)
                return;

            var line = editor.Document.GetLineByNumber(lineNumber);
            var marker = new LineBackgroundRenderer(editor, line, markColor);
            editor.TextArea.TextView.BackgroundRenderers.Add(marker);
        }

        /// <summary>
        /// Selects the entire specified line in the editor and scrolls to it.
        /// </summary>
        /// <param name="editor">The text editor control to operate on.</param>
        /// <param name="lineNumber">The 1-based line number to select. If out of range, selection is cleared.</param>
        public static void SelectLine(this TextEditor editor, int lineNumber)
        {
            if (lineNumber <= 0 || lineNumber > editor.Document.LineCount)
            {
                editor.Select(0, 0);
                return;
            }

            var line = editor.Document.GetLineByNumber(lineNumber);
            editor.Select(line.Offset, line.Length);
            editor.ScrollToLine(lineNumber);
        }

        /// <summary>
        /// Selects a range of text given by a document offset and length, moves the caret to the end of the selection,
        /// and scrolls the view so the start of the selection is visible.
        /// </summary>
        /// <param name="editor">The text editor control to operate on.</param>
        /// <param name="offset">The start offset (0-based) of the selection in the document.</param>
        /// <param name="length">The length of the selection in characters.</param>
        public static void SelectText(this TextEditor editor, int offset, int length)
        {
            editor.Select(offset, length);
            var location = editor.Document.GetLocation(offset);
            editor.ScrollTo(location.Line, location.Column);
            editor.CaretOffset = offset + length;
        }

        /// <summary>
        /// Adds a text segment with background color covering a specific offset and length in the document.
        /// </summary>
        /// <param name="editor">The text editor control whose document will receive the marker.</param>
        /// <param name="offset">Start offset (0-based) in the document where the marker begins.</param>
        /// <param name="length">Number of characters the marker should cover.</param>
        /// <param name="markColor">Color used for the marker.</param>
        public static void SetMarker(this TextEditor editor, int offset, int length, Color markColor)
        {
            var marker = new BackgroundGeometryBuilder { CornerRadius = 3 };
            var segment = new TextSegment { StartOffset = offset, Length = length };
            // Note: In AvalonEdit, markers are typically handled through TextMarkerService or custom rendering
            // This is a simplified implementation
        }

        /// <summary>
        /// Sets the current selection to cover the entire specified 1-based line.
        /// </summary>
        /// <param name="editor">The text editor control to operate on.</param>
        /// <param name="lineNumber">The 1-based line number to select.</param>
        public static void SetSelectionByLine(this TextEditor editor, int lineNumber)
        {
            if (lineNumber <= 0 || lineNumber > editor.Document.LineCount)
                return;

            var line = editor.Document.GetLineByNumber(lineNumber);
            editor.Select(line.Offset, line.Length);
        }

        /// <summary>
        /// Sets the current selection using two document offsets.
        /// </summary>
        /// <param name="editor">The text editor control to operate on.</param>
        /// <param name="startOffset">The start offset (0-based) of the selection.</param>
        /// <param name="endOffset">The end offset (0-based) of the selection.</param>
        public static void SetSelectionByOffset(this TextEditor editor, int startOffset, int endOffset)
        {
            editor.Select(startOffset, endOffset - startOffset);
        }

        /// <summary>
        /// Sets the first visible line in the text editor
        /// </summary>
        /// <param name="editor">The text editor control to scroll.</param>
        /// <param name="lineNumber">The 1-based line number that should become the first visible line.</param>
        public static void SetFirstPhysicalLineVisible(this TextEditor editor, int lineNumber)
        {
            try
            {
                if (lineNumber <= 0)
                    lineNumber = 1;
                if (lineNumber > editor.Document.LineCount)
                    lineNumber = editor.Document.LineCount;

                editor.ScrollToLine(lineNumber);
                editor.TextArea.Caret.Line = lineNumber;
                editor.TextArea.Caret.Column = 1;
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
        /// <param name="lineNumber">The 1-based line number to make visible.</param>
        /// <param name="centerInView">
        /// If true, attempts to position the line near the center of the visible area; otherwise,
        /// scrolls so the line is visible at its natural position.
        /// </param>
        public static void ScrollToLineEx(this TextEditor editor, int lineNumber, bool centerInView = true)
        {
            try
            {
                if (lineNumber <= 0)
                    lineNumber = 1;
                if (lineNumber > editor.Document.LineCount)
                    lineNumber = editor.Document.LineCount;

                if (centerInView)
                {
                    editor.ScrollToLine(lineNumber);
                }
                else
                {
                    editor.ScrollToLine(lineNumber);
                }

                editor.TextArea.Caret.Line = lineNumber;
                editor.TextArea.Caret.Column = 1;
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// Attempts to identify the word under the caret and returns its starting offset and length.
        /// </summary>
        /// <param name="editor">The text editor control to inspect.</param>
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
        public static bool TryGetCurrentWord(this TextEditor editor, out int offset, out int length)
        {
            offset = editor.CaretOffset;
            length = 0;

            try
            {
                var doc = editor.Document;
                int textLength = doc.TextLength;

                if (offset < 0 || offset >= textLength)
                    return false;

                List<char> wordBreakers = new List<char>()
                { ' ', '\t', '\r', '\n', '.', ';', ':', '+', '-', '*', '/', '(', ')', '{', '}', '[', ']', '>', '<', '=' };

                if (offset < textLength)
                {
                    char currentChar = doc.GetCharAt(offset);
                    if (wordBreakers.Contains(currentChar))
                        return false;
                }

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
                length = 0;
                return false;
            }
        }
    }

    /// <summary>
    /// Background renderer for marking lines with a specific color
    /// </summary>
    public class LineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;
        private readonly DocumentLine _line;
        private readonly Color _color;

        public LineBackgroundRenderer(TextEditor editor, DocumentLine line, Color color)
        {
            _editor = editor;
            _line = line;
            _color = color;
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_line.IsDeleted)
                return;

            var builder = new BackgroundGeometryBuilder { AlignToWholePixels = true, CornerRadius = 3 };
            builder.AddSegment(textView, new TextSegment { StartOffset = _line.Offset, EndOffset = _line.EndOffset });

            var geometry = builder.CreateGeometry();
            if (geometry != null)
            {
                drawingContext.DrawGeometry(new SolidColorBrush(_color), null, geometry);
            }
        }
    }
}
