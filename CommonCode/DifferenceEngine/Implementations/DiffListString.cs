using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ZidUtilities.CommonCode.DifferenceEngine.Structure;

namespace ZidUtilities.CommonCode.DifferenceEngine.Implementations
{
    /// <summary>
    /// Represents a read-only list of characters backed by a single string.
    /// This class adapts a <see cref="string"/> to the <see cref="IDiffList"/> interface used by the difference engine.
    /// </summary>
    public class DiffListString : IDiffList
    {
        private string chars;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffListString"/> class with the provided string.
        /// </summary>
        /// <param name="str">The source string to adapt as a diff list. May be null or empty; in such cases <see cref="Count"/> returns 0.</param>
        public DiffListString(string str)
        {
            chars = str;
        }

        #region IDiffList Members
        /// <summary>
        /// Gets the number of items in this diff list.
        /// </summary>
        /// <returns>
        /// The number of characters in the underlying string. Returns 0 if the underlying string is null or empty.
        /// </returns>
        public int Count()
        {
            return String.IsNullOrEmpty(chars) ? 0 : chars.Length;
        }

        /// <summary>
        /// Retrieves the element at the specified zero-based index from the diff list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>
        /// The character at the specified index, returned as an <see cref="IComparable"/>
        /// Consumers can cast the result to <see cref="char"/> when needed.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
        /// </exception>
        public IComparable GetByIndex(int index)
        {
            return chars[index];
        }
        #endregion
    }
}
