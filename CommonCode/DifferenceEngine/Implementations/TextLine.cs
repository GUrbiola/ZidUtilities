using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ZidUtilities.CommonCode.DifferenceEngine.Implementations
{
    /// <summary>
    /// Represents a single line of text used by the difference engine.
    /// Holds the original line (with tabs expanded to spaces) and a cached hash
    /// used for fast comparisons.
    /// </summary>
    public class TextLine : IComparable
    {
        /// <summary>
        /// The line text with tabs replaced by spaces.
        /// </summary>
        public string Line;

        /// <summary>
        /// Cached hash code computed from the original input string.
        /// Used for fast comparisons between TextLine instances.
        /// </summary>
        public int _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextLine"/> class.
        /// Replaces tab characters in the provided string with four spaces and
        /// stores the hash code of the original input string.
        /// </summary>
        /// <param name="str">
        /// The input text line. Tab characters ('\t') will be replaced with four spaces
        /// when stored in the <see cref="Line"/> field.
        /// </param>
        public TextLine(string str)
        {
            Line = str.Replace("\t", "    ");
            _hash = str.GetHashCode();
        }

        #region IComparable Members
        /// <summary>
        /// Compares this instance to another object and returns an integer that indicates
        /// whether this instance precedes, follows, or occurs in the same position in the sort order
        /// as the other object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with this instance. This method expects an instance of <see cref="TextLine"/>.
        /// </param>
        /// <returns>
        /// A signed integer that indicates the relative values of this instance and <paramref name="obj"/>:
        /// - Less than zero: This instance precedes <paramref name="obj"/>.
        /// - Zero: This instance occurs in the same position as <paramref name="obj"/>.
        /// - Greater than zero: This instance follows <paramref name="obj"/>.
        /// The comparison is based on the cached <see cref="_hash"/> values.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Thrown if <paramref name="obj"/> is not a <see cref="TextLine"/> instance.
        /// </exception>
        public int CompareTo(object obj)
        {
            return _hash.CompareTo(((TextLine)obj)._hash);
        }
        #endregion
    }
}
