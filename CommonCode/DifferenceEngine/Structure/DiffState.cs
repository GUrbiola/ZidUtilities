using System;
using System.Collections;

namespace ZidUtilities.CommonCode.DifferenceEngine.Structure
{
    internal class DiffState
    {
        private const int BAD_INDEX = -1;
        private int _startIndex;
        private int _length;

        /// <summary>
        /// Gets the start index of the matched segment.
        /// If the state is Unknown or NoMatch this will be set to <see cref="BAD_INDEX"/>.
        /// </summary>
        public int StartIndex { get { return _startIndex; } }

        /// <summary>
        /// Gets the end index of the matched segment, computed as (StartIndex + Length - 1).
        /// Note: when the internal length state is not a positive match this value may not represent a valid index.
        /// </summary>
        public int EndIndex { get { return ((_startIndex + _length) - 1); } }

        /// <summary>
        /// Gets the effective length of the matched segment.
        /// Behavior:
        /// - If the internal _length is > 0 returns _length.
        /// - If the internal _length == 0 returns 1.
        /// - Otherwise returns 0.
        /// </summary>
        /// <returns>
        /// The effective length according to the internal representation rules.
        /// </returns>
        public int Length
        {
            get
            {
                int len;
                if (_length > 0)
                {
                    len = _length;
                }
                else
                {
                    if (_length == 0)
                    {
                        len = 1;
                    }
                    else
                    {
                        len = 0;
                    }
                }
                return len;
            }
        }

        /// <summary>
        /// Gets the high-level diff status interpreted from the internal _length field.
        /// Mapping:
        /// - _length &gt; 0 => <see cref="DiffStatus.Matched"/>
        /// - _length == -1 => <see cref="DiffStatus.NoMatch"/>
        /// - otherwise => <see cref="DiffStatus.Unknown"/>
        /// </summary>
        /// <returns>
        /// A <see cref="DiffStatus"/> value representing the current state.
        /// </returns>
        public DiffStatus Status
        {
            get
            {
                DiffStatus stat;
                if (_length > 0)
                {
                    stat = DiffStatus.Matched;
                }
                else
                {
                    switch (_length)
                    {
                        case -1:
                            stat = DiffStatus.NoMatch;
                            break;
                        default:
                            System.Diagnostics.Debug.Assert(_length == -2, "Invalid status: _length < -2");
                            stat = DiffStatus.Unknown;
                            break;
                    }
                }
                return stat;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffState"/> class and sets it to Unknown.
        /// </summary>
        public DiffState()
        {
            SetToUnkown();
        }

        /// <summary>
        /// Sets the internal state to Unknown.
        /// Side effects:
        /// - _startIndex is set to <see cref="BAD_INDEX"/>.
        /// - _length is set to the numeric value of <see cref="DiffStatus.Unknown"/>.
        /// </summary>
        protected void SetToUnkown()
        {
            _startIndex = BAD_INDEX;
            _length = (int)DiffStatus.Unknown;
        }

        /// <summary>
        /// Sets this state to a matched range with the provided start and length.
        /// Asserts that <paramref name="start"/> is non-negative and <paramref name="length"/> is greater than zero.
        /// </summary>
        /// <param name="start">The start index of the match (must be >= 0).</param>
        /// <param name="length">The length of the match (must be &gt; 0).</param>
        public void SetMatch(int start, int length)
        {
            System.Diagnostics.Debug.Assert(length > 0, "Length must be greater than zero");
            System.Diagnostics.Debug.Assert(start >= 0, "Start must be greater than or equal to zero");
            _startIndex = start;
            _length = length;
        }

        /// <summary>
        /// Sets this state to indicate there was no match.
        /// Side effects:
        /// - _startIndex is set to <see cref="BAD_INDEX"/>.
        /// - _length is set to the numeric value of <see cref="DiffStatus.NoMatch"/>.
        /// </summary>
        public void SetNoMatch()
        {
            _startIndex = BAD_INDEX;
            _length = (int)DiffStatus.NoMatch;
        }

        /// <summary>
        /// Validates whether the currently stored match length remains valid given updated bounds and the
        /// maximum possible destination length. If the stored match is no longer valid it resets the state to Unknown.
        /// </summary>
        /// <param name="newStart">
        /// The new allowed start index for the destination range. If the stored start is less than this, the match
        /// will be considered invalid.
        /// </param>
        /// <param name="newEnd">
        /// The new allowed end index for the destination range. If the stored end index (computed from stored start and length)
        /// is greater than this, the match will be considered invalid.
        /// </param>
        /// <param name="maxPossibleDestLength">
        /// The maximum possible length available in the destination for a match. If this value is smaller than the stored
        /// match length, the stored match is considered invalid.
        /// </param>
        /// <returns>
        /// True if after validation the internal state is not Unknown (i.e., it represents either Matched or NoMatch);
        /// false if the state was reset to Unknown.
        /// </returns>
        public bool HasValidLength(int newStart, int newEnd, int maxPossibleDestLength)
        {
            if (_length > 0) //have unlocked match
            {
                if ((maxPossibleDestLength < _length) ||
                    ((_startIndex < newStart) || (EndIndex > newEnd)))
                {
                    SetToUnkown();
                }
            }
            return (_length != (int)DiffStatus.Unknown);
        }
    }
}
