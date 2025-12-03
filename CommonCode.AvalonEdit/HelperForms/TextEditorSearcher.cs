using ICSharpCode.AvalonEdit.Document;
using System;
using System.Diagnostics;

namespace ZidUtilities.CommonCode.AvalonEdit.HelperForms
{
    /// <summary>
    /// Provides search functionality over a <see cref="TextDocument"/> similar to a "Find" operation.
    /// Maintains an optional scan region that adjusts automatically with document changes.
    /// </summary>
    public class TextEditorSearcher : IDisposable
    {
        private TextDocument _document;
        private TextSegment _region = null;

        /// <summary>
        /// Gets or sets the target document to search.
        /// Setting a different document clears any existing scan region.
        /// </summary>
        public TextDocument Document
        {
            get => _document;
            set
            {
                if (_document != value)
                {
                    ClearScanRegion();
                    _document = value;
                }
            }
        }

        /// <summary>
        /// Sets the region to search by offset and length.
        /// </summary>
        public void SetScanRegion(int offset, int length)
        {
            _region = new TextSegment { StartOffset = offset, Length = length };
        }

        /// <summary>
        /// Gets a value indicating whether a scan region is currently set.
        /// </summary>
        public bool HasScanRegion => _region != null;

        /// <summary>
        /// Clears any previously set scan region.
        /// </summary>
        public void ClearScanRegion()
        {
            _region = null;
        }

        public void Dispose()
        {
            ClearScanRegion();
            GC.SuppressFinalize(this);
        }

        ~TextEditorSearcher() { Dispose(); }

        /// <summary>
        /// Gets the beginning offset of the scan region, or 0 if no region is set.
        /// </summary>
        public int BeginOffset => _region?.StartOffset ?? 0;

        /// <summary>
        /// Gets the end offset of the scan region, or the document's text length if no region is set.
        /// </summary>
        public int EndOffset => _region != null ? _region.EndOffset : _document.TextLength;

        public bool MatchCase { get; set; }
        public bool MatchWholeWordOnly { get; set; }

        private string _lookFor;
        private string _lookFor2;

        public string LookFor
        {
            get => _lookFor;
            set => _lookFor = value;
        }

        public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
        {
            Debug.Assert(!string.IsNullOrEmpty(_lookFor));
            loopedAround = false;

            int startAt = BeginOffset, endAt = EndOffset;
            int curOffs = Math.Max(startAt, Math.Min(beginAtOffset, endAt));

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

        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
        {
            Debug.Assert(offset2 >= offset1);
            offset2 -= _lookFor.Length;

            Func<char, char, bool> matchFirstCh;
            Func<int, bool> matchWord;

            if (MatchCase)
                matchFirstCh = (lookFor, c) => (lookFor == c);
            else
                matchFirstCh = (lookFor, c) => (lookFor == char.ToUpperInvariant(c));

            if (MatchWholeWordOnly)
                matchWord = IsWholeWordMatch;
            else
                matchWord = IsPartWordMatch;

            char lookForCh = _lookFor2[0];
            if (searchBackward)
            {
                for (int offset = offset2; offset >= offset1; offset--)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset)) && matchWord(offset))
                        return new TextRange { Offset = offset, Length = _lookFor.Length };
                }
            }
            else
            {
                for (int offset = offset1; offset <= offset2; offset++)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset)) && matchWord(offset))
                        return new TextRange { Offset = offset, Length = _lookFor.Length };
                }
            }
            return null;
        }

        private bool IsWholeWordMatch(int offset)
        {
            if (IsWordBoundary(offset) && IsWordBoundary(offset + _lookFor.Length))
                return IsPartWordMatch(offset);
            return false;
        }

        private bool IsWordBoundary(int offset)
        {
            return offset <= 0 || offset >= _document.TextLength || !IsAlphaNumeric(offset - 1) || !IsAlphaNumeric(offset);
        }

        private bool IsAlphaNumeric(int offset)
        {
            char c = _document.GetCharAt(offset);
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private bool IsPartWordMatch(int offset)
        {
            string substr = _document.GetText(offset, _lookFor.Length);
            if (!MatchCase)
                substr = substr.ToUpperInvariant();
            return substr == _lookFor2;
        }
    }
}
