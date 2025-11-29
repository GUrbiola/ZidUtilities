using System;
using System.Drawing;
using ICSharpCode.TextEditor.Document;
using System.Diagnostics;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms  
{
    /// <summary>
    /// Provides search functionality over an <see cref="IDocument"/> similar to a "Find" operation.
    /// Maintains an optional scan region (as a <see cref="TextMarker"/>) that adjusts automatically with document changes.
    /// </summary>
    public class TextEditorSearcher : IDisposable
    {
        IDocument _document;
        /// <summary>
        /// Gets or sets the target document to search.
        /// Setting a different document clears any existing scan region.
        /// </summary>
        public IDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                if (_document != value)
                {
                    ClearScanRegion();
                    _document = value;
                }
            }
        }

        // I would have used the TextAnchor class to represent the beginning and 
        // end of the region to scan while automatically adjusting to changes in 
        // the document--but for some reason it is sealed and its constructor is 
        // internal. Instead I use a TextMarker, which is perhaps even better as 
        // it gives me the opportunity to highlight the region. Note that all the 
        // markers and coloring information is associated with the text document, 
        // not the editor control, so TextEditorSearcher doesn't need a reference 
        // to the TextEditorControl. After adding the marker to the document, we
        // must remember to remove it when it is no longer needed.
        TextMarker _region = null;
        /// <summary>
        /// Sets the region to search from a selection object.
        /// The region is updated automatically as the document changes.
        /// </summary>
        /// <param name="sel">Selection whose offset and length define the scan region.</param>
        public void SetScanRegion(ISelection sel)
        {
            SetScanRegion(sel.Offset, sel.Length);
        }
        /// <summary>
        /// Sets the region to search by offset and length.
        /// The region is updated automatically as the document changes.
        /// </summary>
        /// <param name="offset">Starting offset of the scan region.</param>
        /// <param name="length">Length (number of characters) of the scan region.</param>
        public void SetScanRegion(int offset, int length)
        {
            var bkgColor = _document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            _region = new TextMarker(offset, length, TextMarkerType.SolidBlock,
                bkgColor.HalfMix(Color.FromArgb(160, 160, 160)));
            _document.MarkerStrategy.AddMarker(_region);
        }
        /// <summary>
        /// Gets a value indicating whether a scan region is currently set.
        /// </summary>
        public bool HasScanRegion
        {
            get { return _region != null; }
        }
        /// <summary>
        /// Clears any previously set scan region, removing its marker from the document.
        /// </summary>
        public void ClearScanRegion()
        {
            if (_region != null)
            {
                _document.MarkerStrategy.RemoveMarker(_region);
                _region = null;
            }
        }
        /// <summary>
        /// Disposes the searcher, clearing any scan region and preventing finalizer actions.
        /// </summary>
        public void Dispose() { ClearScanRegion(); GC.SuppressFinalize(this); }
        /// <summary>
        /// Finalizer to ensure resources are released if Dispose was not called.
        /// </summary>
        ~TextEditorSearcher() { Dispose(); }
        /// <summary>
        /// Gets the beginning offset of the scan region, or 0 if no region is set.
        /// </summary>
        public int BeginOffset
        {
            get
            {
                if (_region != null)
                    return _region.Offset;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Gets the end offset (one past the last index) of the scan region, or the document's text length if no region is set.
        /// </summary>
        public int EndOffset
        {
            get
            {
                if (_region != null)
                    return _region.EndOffset;
                else
                    return _document.TextLength;
            }
        }
        /// <summary>
        /// When true, searches are case-sensitive.
        /// </summary>
        public bool MatchCase;
        /// <summary>
        /// When true, only whole-word matches are considered valid.
        /// </summary>
        public bool MatchWholeWordOnly;
        string _lookFor;
        string _lookFor2; // uppercase in case-insensitive mode
        /// <summary>
        /// The string to search for. Set this before invoking <see cref="FindNext"/>.
        /// </summary>
        public string LookFor
        {
            get { return _lookFor; }
            set { _lookFor = value; }
        }
        /// <summary>
        /// Finds the next instance of <see cref="LookFor"/> in the document according to search rules.
        /// </summary>
        /// <param name="beginAtOffset">Offset in the document at which to begin the search. If a match exists at this offset it will be returned.</param>
        /// <param name="searchBackward">If true, searches backward from <paramref name="beginAtOffset"/>; otherwise searches forward.</param>
        /// <param name="loopedAround">Output parameter set to true when the search wrapped around the search region.</param>
        /// <returns>
        /// A <see cref="TextRange"/> representing the matched region, or null if no match was found inside the scan region.
        /// </returns>
        /// <remarks>
        /// The search is constrained within the current scan region if one is set; otherwise it spans the whole document.
        /// The search honors <see cref="MatchCase"/> and <see cref="MatchWholeWordOnly"/>.
        /// </remarks>
        public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
        {
            Debug.Assert(!string.IsNullOrEmpty(_lookFor));
            loopedAround = false;

            int startAt = BeginOffset, endAt = EndOffset;
            int curOffs = beginAtOffset.InRange(startAt, endAt);

            _lookFor2 = MatchCase ? _lookFor : _lookFor.ToUpperInvariant();

            TextRange result;
            if (searchBackward)
            {
                result = FindNextIn(startAt, curOffs, true);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(curOffs, endAt, true);
                }
            }
            else
            {
                result = FindNextIn(curOffs, endAt, false);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(startAt, curOffs, false);
                }
            }
            return result;
        }
        /// <summary>
        /// Searches for the next occurrence of <see cref="LookFor"/> inside the interval [offset1, offset2).
        /// </summary>
        /// <param name="offset1">Inclusive start offset of the search interval.</param>
        /// <param name="offset2">Exclusive end offset of the search interval.</param>
        /// <param name="searchBackward">If true, searches from <paramref name="offset2"/> down to <paramref name="offset1"/>; otherwise from <paramref name="offset1"/> up to <paramref name="offset2"/>.</param>
        /// <returns>
        /// A <see cref="TextRange"/> for the match found within the interval, or null if none was found.
        /// </returns>
        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
        {
            Debug.Assert(offset2 >= offset1);
            offset2 -= _lookFor.Length;

            // Make behavior decisions before starting search loop
            Func<char, char, bool> matchFirstCh;
            Func<int, bool> matchWord;
            if (MatchCase)
                matchFirstCh = (lookFor, c) => (lookFor == c);
            else
                matchFirstCh = (lookFor, c) => (lookFor == Char.ToUpperInvariant(c));
            if (MatchWholeWordOnly)
                matchWord = IsWholeWordMatch;
            else
                matchWord = IsPartWordMatch;

            // Search
            char lookForCh = _lookFor2[0];
            if (searchBackward)
            {
                for (int offset = offset2; offset >= offset1; offset--)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset)) && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            else
            {
                for (int offset = offset1; offset <= offset2; offset++)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset)) && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            return null;
        }
        /// <summary>
        /// Determines whether the match beginning at <paramref name="offset"/> is a whole-word match.
        /// </summary>
        /// <param name="offset">Offset where the potential match starts.</param>
        /// <returns>True if the match at <paramref name="offset"/> is a whole-word match; otherwise false.</returns>
        private bool IsWholeWordMatch(int offset)
        {
            if (IsWordBoundary(offset) && IsWordBoundary(offset + _lookFor.Length))
                return IsPartWordMatch(offset);
            else
                return false;
        }
        /// <summary>
        /// Determines whether the specified offset is at a word boundary (start or end of a word).
        /// </summary>
        /// <param name="offset">Offset to evaluate.</param>
        /// <returns>
        /// True if <paramref name="offset"/> is at or outside the document bounds or between two non-alphanumeric characters; otherwise false.
        /// </returns>
        private bool IsWordBoundary(int offset)
        {
            return offset <= 0 || offset >= _document.TextLength || !IsAlphaNumeric(offset - 1) || !IsAlphaNumeric(offset);
        }
        /// <summary>
        /// Determines whether the character at the given offset is considered alphanumeric for word matching.
        /// </summary>
        /// <param name="offset">Offset of the character to inspect.</param>
        /// <returns>True if the character is a letter, digit, or underscore; otherwise false.</returns>
        private bool IsAlphaNumeric(int offset)
        {
            char c = _document.GetCharAt(offset);
            return Char.IsLetterOrDigit(c) || c == '_';
        }
        /// <summary>
        /// Checks whether the substring at <paramref name="offset"/> matches the search string according to case settings.
        /// </summary>
        /// <param name="offset">Offset where the substring begins.</param>
        /// <returns>True if the substring equals <see cref="LookFor"/> (respecting <see cref="MatchCase"/>); otherwise false.</returns>
        private bool IsPartWordMatch(int offset)
        {
            string substr = _document.GetText(offset, _lookFor.Length);
            if (!MatchCase)
                substr = substr.ToUpperInvariant();
            return substr == _lookFor2;
        }
    }
}
